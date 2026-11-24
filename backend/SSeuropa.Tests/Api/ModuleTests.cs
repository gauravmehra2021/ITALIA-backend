using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;

namespace SSeuropa.Tests.Api;

/// <summary>
/// Unit → Module → Lesson. Every module starts with its video lesson; the next module opens only
/// when every lesson of the previous one is completed. A module video without a video yet shows
/// "coming soon" and never blocks students.
/// </summary>
public class ModuleTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() },
    };

    public ModuleTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.WithWebHostBuilder(b => b.UseSetting("Database:Provider", "InMemory")).CreateClient();
    }

    [Fact]
    public async Task SchoolCourse_IsGroupedIntoLezioniModules_EachStartingWithAVideo()
    {
        var admin = await DemoLoginAsync("admin@sseuropa.demo");
        var course = (await GetAsync<List<CourseCard>>("/api/learn/courses", admin)).Single(c => c.Course.Slug == "corso-italiano-a0");
        var outline = await GetAsync<Outline>($"/api/curriculum/courses/{course.Course.Id}", admin);

        // Placeholder videos don't count as lessons.
        course.TotalLessons.Should().Be(156);

        var unit = outline.Units[0];
        unit.Modules.Select(m => m.Title).Should().StartWith(["Lezione 1", "Lezione 2"]);
        unit.Modules[^1].Title.Should().Be("Chapter 1 test");
        // Lesson modules start with a video; the chapter test is a test module without one.
        unit.Modules.Where(m => !m.IsTest).Should().OnlyContain(m => m.Lessons[0].IsModuleVideo && m.Lessons[0].VideoPending);
        unit.Modules[^1].IsTest.Should().BeTrue();
        unit.Modules[^1].Lessons.Should().ContainSingle().Which.Kind.Should().Be("Exam");
        unit.Modules[0].Lessons.Count(l => !l.IsModuleVideo).Should().Be(3);

        // Lesson numbers run through the whole unit, module after module.
        unit.Lessons.Select(l => l.Order).Should().Equal(Enumerable.Range(1, unit.Lessons.Count));
        unit.Modules.SelectMany(m => m.Lessons).Select(l => l.Id).Should().Equal(unit.Lessons.Select(l => l.Id));

        // Every chapter has Lezione 1–4 and ends with its test (Chapter 7: final A1, Chapter 12: final A2).
        outline.Units.Should().HaveCount(12).And.AllSatisfy(u =>
        {
            u.Modules.Take(4).Select(m => m.Title).Should().Equal("Lezione 1", "Lezione 2", "Lezione 3", "Lezione 4");
            u.Modules.Should().HaveCount(5);
            u.Modules[^1].IsTest.Should().BeTrue();
        });
        outline.Units[6].Modules[^1].Title.Should().Be("Final A1 test (Chapters 1–7)");
        outline.Units[11].Modules[^1].Title.Should().Be("Final A2 test (Chapters 8–12)");
    }

    [Fact]
    public async Task Modules_OpenOneAfterAnother_AndPendingVideosNeverBlock()
    {
        var teacher = await DemoLoginAsync("mentor@sseuropa.demo");
        var (courseId, unit) = await BuildCourseAsync(teacher);
        var m1 = unit.Modules[0];
        var m2 = await SendAsync<ModuleData>(HttpMethod.Post, $"/api/curriculum/units/{unit.Id}/modules", teacher, new { title = "Introducing yourself" });
        m2.Lessons.Should().ContainSingle().Which.IsModuleVideo.Should().BeTrue();
        var a = await AddLessonAsync(teacher, m1.Id, "Ciao");
        var b = await AddLessonAsync(teacher, m1.Id, "Grazie");
        var c = await AddLessonAsync(teacher, m2.Id, "Mi chiamo");

        var student = await StudentInCourseAsync(courseId);
        var outline = await GetAsync<Outline>($"/api/learn/courses/{courseId}", student);
        var mods = outline.Units[0].Modules;
        mods[0].Locked.Should().BeFalse();
        mods[1].Locked.Should().BeTrue();
        mods[0].TotalLessons.Should().Be(2); // the video isn't ready, so it doesn't count
        outline.TotalLessons.Should().Be(3);

        // Lesson 2 of module 2 can't be opened before module 1 is done (checked by the server too).
        (await RawAsync(HttpMethod.Get, $"/api/learn/lessons/{c}", student)).StatusCode.Should().Be(HttpStatusCode.Forbidden);
        // The pending video can't be "completed".
        (await RawAsync(HttpMethod.Post, $"/api/learn/lessons/{mods[0].Lessons[0].Id}/complete", student, new { scorePercent = 100 }))
            .StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var r1 = await SendAsync<Result>(HttpMethod.Post, $"/api/learn/lessons/{a}/complete", student, new { scorePercent = 100 });
        r1.ModuleCompleted.Should().BeFalse();
        var r2 = await SendAsync<Result>(HttpMethod.Post, $"/api/learn/lessons/{b}/complete", student, new { scorePercent = 100 });
        r2.ModuleCompleted.Should().BeTrue();
        r2.NextLessonId.Should().Be(c); // the module-2 video is skipped while it has no video

        var after = await GetAsync<Outline>($"/api/learn/courses/{courseId}", student);
        after.Units[0].Modules[0].Completed.Should().BeTrue();
        after.Units[0].Modules[1].Locked.Should().BeFalse();
        (await RawAsync(HttpMethod.Get, $"/api/learn/lessons/{c}", student)).StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task ModuleVideo_StaysFirst_CantBeDeleted_AndCountsOnceAVideoIsAdded()
    {
        var teacher = await DemoLoginAsync("mentor@sseuropa.demo");
        var (courseId, unit) = await BuildCourseAsync(teacher);
        var module = unit.Modules[0];
        var videoId = module.Lessons[0].Id;
        var lesson = await AddLessonAsync(teacher, module.Id, "Ciao");

        (await RawAsync(HttpMethod.Delete, $"/api/curriculum/lessons/{videoId}", teacher)).StatusCode.Should().Be(HttpStatusCode.BadRequest);
        (await RawAsync(HttpMethod.Post, $"/api/curriculum/lessons/{lesson}/move", teacher, new { direction = -1 })).StatusCode.Should().Be(HttpStatusCode.OK);
        var outline = await GetAsync<Outline>($"/api/curriculum/courses/{courseId}", teacher);
        outline.Units[0].Modules[0].Lessons[0].Id.Should().Be(videoId);

        // Renaming the module renames its video.
        var renamed = await SendAsync<ModuleData>(HttpMethod.Put, $"/api/curriculum/modules/{module.Id}", teacher, new { title = "Italian basics" });
        renamed.Lessons[0].Title.Should().Be("Video: Italian basics");

        // Add the video: it is now a real lesson students must watch first.
        await SendAsync<JsonElement>(HttpMethod.Put, $"/api/curriculum/lessons/{videoId}", teacher, new
        {
            title = "Video: Italian basics",
            kind = "Vocabulary", // ignored: the module video is always a video lesson
            status = "Published",
            blocks = new[] { new { type = "Video", mediaUrl = "https://www.youtube.com/watch?v=abc123" } },
        });
        var student = await StudentInCourseAsync(courseId);
        var learn = await GetAsync<Outline>($"/api/learn/courses/{courseId}", student);
        learn.TotalLessons.Should().Be(2);
        var first = learn.Units[0].Modules[0].Lessons;
        first[0].VideoPending.Should().BeFalse();
        first[0].Kind.Should().Be("Video");
        first[1].Locked.Should().BeTrue();
        learn.Units[0].Modules[0].VideoReady.Should().BeTrue();

        // Deleting the module removes its lessons.
        (await RawAsync(HttpMethod.Delete, $"/api/curriculum/modules/{module.Id}", teacher)).StatusCode.Should().Be(HttpStatusCode.OK);
        (await GetAsync<Outline>($"/api/curriculum/courses/{courseId}", teacher)).Units[0].Lessons.Should().BeEmpty();
    }

    [Fact]
    public async Task TestModule_HasNoVideo_AndCanBeSwitched()
    {
        var teacher = await DemoLoginAsync("mentor@sseuropa.demo");
        var (_, unit) = await BuildCourseAsync(teacher);
        var test = await SendAsync<ModuleData>(HttpMethod.Post, $"/api/curriculum/units/{unit.Id}/modules", teacher, new { title = "Unit test", isTest = true });
        test.IsTest.Should().BeTrue();
        test.Lessons.Should().BeEmpty();

        var normal = await SendAsync<ModuleData>(HttpMethod.Put, $"/api/curriculum/modules/{test.Id}", teacher, new { title = "Unit test", isTest = false });
        normal.Lessons.Should().ContainSingle().Which.IsModuleVideo.Should().BeTrue();

        var back = await SendAsync<ModuleData>(HttpMethod.Put, $"/api/curriculum/modules/{test.Id}", teacher, new { title = "Unit test", isTest = true });
        back.Lessons.Should().BeEmpty();
    }

    // ── helpers ─────────────────────────────────────────────────────────────

    private async Task<(Guid CourseId, UnitData Unit)> BuildCourseAsync(string teacher)
    {
        var course = await SendAsync<CourseInfo>(HttpMethod.Post, "/api/courses", teacher, new
        {
            title = $"Module test {Guid.NewGuid():N}",
            description = "Built in a test",
            level = "Beginner",
            status = "Published",
        });
        var unit = await SendAsync<UnitData>(HttpMethod.Post, $"/api/curriculum/courses/{course.Id}/units", teacher, new { title = "Unit A", status = "Published" });
        unit.Modules.Should().ContainSingle();
        unit.Modules[0].Lessons.Should().ContainSingle().Which.IsModuleVideo.Should().BeTrue();
        return (course.Id, unit);
    }

    private async Task<Guid> AddLessonAsync(string teacher, Guid moduleId, string title)
    {
        var lesson = await SendAsync<LessonInfo>(HttpMethod.Post, $"/api/curriculum/modules/{moduleId}/lessons", teacher, new
        {
            title,
            kind = "Vocabulary",
            status = "Published",
            exercises = new[]
            {
                new { type = "PickTranslation", prompt = "?", italian = title, options = new[] { new { text = "a" }, new { text = "b" } }, correctIndex = 0 },
            },
        });
        lesson.ModuleId.Should().Be(moduleId);
        return lesson.Id;
    }

    /// <summary>A new student in a batch that has the course.</summary>
    private async Task<string> StudentInCourseAsync(Guid courseId)
    {
        var admin = await DemoLoginAsync("admin@sseuropa.demo");
        var batch = await SendAsync<JsonElement>(HttpMethod.Post, "/api/batches", admin, new { name = $"Batch {Guid.NewGuid():N}", level = "A0", courseIds = new[] { courseId } });
        var email = $"m-{Guid.NewGuid():N}@example.com";
        var created = await SendAsync<JsonElement>(HttpMethod.Post, "/api/students", admin, new { firstName = "Mod", lastName = "Student", email, batchId = batch.GetProperty("id").GetGuid() });
        var res = await _client.PostAsJsonAsync("/api/auth/login", new { email, password = created.GetProperty("temporaryPassword").GetString(), deviceId = $"t-{email}", deviceName = "Test" });
        res.StatusCode.Should().Be(HttpStatusCode.OK);
        return (await res.Content.ReadFromJsonAsync<Envelope<AuthData>>(JsonOptions))!.Data!.AccessToken;
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
    private sealed record CourseCard(CourseInfo Course, int TotalLessons);
    private sealed record LessonSummary(Guid Id, int Order, string Title, string Kind, bool IsModuleVideo, bool VideoPending, bool? Locked);
    private sealed record ModuleData(Guid Id, int Order, string Title, List<LessonSummary> Lessons, int TotalLessons, bool? Locked, bool? Completed, bool VideoReady, bool IsTest);
    private sealed record UnitData(Guid Id, int Order, string Title, List<LessonSummary> Lessons, List<ModuleData> Modules);
    private sealed record Outline(List<UnitData> Units, int TotalLessons);
    private sealed record LessonInfo(Guid Id, Guid? ModuleId);
    private sealed record Result(bool Completed, bool ModuleCompleted, Guid? NextLessonId);
}
