using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SSeuropa.Infrastructure.Persistence;

namespace SSeuropa.Tests.Api;

/// <summary>Punjabi / Hindi / Urdu translations of lesson content (translation memory).</summary>
public class TranslationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _app;
    private readonly HttpClient _client;
    private static readonly JsonSerializerOptions Json = new() { PropertyNameCaseInsensitive = true };

    public TranslationTests(WebApplicationFactory<Program> factory)
    {
        _app = factory.WithWebHostBuilder(b => b.UseSetting("Database:Provider", "InMemory"));
        _client = _app.CreateClient();
    }

    [Fact]
    public async Task Student_GetsTranslations_ForTextsOnScreen()
    {
        var student = await LoginAsync("priya@sseuropa.demo");

        // "tree" comes with the built-in course content (Punjabi written on the word).
        var pa = await PostAsync<Dictionary<string, string>>("/api/learn/translations", student,
            new { language = "Punjabi", texts = new[] { "tree", "a text nobody translated", "tree" } });
        pa.Should().ContainKey("tree").WhoseValue.Should().Be("Rukh / Darakhat");
        pa.Should().NotContainKey("a text nobody translated");

        (await PostAsync<Dictionary<string, string>>("/api/learn/translations", student, new { language = "English", texts = new[] { "tree" } }))
            .Should().BeEmpty();
        (await RawAsync(HttpMethod.Post, "/api/learn/translations", student, new { language = "Klingon", texts = new[] { "tree" } }))
            .StatusCode.Should().Be(HttpStatusCode.BadRequest);
        (await _client.PostAsJsonAsync("/api/learn/translations", new { language = "Punjabi", texts = new[] { "tree" } }))
            .StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Admin_ListsCourseTexts_SavesTranslation_AndRestartKeepsTheEdit()
    {
        var admin = await LoginAsync("admin@sseuropa.demo");
        var courses = await GetAsync<JsonElement>("/api/courses?pageSize=50", admin);
        var courseId = courses.GetProperty("items").EnumerateArray()
            .First(c => c.GetProperty("slug").GetString() == "corso-italiano-capitolo-1").GetProperty("id").GetString();

        var page = await GetAsync<JsonElement>($"/api/translations?courseId={courseId}&search=Lesson%201%20%C2%B7%20The%20Italian%20alphabet", admin);
        page.GetProperty("totalTexts").GetInt32().Should().BeGreaterThan(300);
        var row = page.GetProperty("page").GetProperty("items").EnumerateArray().Single();
        row.GetProperty("source").GetString().Should().Be("Lesson 1 · The Italian alphabet");

        // Save Punjabi + Hindi + Urdu for the lesson title; students see it.
        await SendAsync<JsonElement>(HttpMethod.Put, "/api/translations", admin, new
        {
            source = "Lesson 1 · The Italian alphabet",
            punjabi = "Lesson 1 · Italian varnmala (test)",
            hindi = "पाठ 1 · इटालियन वर्णमाला (test)",
            urdu = "سبق 1 · اطالوی حروفِ تہجی (test)",
        });
        var student = await LoginAsync("priya@sseuropa.demo");
        var hi = await PostAsync<Dictionary<string, string>>("/api/learn/translations", student,
            new { language = "Hindi", texts = new[] { "Lesson 1 · The Italian alphabet" } });
        hi["Lesson 1 · The Italian alphabet"].Should().Be("पाठ 1 · इटालियन वर्णमाला (test)");
        var ur = await PostAsync<Dictionary<string, string>>("/api/learn/translations", student,
            new { language = "Urdu", texts = new[] { "Lesson 1 · The Italian alphabet" } });
        ur["Lesson 1 · The Italian alphabet"].Should().Be("سبق 1 · اطالوی حروفِ تہجی (test)");

        // An older client that doesn't send Urdu keeps it; an empty string removes it.
        var kept = await SendAsync<JsonElement>(HttpMethod.Put, "/api/translations", admin, new
        {
            source = "Lesson 1 · The Italian alphabet",
            punjabi = "Lesson 1 · Italian varnmala (test)",
            hindi = "पाठ 1 · इटालियन वर्णमाला (test)",
        });
        kept.GetProperty("urdu").GetString().Should().Be("سبق 1 · اطالوی حروفِ تہجی (test)");
        var cleared = await SendAsync<JsonElement>(HttpMethod.Put, "/api/translations", admin, new { source = "Lesson 1 · The Italian alphabet", urdu = "" });
        cleared.GetProperty("urdu").ValueKind.Should().Be(JsonValueKind.Null);
        cleared.GetProperty("hindi").GetString().Should().Be("पाठ 1 · इटालियन वर्णमाला (test)");
        await SendAsync<JsonElement>(HttpMethod.Put, "/api/translations", admin, new { source = "Lesson 1 · The Italian alphabet", urdu = "سبق 1 · اطالوی حروفِ تہجی (test)" });

        // Restarting (re-seeding the built-in translations) never replaces an admin's edit.
        var seeder = _app.Services.GetServices<IHostedService>().OfType<CourseContentSeeder>().Single();
        await seeder.StartAsync(CancellationToken.None);
        var pa = await PostAsync<Dictionary<string, string>>("/api/learn/translations", student,
            new { language = "Punjabi", texts = new[] { "Lesson 1 · The Italian alphabet" } });
        pa["Lesson 1 · The Italian alphabet"].Should().Be("Lesson 1 · Italian varnmala (test)");

        // Missing filter lists only texts without that language.
        var missing = await GetAsync<JsonElement>($"/api/translations?courseId={courseId}&missing=hindi&pageSize=200", admin);
        missing.GetProperty("page").GetProperty("items").EnumerateArray()
            .Should().OnlyContain(r => r.GetProperty("hindi").ValueKind == JsonValueKind.Null);
        var missingUr = await GetAsync<JsonElement>($"/api/translations?courseId={courseId}&missing=urdu&pageSize=200", admin);
        missingUr.GetProperty("page").GetProperty("items").EnumerateArray()
            .Should().OnlyContain(r => r.GetProperty("urdu").ValueKind == JsonValueKind.Null);
    }

    [Fact]
    public async Task Students_CannotManageTranslations()
    {
        var student = await LoginAsync("priya@sseuropa.demo");
        (await RawAsync(HttpMethod.Get, "/api/translations", student)).StatusCode.Should().Be(HttpStatusCode.Forbidden);
        (await RawAsync(HttpMethod.Put, "/api/translations", student, new { source = "tree", punjabi = "x" }))
            .StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    // ── helpers ─────────────────────────────────────────────────────────────

    private async Task<string> LoginAsync(string email)
    {
        var res = await _client.PostAsJsonAsync("/api/auth/demo-login", new { email, deviceId = $"tr-{email}", deviceName = "Test" });
        res.StatusCode.Should().Be(HttpStatusCode.OK);
        var doc = JsonDocument.Parse(await res.Content.ReadAsStringAsync());
        return doc.RootElement.GetProperty("data").GetProperty("accessToken").GetString()!;
    }

    private async Task<HttpResponseMessage> RawAsync(HttpMethod method, string path, string token, object? body = null)
    {
        using var req = new HttpRequestMessage(method, path);
        req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        if (body is not null) req.Content = JsonContent.Create(body);
        return await _client.SendAsync(req);
    }

    private async Task<T> SendAsync<T>(HttpMethod method, string path, string token, object? body = null)
    {
        var res = await RawAsync(method, path, token, body);
        var text = await res.Content.ReadAsStringAsync();
        res.IsSuccessStatusCode.Should().BeTrue($"{method} {path} returned {(int)res.StatusCode}: {text}");
        return JsonDocument.Parse(text).RootElement.GetProperty("data").Deserialize<T>(Json)!;
    }

    private Task<T> GetAsync<T>(string path, string token) => SendAsync<T>(HttpMethod.Get, path, token);
    private Task<T> PostAsync<T>(string path, string token, object body) => SendAsync<T>(HttpMethod.Post, path, token, body);
}
