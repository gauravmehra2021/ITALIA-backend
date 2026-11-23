using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;

namespace SSeuropa.Tests.Api;

/// <summary>Built-in A1 course, student learning flow and the staff course builder.</summary>
public class CurriculumTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() },
    };

    public CurriculumTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.WithWebHostBuilder(b => b.UseSetting("Database:Provider", "InMemory")).CreateClient();
    }

    [Fact]
    public async Task BuiltInA1Course_IsSeeded_AndOldDemoCoursesAreGone()
    {
        // Students only see the courses of their batch.
        await PlaceInBatchWithAllCoursesAsync("priya@sseuropa.demo");
        var student = await LoginAsync("priya@sseuropa.demo");
        var catalogue = await GetAsync<List<CourseCard>>("/api/learn/courses", student);

        catalogue.Should().Contain(c => c.Course.Slug == "italian-a1-foundations" && c.TotalLessons == 40);
        catalogue.Should().NotContain(c => c.Course.Title == "Italian Essentials A1");
    }

    [Fact]
    public async Task Student_EnrollsAndLearns_LessonsUnlockInOrder()
    {
        await PlaceInBatchWithAllCoursesAsync("priya@sseuropa.demo");
        var student = await LoginAsync("priya@sseuropa.demo");
        var course = (await GetAsync<List<CourseCard>>("/api/learn/courses", student)).Single(c => c.Course.Slug == "italian-a1-foundations");

        var enrolled = await SendAsync<Outline>(HttpMethod.Post, $"/api/learn/courses/{course.Course.Id}/enroll", student);
        enrolled.IsEnrolled.Should().BeTrue();
        enrolled.Units.Should().HaveCount(10);

        var first = enrolled.Units[0].Lessons[0];
        var second = enrolled.Units[0].Lessons[1];
        first.Locked.Should().BeFalse();
        second.Locked.Should().BeTrue();

        // Locked lesson can't be opened yet.
        (await RawAsync(HttpMethod.Get, $"/api/learn/lessons/{second.Id}", student)).StatusCode.Should().Be(HttpStatusCode.Forbidden);

        var lesson = await GetAsync<LessonData>($"/api/learn/lessons/{first.Id}", student);
        lesson.Exercises.Should().NotBeEmpty();
        lesson.Blocks.Should().Contain(b => b.Type == "Vocabulary");

        var result = await SendAsync<LessonResult>(HttpMethod.Post, $"/api/learn/lessons/{first.Id}/complete", student, new { scorePercent = 95 });
        result.Stars.Should().Be(3);
        result.Completed.Should().BeTrue();
        result.XpGained.Should().BeGreaterThan(0);
        result.NextLessonId.Should().Be(second.Id);
        result.StreakDays.Should().BeGreaterThanOrEqualTo(1);

        // Next lesson is now open, and replaying with a lower score keeps the best result.
        (await RawAsync(HttpMethod.Get, $"/api/learn/lessons/{second.Id}", student)).StatusCode.Should().Be(HttpStatusCode.OK);
        var replay = await SendAsync<LessonResult>(HttpMethod.Post, $"/api/learn/lessons/{first.Id}/complete", student, new { scorePercent = 40 });
        replay.Stars.Should().Be(0);
        replay.XpGained.Should().Be(0);

        var outline = await GetAsync<Outline>($"/api/learn/courses/{course.Course.Id}", student);
        outline.CompletedLessons.Should().Be(1);
        outline.Units[0].Lessons[0].Stars.Should().Be(3);

        var me = await GetAsync<LearnerSummary>("/api/learn/me", student);
        me.TotalXp.Should().BeGreaterThan(0);
        me.WordsLearned.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task NotEnrolled_CanOpenFreePreview_ButNotOtherLessons()
    {
        // A brand-new student without a batch; the course is open to every student (like a free demo course).
        var admin = await LoginAsync("admin@sseuropa.demo");
        var a1 = (await GetAsync<List<CourseCard>>("/api/learn/courses", admin)).Single(c => c.Course.Slug == "italian-a1-foundations");
        await SetOpenToAllAsync(a1.Course.Id, true);
        var email = $"preview-{Guid.NewGuid():N}@example.com";
        var created = await SendAsync<JsonElement>(HttpMethod.Post, "/api/students", admin, new { firstName = "Preview", lastName = "Student", email });
        var student = await PasswordLoginAsync(email, created.GetProperty("temporaryPassword").GetString()!);

        var course = (await GetAsync<List<CourseCard>>("/api/learn/courses", student)).Single(c => c.Course.Slug == "italian-a1-foundations");
        var outline = await GetAsync<Outline>($"/api/learn/courses/{course.Course.Id}", student);
        outline.IsEnrolled.Should().BeFalse();

        var preview = outline.Units[0].Lessons.Single(l => l.IsFreePreview);
        (await RawAsync(HttpMethod.Get, $"/api/learn/lessons/{preview.Id}", student)).StatusCode.Should().Be(HttpStatusCode.OK);
        (await RawAsync(HttpMethod.Get, $"/api/learn/lessons/{outline.Units[1].Lessons[0].Id}", student)).StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Teacher_BuildsUnitAndLesson_ValidationAndStudentsCantEdit()
    {
        var teacher = await LoginAsync("mentor@sseuropa.demo");
        var courseRes = await SendAsync<CourseInfo>(HttpMethod.Post, "/api/courses", teacher, new
        {
            title = "Test course builder",
            description = "Built in a test",
            level = "Beginner",
            status = "Published",
        });

        var unit = await SendAsync<UnitData>(HttpMethod.Post, $"/api/curriculum/courses/{courseRes.Id}/units", teacher, new { title = "Unit A", status = "Published" });
        unit.Order.Should().Be(1);

        // An exercise without options is rejected.
        var bad = await RawAsync(HttpMethod.Post, $"/api/curriculum/units/{unit.Id}/lessons", teacher, new
        {
            title = "Broken",
            kind = "Vocabulary",
            exercises = new[] { new { type = "PickTranslation", prompt = "?", options = Array.Empty<object>() } },
        });
        bad.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var lesson = await SendAsync<LessonData>(HttpMethod.Post, $"/api/curriculum/units/{unit.Id}/lessons", teacher, new
        {
            title = "Saluti",
            kind = "Vocabulary",
            status = "Published",
            blocks = new[] { new { type = "Vocabulary", items = new[] { new { italian = "ciao", english = "hi" } } } },
            exercises = new[]
            {
                new { type = "PickTranslation", prompt = "What does it mean?", italian = "ciao", options = new[] { new { text = "hi" }, new { text = "bye" } }, correctIndex = 0 },
            },
        });
        lesson.Title.Should().Be("Saluti");

        var outline = await GetAsync<Outline>($"/api/curriculum/courses/{courseRes.Id}", teacher);
        outline.Units.Single().Lessons.Single().ExerciseCount.Should().Be(1);

        var student = await LoginAsync("priya@sseuropa.demo");
        (await RawAsync(HttpMethod.Post, $"/api/curriculum/courses/{courseRes.Id}/units", student, new { title = "Nope" }))
            .StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task CourseMedia_IsServed()
    {
        (await _client.GetAsync("/content/img/waving-hand.png")).StatusCode.Should().Be(HttpStatusCode.OK);
        (await _client.GetAsync("/content/pdf/italian-a1-foundations-unit-01.pdf")).StatusCode.Should().Be(HttpStatusCode.OK);
        (await _client.GetAsync("/content/pdf/corso-italiano-a0-unit-01.pdf")).StatusCode.Should().Be(HttpStatusCode.OK);
        (await _client.GetAsync("/content/slides/corso-a0-cap1-lez1-slides.pdf")).StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Student_SavesPortalLanguage_OnlyEnglishPunjabiHindiUrdu()
    {
        var student = await LoginAsync("priya@sseuropa.demo");

        (await SendAsync<string>(HttpMethod.Put, "/api/learn/me/language", student, new { language = "punjabi" })).Should().Be("Punjabi");
        var again = await _client.PostAsJsonAsync("/api/auth/demo-login", new { email = "priya@sseuropa.demo", deviceId = "t-priya@sseuropa.demo", deviceName = "Test" });
        (await again.Content.ReadAsStringAsync()).Should().Contain("\"preferredLanguage\":\"Punjabi\"");

        (await SendAsync<string>(HttpMethod.Put, "/api/learn/me/language", student, new { language = "Hindi" })).Should().Be("Hindi");
        (await SendAsync<string>(HttpMethod.Put, "/api/learn/me/language", student, new { language = "urdu" })).Should().Be("Urdu");
        (await RawAsync(HttpMethod.Put, "/api/learn/me/language", student, new { language = "Italian" })).StatusCode.Should().Be(HttpStatusCode.BadRequest);
        (await RawAsync(HttpMethod.Put, "/api/learn/me/language", student, new { language = "Klingon" })).StatusCode.Should().Be(HttpStatusCode.BadRequest);

        (await SendAsync<string>(HttpMethod.Put, "/api/learn/me/language", student, new { language = "English" })).Should().Be("English");
    }

    [Fact]
    public async Task DemoStudent_HasEveryLessonOpen_OtherStudentsStillUnlockInOrder()
    {
        await PlaceInBatchWithAllCoursesAsync("student@sseuropa.demo", "priya@sseuropa.demo");
        var demo = await LoginAsync("student@sseuropa.demo");
        var course = (await GetAsync<List<CourseCard>>("/api/learn/courses", demo)).Single(c => c.Course.Slug == "corso-italiano-a0");
        var outline = await SendAsync<Outline>(HttpMethod.Post, $"/api/learn/courses/{course.Course.Id}/enroll", demo);

        outline.Units.SelectMany(u => u.Lessons).Should().OnlyContain(l => l.Locked == false);
        var test = outline.Units[1].Lessons[^1]; // Chapter 2 test, nothing completed before it
        (await RawAsync(HttpMethod.Get, $"/api/learn/lessons/{test.Id}", demo)).StatusCode.Should().Be(HttpStatusCode.OK);

        var other = await LoginAsync("priya@sseuropa.demo");
        var otherOutline = await SendAsync<Outline>(HttpMethod.Post, $"/api/learn/courses/{course.Course.Id}/enroll", other);
        otherOutline.Units[1].Lessons[^1].Locked.Should().BeTrue();
        (await RawAsync(HttpMethod.Get, $"/api/learn/lessons/{test.Id}", other)).StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task SchoolSyllabusCourse_IsSeededOnce_WithPunjabiAndChapterTests()
    {
        var admin = await LoginAsync("admin@sseuropa.demo");
        var catalogue = await GetAsync<List<CourseCard>>("/api/learn/courses", admin);

        // Exactly one copy, with all 5 chapters (4 lessons × 3 parts + a test each).
        var course = catalogue.Should().ContainSingle(c => c.Course.Slug == "corso-italiano-a0").Subject;
        course.TotalLessons.Should().Be(65);

        var outline = await GetAsync<Outline>($"/api/curriculum/courses/{course.Course.Id}", admin);
        outline.Units.Select(u => u.Title).Should().HaveCount(5).And.AllSatisfy(t => t.Should().StartWith("Chapter"));

        // Words carry their Punjabi meaning.
        var first = await SendAsync<JsonElement>(HttpMethod.Get, $"/api/curriculum/lessons/{outline.Units[0].Lessons[0].Id}", admin);
        first.GetRawText().Should().Contain("\"punjabi\":\"Kelā\"");

        // Chapter test: 55% is below the school's 60% pass mark, 65% passes.
        var test = outline.Units[0].Lessons[^1];
        test.Title.Should().Be("Chapter 1 test");
        var failed = await SendAsync<LessonResult>(HttpMethod.Post, $"/api/learn/lessons/{test.Id}/complete", admin, new { scorePercent = 55 });
        failed.Completed.Should().BeFalse();
        failed.Stars.Should().Be(0);
        var passed = await SendAsync<LessonResult>(HttpMethod.Post, $"/api/learn/lessons/{test.Id}/complete", admin, new { scorePercent = 65 });
        passed.Completed.Should().BeTrue();
    }

    // ── helpers ─────────────────────────────────────────────────────────────

    private async Task<string> LoginAsync(string email)
    {
        var res = await _client.PostAsJsonAsync("/api/auth/demo-login", new { email, deviceId = $"t-{email}", deviceName = "Test" });
        res.StatusCode.Should().Be(HttpStatusCode.OK);
        return (await res.Content.ReadFromJsonAsync<Envelope<AuthData>>(JsonOptions))!.Data!.AccessToken;
    }

    private async Task<string> PasswordLoginAsync(string email, string password)
    {
        var res = await _client.PostAsJsonAsync("/api/auth/login", new { email, password, deviceId = $"t-{email}", deviceName = "Test" });
        res.StatusCode.Should().Be(HttpStatusCode.OK);
        return (await res.Content.ReadFromJsonAsync<Envelope<AuthData>>(JsonOptions))!.Data!.AccessToken;
    }

    /// <summary>Students only see their batch's courses: put them in a new batch that has every course.</summary>
    private async Task PlaceInBatchWithAllCoursesAsync(params string[] emails)
    {
        var admin = await LoginAsync("admin@sseuropa.demo");
        var courses = await GetAsync<List<CourseCard>>("/api/learn/courses", admin);
        var batch = await SendAsync<JsonElement>(HttpMethod.Post, "/api/batches", admin, new
        {
            name = $"Test batch {Guid.NewGuid():N}",
            level = "A1",
            courseIds = courses.Select(c => c.Course.Id).ToList()
        });
        var students = await GetAsync<List<StudentRow>>("/api/students", admin);
        var ids = students.Where(s => emails.Contains(s.Email)).Select(s => s.Id).ToList();
        ids.Should().HaveCount(emails.Length);
        await SendAsync<JsonElement>(HttpMethod.Post, "/api/students/move", admin, new { studentIds = ids, batchId = batch.GetProperty("id").GetGuid() });
    }

    private async Task SetOpenToAllAsync(Guid courseId, bool open)
    {
        var admin = await LoginAsync("admin@sseuropa.demo");
        var res = await RawAsync(HttpMethod.Get, $"/api/courses/{courseId}", admin);
        var course = System.Text.Json.Nodes.JsonNode.Parse(await res.Content.ReadAsStringAsync())!["data"]!;
        course["openToAll"] = open;
        var put = await RawAsync(HttpMethod.Put, $"/api/courses/{courseId}", admin, course);
        put.IsSuccessStatusCode.Should().BeTrue(await put.Content.ReadAsStringAsync());
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
    private sealed record StudentRow(Guid Id, string Email);
    private sealed record CourseInfo(Guid Id, string Title, string? Slug);
    private sealed record CourseCard(CourseInfo Course, bool IsEnrolled, int CompletionPercent, int TotalLessons);
    private sealed record LessonSummary(Guid Id, string Title, bool IsFreePreview, int ExerciseCount, bool? Locked, int? Stars);
    private sealed record UnitData(Guid Id, int Order, string Title, List<LessonSummary> Lessons);
    private sealed record Outline(CourseInfo Course, List<UnitData> Units, int TotalLessons, int CompletedLessons, bool IsEnrolled);
    private sealed record Block(string Type);
    private sealed record LessonData(Guid Id, string Title, List<Block> Blocks, List<object> Exercises);
    private sealed record LessonResult(int Stars, bool Completed, int XpGained, Guid? NextLessonId, int StreakDays);
    private sealed record LearnerSummary(int TotalXp, int StreakDays, int WordsLearned);
}
