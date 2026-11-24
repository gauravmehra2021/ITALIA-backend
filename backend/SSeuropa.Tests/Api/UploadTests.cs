using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace SSeuropa.Tests.Api;

/// <summary>
/// Teachers upload lesson videos, images and PDFs. On the live server the files must stay on a
/// folder that survives deploys, links must use the public https address, videos up to 500 MB
/// must be accepted, and nobody may write files outside the uploads folder.
/// </summary>
public class UploadTests : IClassFixture<WebApplicationFactory<Program>>, IDisposable
{
    private readonly WebApplicationFactory<Program> _app;
    private readonly HttpClient _client;
    private readonly string _uploads = Path.Combine(Path.GetTempPath(), "sseuropa-upload-test-" + Guid.NewGuid().ToString("N"));

    public UploadTests(WebApplicationFactory<Program> factory)
    {
        _app = factory.WithWebHostBuilder(b => b
            .UseSetting("Database:Provider", "InMemory")
            .UseSetting("Storage:UploadsPath", _uploads)
            .UseSetting("Storage:PublicBaseUrl", "https://api.example.test"));
        _client = _app.CreateClient();
    }

    public void Dispose()
    {
        if (Directory.Exists(_uploads)) Directory.Delete(_uploads, recursive: true);
    }

    [Fact]
    public async Task TeacherUpload_IsStoredInTheConfiguredFolder_LinkedWithThePublicAddress_AndServed()
    {
        var teacher = await LoginAsync("mentor@sseuropa.demo");
        var bytes = new byte[] { 0x89, 0x50, 0x4E, 0x47, 1, 2, 3, 4 };
        using var form = new MultipartFormDataContent
        {
            { new ByteArrayContent(bytes) { Headers = { ContentType = new MediaTypeHeaderValue("image/png") } }, "file", "lesson photo.png" },
            { new StringContent("images"), "category" },
        };
        using var req = new HttpRequestMessage(HttpMethod.Post, "/api/media/upload") { Content = form };
        req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", teacher);
        var res = await _client.SendAsync(req);
        res.StatusCode.Should().Be(HttpStatusCode.OK);

        var url = (await res.Content.ReadFromJsonAsync<Envelope<Uploaded>>())!.Data!.Url;
        url.Should().StartWith("https://api.example.test/uploads/images/");
        var relative = new Uri(url).AbsolutePath;
        File.Exists(Path.Combine(_uploads, relative["/uploads/".Length..])).Should().BeTrue();

        var served = await _client.GetAsync(relative);
        served.StatusCode.Should().Be(HttpStatusCode.OK);
        (await served.Content.ReadAsByteArrayAsync()).Should().Equal(bytes);
    }

    [Fact]
    public void LessonVideosUpTo500Mb_AreAcceptedByTheFormReader()
    {
        var form = _app.Services.GetRequiredService<IOptions<FormOptions>>().Value;
        form.MultipartBodyLengthLimit.Should().Be(500L * 1024 * 1024);
    }

    [Fact]
    public async Task LocalDirect_RejectsAnonymousCallers_AndKeysOutsideTheUploadsFolder()
    {
        var evil = "courses/../../../evil.txt";
        var anon = await _client.PutAsync($"/api/uploads/local-direct?key={Uri.EscapeDataString(evil)}", new StringContent("x"));
        anon.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        var teacher = await LoginAsync("mentor@sseuropa.demo");
        foreach (var key in new[] { evil, "../appsettings.json", "notcourses/x.txt" })
        {
            using var req = new HttpRequestMessage(HttpMethod.Put, $"/api/uploads/local-direct?key={Uri.EscapeDataString(key)}") { Content = new StringContent("x") };
            req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", teacher);
            (await _client.SendAsync(req)).StatusCode.Should().Be(HttpStatusCode.BadRequest, key);
        }
        File.Exists(Path.Combine(Path.GetDirectoryName(_uploads)!, "evil.txt")).Should().BeFalse();

        using var ok = new HttpRequestMessage(HttpMethod.Put, "/api/uploads/local-direct?key=courses%2Fdemo%2Fvideo.mp4") { Content = new StringContent("video") };
        ok.Headers.Authorization = new AuthenticationHeaderValue("Bearer", teacher);
        (await _client.SendAsync(ok)).StatusCode.Should().Be(HttpStatusCode.OK);
        File.Exists(Path.Combine(_uploads, "courses", "demo", "video.mp4")).Should().BeTrue();
    }

    [Fact]
    public async Task BehindAProxy_WithoutPublicBaseUrl_LinksUseTheForwardedHttpsAddress()
    {
        var client = _app.WithWebHostBuilder(b => b.UseSetting("Storage:PublicBaseUrl", "")).CreateClient();
        var teacher = await LoginAsync("mentor@sseuropa.demo", client);
        using var form = new MultipartFormDataContent { { new ByteArrayContent([1, 2, 3]), "file", "doc.pdf" } };
        using var req = new HttpRequestMessage(HttpMethod.Post, "/api/media/upload") { Content = form };
        req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", teacher);
        req.Headers.Add("X-Forwarded-Proto", "https");
        req.Headers.Add("X-Forwarded-Host", "api.sseuropa.com");
        var res = await client.SendAsync(req);
        res.StatusCode.Should().Be(HttpStatusCode.OK);
        (await res.Content.ReadFromJsonAsync<Envelope<Uploaded>>())!.Data!.Url.Should().StartWith("https://api.sseuropa.com/uploads/general/");
    }

    private async Task<string> LoginAsync(string email, HttpClient? client = null)
    {
        var res = await (client ?? _client).PostAsJsonAsync("/api/auth/demo-login", new { email, deviceId = $"u-{email}", deviceName = "Test" });
        res.StatusCode.Should().Be(HttpStatusCode.OK);
        return (await res.Content.ReadFromJsonAsync<Envelope<Auth>>())!.Data!.AccessToken;
    }

    private sealed record Envelope<T>(bool Success, T? Data);
    private sealed record Auth(string AccessToken);
    private sealed record Uploaded(string Url, string FileName, long FileSize);
}
