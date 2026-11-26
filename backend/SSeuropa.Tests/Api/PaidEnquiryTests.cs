using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;

namespace SSeuropa.Tests.Api;

/// <summary>
/// Enquiry marked Paid → a student account appears under Students → "New (paid)" without a login.
/// A Teacher (view-only on Students) or anyone with full access then places them in a batch,
/// which emails the one-time password.
/// </summary>
public class PaidEnquiryTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() },
    };

    public PaidEnquiryTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.WithWebHostBuilder(b => b.UseSetting("Database:Provider", "InMemory")).CreateClient();
    }

    [Fact]
    public async Task PaidEnquiry_BecomesStudent_TeacherSendsLogin()
    {
        var staff = await DemoLoginAsync("staff@sseuropa.demo");
        var admin = await DemoLoginAsync("admin@sseuropa.demo");
        var teacherAuth = await DemoLoginDataAsync("mentor@sseuropa.demo");
        var teacher = teacherAuth.AccessToken;

        var email = $"lead-{Guid.NewGuid():N}@example.com";
        var enquiry = await SubmitEnquiryAsync("Harpreet Kaur Sandhu", email);

        // Enrolled is only a label: no account yet.
        var enrolled = await SendAsync<EnquiryRow>(HttpMethod.Put, $"/api/enquiries/{enquiry.Id}/status", staff, new { status = "Enrolled" });
        enrolled.ConvertedUserId.Should().BeNull();

        // Paid: the account is made, waiting for its login.
        var paid = await SendAsync<EnquiryRow>(HttpMethod.Put, $"/api/enquiries/{enquiry.Id}/status", staff, new { status = "Paid" });
        paid.Status.Should().Be("Paid");
        paid.ConvertedUserId.Should().NotBeNull();
        var studentId = paid.ConvertedUserId!.Value;

        var row = (await GetAsync<List<StudentRow>>("/api/students", admin)).Single(s => s.Id == studentId);
        row.Email.Should().Be(email);
        row.FirstName.Should().Be("Harpreet");
        row.LastName.Should().Be("Kaur Sandhu");
        row.AwaitingLogin.Should().BeTrue();
        row.BatchName.Should().BeNull();

        // Saving Paid again doesn't make a second account.
        (await SendAsync<EnquiryRow>(HttpMethod.Put, $"/api/enquiries/{enquiry.Id}/status", staff, new { status = "Paid", staffNotes = "again" }))
            .ConvertedUserId.Should().Be(studentId);
        (await GetAsync<List<StudentRow>>("/api/students", admin)).Count(s => s.Email == email).Should().Be(1);

        // The Teacher sees the waiting student, but can only use a batch they teach.
        (await GetAsync<List<StudentRow>>("/api/students", teacher)).Should().Contain(s => s.Id == studentId);
        var other = await SendAsync<BatchRow>(HttpMethod.Post, "/api/batches", admin, new { name = $"Other {Guid.NewGuid():N}", courseIds = Array.Empty<Guid>() });
        (await RawAsync(HttpMethod.Post, $"/api/students/{studentId}/give-access", teacher, new { batchIds = new[] { other.Id } }))
            .StatusCode.Should().Be(HttpStatusCode.BadRequest);
        (await RawAsync(HttpMethod.Post, $"/api/students/{studentId}/give-access", teacher, new { batchIds = Array.Empty<Guid>() }))
            .StatusCode.Should().Be(HttpStatusCode.BadRequest);

        // Until the login is sent, a batch can't be given another way (Edit, bulk move) and no password reset.
        (await RawAsync(HttpMethod.Put, $"/api/students/{studentId}", admin, new { firstName = "Harpreet", lastName = "Kaur", batchIds = new[] { other.Id } }))
            .StatusCode.Should().Be(HttpStatusCode.BadRequest);
        (await RawAsync(HttpMethod.Post, "/api/students/move", admin, new { studentIds = new[] { studentId }, batchId = other.Id, mode = "add" }))
            .StatusCode.Should().Be(HttpStatusCode.BadRequest);
        (await RawAsync(HttpMethod.Post, $"/api/students/{studentId}/reset-password", admin))
            .StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var mine = await SendAsync<BatchRow>(HttpMethod.Post, "/api/batches", admin, new { name = $"Mine {Guid.NewGuid():N}", mentorId = teacherAuth.User.Id, courseIds = Array.Empty<Guid>() });
        var result = await SendAsync<AccessResult>(HttpMethod.Post, $"/api/students/{studentId}/give-access", teacher, new { batchIds = new[] { mine.Id } });
        result.TemporaryPassword.Should().NotBeNullOrWhiteSpace();
        result.Student.AwaitingLogin.Should().BeFalse();
        result.Student.BatchName.Should().Be(mine.Name);

        // The one-time password works and must be changed.
        (await PasswordLoginAsync(email, result.TemporaryPassword!)).User.MustChangePassword.Should().BeTrue();

        // Once access is given, a view-only Teacher can't use it again (e.g. to reset the password).
        (await RawAsync(HttpMethod.Post, $"/api/students/{studentId}/give-access", teacher, new { batchIds = new[] { mine.Id } }))
            .StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task PaidEnquiry_WithTeamMemberEmail_IsRefused()
    {
        var staff = await DemoLoginAsync("staff@sseuropa.demo");
        var enquiry = await SubmitEnquiryAsync("Someone", "admin@sseuropa.demo");
        (await RawAsync(HttpMethod.Put, $"/api/enquiries/{enquiry.Id}/status", staff, new { status = "Paid" }))
            .StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    private async Task<EnquiryRow> SubmitEnquiryAsync(string name, string email)
    {
        var res = await _client.PostAsJsonAsync("/api/enquiries", new
        {
            fullName = name, email, phone = "+91 98765 43210", interestedCourse = "Italian A1",
            preferredLanguage = "Punjabi", message = "I want to join."
        });
        res.StatusCode.Should().Be(HttpStatusCode.OK);
        return (await res.Content.ReadFromJsonAsync<Envelope<EnquiryRow>>(JsonOptions))!.Data!;
    }

    private async Task<string> DemoLoginAsync(string email) => (await DemoLoginDataAsync(email)).AccessToken;

    private async Task<AuthData> DemoLoginDataAsync(string email)
    {
        var res = await _client.PostAsJsonAsync("/api/auth/demo-login", new { email, deviceId = $"t-{email}", deviceName = "Test" });
        res.StatusCode.Should().Be(HttpStatusCode.OK);
        return (await res.Content.ReadFromJsonAsync<Envelope<AuthData>>(JsonOptions))!.Data!;
    }

    private async Task<AuthData> PasswordLoginAsync(string email, string password)
    {
        var res = await _client.PostAsJsonAsync("/api/auth/login", new { email, password, deviceId = $"t-{email}", deviceName = "Test" });
        res.StatusCode.Should().Be(HttpStatusCode.OK);
        return (await res.Content.ReadFromJsonAsync<Envelope<AuthData>>(JsonOptions))!.Data!;
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
        return JsonSerializer.Deserialize<Envelope<T>>(text, JsonOptions)!.Data!;
    }

    private Task<T> GetAsync<T>(string path, string token) => SendAsync<T>(HttpMethod.Get, path, token);

    private sealed record Envelope<T>(bool Success, string? Message, T? Data);
    private sealed record UserData(Guid Id, string Email, bool MustChangePassword);
    private sealed record AuthData(string AccessToken, UserData User);
    private sealed record EnquiryRow(Guid Id, string Status, Guid? ConvertedUserId);
    private sealed record BatchRow(Guid Id, string Name);
    private sealed record StudentRow(Guid Id, string Email, string FirstName, string LastName, string? BatchName, bool AwaitingLogin);
    private sealed record AccessResult(StudentRow Student, string? TemporaryPassword);
}
