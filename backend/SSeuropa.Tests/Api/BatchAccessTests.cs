using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;

namespace SSeuropa.Tests.Api;

/// <summary>
/// Students are grouped into batches; a batch's courses are the only courses its students can open.
/// The team adds students (temporary password by email, changed at first sign-in).
/// </summary>
public class BatchAccessTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() },
    };

    public BatchAccessTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.WithWebHostBuilder(b => b.UseSetting("Database:Provider", "InMemory")).CreateClient();
    }

    [Fact]
    public async Task NewStudent_SeesOnlyBatchCourses_AndLosesThemWhenMovedOut()
    {
        var admin = await DemoLoginAsync("admin@sseuropa.demo");
        var courses = await GetAsync<List<CourseCard>>("/api/learn/courses", admin);
        var a0 = courses.Single(c => c.Course.Slug == "corso-italiano-a0").Course.Id;
        var a1 = courses.Single(c => c.Course.Slug == "italian-a1-foundations").Course.Id;

        // Added by the team → temporary password, must change it.
        var (studentId, student) = await CreateStudentAsync(admin);

        // No batch yet: nothing to open, and joining a course is refused.
        (await GetAsync<List<CourseCard>>("/api/learn/courses", student)).Should().BeEmpty();
        (await RawAsync(HttpMethod.Post, $"/api/learn/courses/{a1}/enroll", student)).StatusCode.Should().Be(HttpStatusCode.Forbidden);
        (await RawAsync(HttpMethod.Get, $"/api/learn/courses/{a1}", student)).StatusCode.Should().Be(HttpStatusCode.Forbidden);

        // Batch with the A0 course → the student is enrolled in it automatically, A1 stays closed.
        var batch = await SendAsync<BatchRow>(HttpMethod.Post, "/api/batches", admin, new { name = $"A0 Morning {Guid.NewGuid():N}", level = "a0", courseIds = new[] { a0 } });
        batch.Level.Should().Be("A0");
        await SendAsync<JsonElement>(HttpMethod.Post, "/api/students/move", admin, new { studentIds = new[] { studentId }, batchId = batch.Id });

        var mine = await GetAsync<List<CourseCard>>("/api/learn/courses", student);
        mine.Should().ContainSingle().Which.Course.Id.Should().Be(a0);
        mine[0].IsEnrolled.Should().BeTrue();
        (await RawAsync(HttpMethod.Post, $"/api/learn/courses/{a1}/enroll", student)).StatusCode.Should().Be(HttpStatusCode.Forbidden);

        // Assigning A1 to the batch opens it for everyone in the batch.
        await SendAsync<BatchRow>(HttpMethod.Put, $"/api/batches/{batch.Id}", admin, new { name = batch.Name, level = "A0", courseIds = new[] { a0, a1 } });
        (await GetAsync<List<CourseCard>>("/api/learn/courses", student)).Should().HaveCount(2).And.OnlyContain(c => c.IsEnrolled);

        var row = (await GetAsync<List<StudentRow>>("/api/students", admin)).Single(s => s.Id == studentId);
        row.BatchName.Should().Be(batch.Name);
        row.ActiveCourses.Should().Be(2);
        row.AddedByStaff.Should().BeTrue();

        // A batch with students can't be deleted.
        (await RawAsync(HttpMethod.Delete, $"/api/batches/{batch.Id}", admin)).StatusCode.Should().Be(HttpStatusCode.BadRequest);

        // Out of the batch → courses close again.
        await SendAsync<JsonElement>(HttpMethod.Post, "/api/students/move", admin, new { studentIds = new[] { studentId }, batchId = (Guid?)null });
        (await GetAsync<List<CourseCard>>("/api/learn/courses", student)).Should().BeEmpty();
        (await RawAsync(HttpMethod.Delete, $"/api/batches/{batch.Id}", admin)).StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task StudentAddedByTeam_MustChangeTemporaryPassword()
    {
        var admin = await DemoLoginAsync("admin@sseuropa.demo");
        var email = $"pw-{Guid.NewGuid():N}@example.com";
        var created = await SendAsync<JsonElement>(HttpMethod.Post, "/api/students", admin, new { firstName = "Simran", lastName = "Kaur", email });
        var temp = created.GetProperty("temporaryPassword").GetString()!;
        temp.Should().HaveLength(12);

        var login = await PasswordLoginAsync(email, temp);
        login.User.MustChangePassword.Should().BeTrue();

        (await RawAsync(HttpMethod.Post, "/api/auth/change-password", login.AccessToken, new { currentPassword = "wrong", newPassword = "Italia2026" }))
            .StatusCode.Should().Be(HttpStatusCode.BadRequest);
        (await RawAsync(HttpMethod.Post, "/api/auth/change-password", login.AccessToken, new { currentPassword = temp, newPassword = "short1" }))
            .StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var changed = await SendAsync<UserData>(HttpMethod.Post, "/api/auth/change-password", login.AccessToken, new { currentPassword = temp, newPassword = "Italia2026" });
        changed.MustChangePassword.Should().BeFalse();

        (await PasswordLoginAsync(email, "Italia2026")).User.MustChangePassword.Should().BeFalse();

        // Duplicate email is refused.
        (await RawAsync(HttpMethod.Post, "/api/students", admin, new { firstName = "Again", lastName = "", email }))
            .StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task Roles_StaffAddsStudents_OnlyAdminsChangeBatches_TeachersReadTheirOwn()
    {
        var admin = await DemoLoginAsync("admin@sseuropa.demo");
        var staff = await DemoLoginAsync("staff@sseuropa.demo");
        var teacher = await DemoLoginAsync("mentor@sseuropa.demo");
        var student = await DemoLoginAsync("priya@sseuropa.demo");

        // Staff (admissions) can add a student but not create a batch.
        (await RawAsync(HttpMethod.Post, "/api/students", staff, new { firstName = "Staff", lastName = "Added", email = $"staff-{Guid.NewGuid():N}@example.com" }))
            .StatusCode.Should().Be(HttpStatusCode.OK);
        (await RawAsync(HttpMethod.Post, "/api/batches", staff, new { name = "Nope", level = "A1" }))
            .StatusCode.Should().Be(HttpStatusCode.Forbidden);

        // Teachers can't add students; students can't see the list at all.
        (await RawAsync(HttpMethod.Post, "/api/students", teacher, new { firstName = "T", lastName = "X", email = $"t-{Guid.NewGuid():N}@example.com" }))
            .StatusCode.Should().Be(HttpStatusCode.Forbidden);
        (await RawAsync(HttpMethod.Get, "/api/students", student)).StatusCode.Should().Be(HttpStatusCode.Forbidden);

        // A teacher sees only the batches they lead and those students.
        var teachers = await GetAsync<List<TeacherRow>>("/api/batches/teachers", admin);
        var mentorId = teachers.First(t => t.Role == "Mentor").Id;
        var led = await SendAsync<BatchRow>(HttpMethod.Post, "/api/batches", admin, new { name = $"Led {Guid.NewGuid():N}", level = "A1", mentorId });
        var notLed = await SendAsync<BatchRow>(HttpMethod.Post, "/api/batches", admin, new { name = $"Other {Guid.NewGuid():N}", level = "A2" });
        var (inLed, _) = await CreateStudentAsync(admin, led.Id);
        var (inOther, _) = await CreateStudentAsync(admin, notLed.Id);

        var teacherBatches = await GetAsync<List<BatchRow>>("/api/batches", teacher);
        teacherBatches.Should().Contain(b => b.Id == led.Id).And.NotContain(b => b.Id == notLed.Id);
        var teacherStudents = await GetAsync<List<StudentRow>>("/api/students", teacher);
        teacherStudents.Should().Contain(s => s.Id == inLed).And.NotContain(s => s.Id == inOther);

        // Signed-up students show up in the list too (no batch yet).
        var all = await GetAsync<List<StudentRow>>("/api/students", admin);
        all.Should().Contain(s => s.Email == "priya@sseuropa.demo" && !s.AddedByStaff);
    }

    // ── helpers ─────────────────────────────────────────────────────────────

    private async Task<(Guid Id, string Token)> CreateStudentAsync(string adminToken, Guid? batchId = null)
    {
        var email = $"s-{Guid.NewGuid():N}@example.com";
        var created = await SendAsync<JsonElement>(HttpMethod.Post, "/api/students", adminToken, new { firstName = "New", lastName = "Student", email, batchId });
        var id = created.GetProperty("student").GetProperty("id").GetGuid();
        var login = await PasswordLoginAsync(email, created.GetProperty("temporaryPassword").GetString()!);
        return (id, login.AccessToken);
    }

    private async Task<string> DemoLoginAsync(string email)
    {
        var res = await _client.PostAsJsonAsync("/api/auth/demo-login", new { email, deviceId = $"t-{email}", deviceName = "Test" });
        res.StatusCode.Should().Be(HttpStatusCode.OK);
        return (await res.Content.ReadFromJsonAsync<Envelope<AuthData>>(JsonOptions))!.Data!.AccessToken;
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
    private sealed record CourseInfo(Guid Id, string Title, string? Slug);
    private sealed record CourseCard(CourseInfo Course, bool IsEnrolled);
    private sealed record BatchRow(Guid Id, string Name, string Level, int StudentCount);
    private sealed record TeacherRow(Guid Id, string FullName, string Role);
    private sealed record StudentRow(Guid Id, string Email, string? BatchName, int ActiveCourses, bool AddedByStaff);
}
