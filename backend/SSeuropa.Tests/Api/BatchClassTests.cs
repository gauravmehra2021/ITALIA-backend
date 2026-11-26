using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;

namespace SSeuropa.Tests.Api;

/// <summary>
/// Live classes are scheduled per batch: only that batch's students see the class, can open its meeting
/// and get its alerts. A student may be in several batches (e.g. morning 11–12 and evening 17–18).
/// </summary>
public class BatchClassTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() },
    };

    public BatchClassTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.WithWebHostBuilder(b => b.UseSetting("Database:Provider", "InMemory")).CreateClient();
    }

    [Fact]
    public async Task StudentInTwoBatches_GetsBothBatchesCourses_AndClasses_OthersDoNot()
    {
        var admin = await DemoLoginAsync("admin@sseuropa.demo");
        var courses = await GetAsync<List<CourseCard>>("/api/learn/courses", admin);
        var cap1 = courses.Single(c => c.Course.Slug == "corso-italiano-capitolo-1").Course.Id;
        var cap2 = courses.Single(c => c.Course.Slug == "corso-italiano-capitolo-2").Course.Id;

        var morning = await NewBatchAsync(admin, "Morning", "11:00", "12:00", cap1);
        var evening = await NewBatchAsync(admin, "Evening", "17:00", "18:00", cap2);
        morning.StartTime.Should().Be("11:00");
        evening.EndTime.Should().Be("18:00");

        var (bothId, both) = await CreateStudentAsync(admin, morning.Id, evening.Id);
        var (_, morningOnly) = await CreateStudentAsync(admin, morning.Id);
        var (_, eveningOnly) = await CreateStudentAsync(admin, evening.Id);

        // Courses of both batches are open for the student who is in both.
        (await GetAsync<List<CourseCard>>("/api/learn/courses", both)).Select(c => c.Course.Id).Should().BeEquivalentTo([cap1, cap2]);
        var row = (await GetAsync<List<StudentRow>>("/api/students", admin)).Single(s => s.Id == bothId);
        row.Batches!.Select(b => b.Name).Should().BeEquivalentTo([morning.Name, evening.Name]);

        var morningClass = await ScheduleAsync(admin, morning.Id, "Morning speaking");
        var eveningClass = await ScheduleAsync(admin, evening.Id, "Evening speaking");
        morningClass.BatchName.Should().Be(morning.Name);
        morningClass.BatchStudentCount.Should().Be(2);

        (await ClassIdsAsync(both)).Should().Contain([morningClass.Id, eveningClass.Id]);
        var forMorning = await ClassIdsAsync(morningOnly);
        forMorning.Should().Contain(morningClass.Id).And.NotContain(eveningClass.Id);
        (await ClassIdsAsync(eveningOnly)).Should().Contain(eveningClass.Id).And.NotContain(morningClass.Id);

        // The meeting link of another batch's class is refused.
        (await RawAsync(HttpMethod.Get, $"/api/classes/{eveningClass.Id}/meeting", morningOnly)).StatusCode.Should().Be(HttpStatusCode.Forbidden);
        (await RawAsync(HttpMethod.Get, $"/api/classes/{morningClass.Id}/meeting", morningOnly)).StatusCode.Should().Be(HttpStatusCode.OK);

        // Mobile app shows the same thing.
        var mobile = await GetRawListAsync("/api/v1/students/me/live-classes", eveningOnly);
        mobile.Should().Contain(eveningClass.Id.ToString()).And.NotContain(morningClass.Id.ToString());
    }

    [Fact]
    public async Task ClassStartAlert_GoesOnlyToTheBatchStudents()
    {
        var admin = await DemoLoginAsync("admin@sseuropa.demo");
        var batch = await NewBatchAsync(admin, "Alert", null, null);
        var other = await NewBatchAsync(admin, "Other", null, null);
        var (_, inBatch1) = await CreateStudentAsync(admin, batch.Id);
        await CreateStudentAsync(admin, batch.Id, other.Id);
        var (_, outsider) = await CreateStudentAsync(admin, other.Id);

        var cls = await ScheduleAsync(admin, batch.Id, "Alert test");
        var result = await SendAsync<JsonElement>(HttpMethod.Post, $"/api/classes/{cls.Id}/notify-start", admin);
        result.GetProperty("sentCount").GetInt32().Should().Be(2);

        (await NotificationTitlesAsync(inBatch1)).Should().Contain(t => t.Contains("Alert test"));
        (await NotificationTitlesAsync(outsider)).Should().NotContain(t => t.Contains("Alert test"));
    }

    [Fact]
    public async Task ClassNeedsABatch_AndCancelledClassesDisappearForStudents()
    {
        var admin = await DemoLoginAsync("admin@sseuropa.demo");
        var noBatch = await RawAsync(HttpMethod.Post, "/api/classes", admin,
            new { title = "No batch", scheduledAtUtc = DateTime.UtcNow.AddDays(1), durationMinutes = 60 });
        noBatch.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var batch = await NewBatchAsync(admin, "Cancel", null, null);
        var (_, student) = await CreateStudentAsync(admin, batch.Id);
        var cls = await ScheduleAsync(admin, batch.Id, "");
        cls.Title.Should().Contain(batch.Name); // empty title → "<batch> — live class"
        (await ClassIdsAsync(student)).Should().Contain(cls.Id);

        await SendAsync<JsonElement>(HttpMethod.Post, $"/api/classes/{cls.Id}/cancel", admin);
        (await ClassIdsAsync(student)).Should().NotContain(cls.Id);
    }

    [Fact]
    public async Task BulkAddAndRemove_KeepOtherBatches()
    {
        var admin = await DemoLoginAsync("admin@sseuropa.demo");
        var a = await NewBatchAsync(admin, "A", null, null);
        var b = await NewBatchAsync(admin, "B", null, null);
        var (id, _) = await CreateStudentAsync(admin, a.Id);

        await SendAsync<JsonElement>(HttpMethod.Post, "/api/students/move", admin, new { studentIds = new[] { id }, batchId = b.Id, mode = "add" });
        (await BatchNamesAsync(admin, id)).Should().BeEquivalentTo([a.Name, b.Name]);

        await SendAsync<JsonElement>(HttpMethod.Post, "/api/students/move", admin, new { studentIds = new[] { id }, batchId = a.Id, mode = "remove" });
        (await BatchNamesAsync(admin, id)).Should().BeEquivalentTo([b.Name]);

        await SendAsync<JsonElement>(HttpMethod.Post, "/api/students/move", admin, new { studentIds = new[] { id }, batchId = a.Id });
        (await BatchNamesAsync(admin, id)).Should().BeEquivalentTo([a.Name]);

        // Batch counts include students who are in several batches.
        var all = await GetAsync<List<BatchRow>>("/api/batches", admin);
        all.Single(x => x.Id == a.Id).StudentCount.Should().Be(1);
        all.Single(x => x.Id == b.Id).StudentCount.Should().Be(0);
    }

    [Theory]
    [InlineData("12:00", "11:00")]
    [InlineData("25:00", "26:00")]
    [InlineData("11:00", null)]
    public async Task BatchTiming_IsValidated(string? start, string? end)
    {
        var admin = await DemoLoginAsync("admin@sseuropa.demo");
        var res = await RawAsync(HttpMethod.Post, "/api/batches", admin, new { name = $"T {Guid.NewGuid():N}", level = "A1", startTime = start, endTime = end });
        res.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // ── helpers ─────────────────────────────────────────────────────────────

    private Task<BatchRow> NewBatchAsync(string admin, string label, string? start, string? end, params Guid[] courseIds) =>
        SendAsync<BatchRow>(HttpMethod.Post, "/api/batches", admin,
            new { name = $"{label} {Guid.NewGuid():N}"[..20], level = "A1", courseIds, startTime = start, endTime = end });

    private Task<ClassRow> ScheduleAsync(string admin, Guid batchId, string title) =>
        SendAsync<ClassRow>(HttpMethod.Post, "/api/classes", admin,
            new { batchId, title, scheduledAtUtc = DateTime.UtcNow.AddDays(1), durationMinutes = 60 });

    private async Task<List<Guid>> ClassIdsAsync(string token) =>
        (await GetAsync<List<ClassRow>>("/api/classes", token)).Select(c => c.Id).ToList();

    private async Task<List<string>> BatchNamesAsync(string admin, Guid studentId) =>
        (await GetAsync<List<StudentRow>>("/api/students", admin)).Single(s => s.Id == studentId).Batches!.Select(b => b.Name).ToList();

    private async Task<List<string>> NotificationTitlesAsync(string token) =>
        (await GetAsync<List<JsonElement>>("/api/notifications", token)).Select(n => n.GetProperty("title").GetString() ?? "").ToList();

    private async Task<string> GetRawListAsync(string path, string token)
    {
        var res = await RawAsync(HttpMethod.Get, path, token);
        res.StatusCode.Should().Be(HttpStatusCode.OK);
        return await res.Content.ReadAsStringAsync();
    }

    private async Task<(Guid Id, string Token)> CreateStudentAsync(string adminToken, params Guid[] batchIds)
    {
        var email = $"s-{Guid.NewGuid():N}@example.com";
        var created = await SendAsync<JsonElement>(HttpMethod.Post, "/api/students", adminToken, new { firstName = "New", lastName = "Student", email, batchIds });
        var id = created.GetProperty("student").GetProperty("id").GetGuid();
        var res = await _client.PostAsJsonAsync("/api/auth/login", new { email, password = created.GetProperty("temporaryPassword").GetString(), deviceId = $"t-{email}", deviceName = "Test" });
        res.StatusCode.Should().Be(HttpStatusCode.OK);
        return (id, (await res.Content.ReadFromJsonAsync<Envelope<AuthData>>(JsonOptions))!.Data!.AccessToken);
    }

    private async Task<string> DemoLoginAsync(string email)
    {
        var res = await _client.PostAsJsonAsync("/api/auth/demo-login", new { email, deviceId = $"t-{email}", deviceName = "Test" });
        res.StatusCode.Should().Be(HttpStatusCode.OK);
        return (await res.Content.ReadFromJsonAsync<Envelope<AuthData>>(JsonOptions))!.Data!.AccessToken;
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
    private sealed record AuthData(string AccessToken);
    private sealed record CourseInfo(Guid Id, string Title, string? Slug);
    private sealed record CourseCard(CourseInfo Course, bool IsEnrolled);
    private sealed record BatchRow(Guid Id, string Name, int StudentCount, string? StartTime, string? EndTime);
    private sealed record BatchRef(Guid Id, string Name);
    private sealed record StudentRow(Guid Id, string? BatchName, List<BatchRef>? Batches);
    private sealed record ClassRow(Guid Id, string Title, Guid? BatchId, string? BatchName, int BatchStudentCount);
}
