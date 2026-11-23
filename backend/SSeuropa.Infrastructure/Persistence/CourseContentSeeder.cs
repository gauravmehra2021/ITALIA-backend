using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using SSeuropa.Application.Interfaces;
using SSeuropa.Domain.Entities;
using SSeuropa.Domain.Enums;

namespace SSeuropa.Infrastructure.Persistence;

/// <summary>
/// Runs on startup, after the database initializer:
///  1. Removes the old demo/test courses (and everything linked to them) that existed
///     before the new curriculum, once and for good.
///  2. Loads the built-in courses (Seed/*.json, produced by ITALIA-backend/content)
///     if a course with the same slug doesn't exist yet. Existing courses are never
///     overwritten, so edits made in the admin panel are kept.
/// </summary>
public sealed class CourseContentSeeder(IServiceScopeFactory scopes, ILogger<CourseContentSeeder> logger) : IHostedService
{
    /// <summary>The old built-in demo courses (removed on request, 17 Nov 2026). Always removed, including when demo data is re-seeded.</summary>
    private static readonly HashSet<string> LegacyDemoTitles = new(StringComparer.OrdinalIgnoreCase)
    {
        "Italian Essentials A1",
        "Conversational Italian B1",
        "Advanced Grammar Studio C1",
    };

    /// <summary>Test drafts created by hand on the live server. Only those created before the cutoff are removed.</summary>
    private static readonly HashSet<string> LegacyDraftTitles = new(StringComparer.OrdinalIgnoreCase) { "test", "conversation" };

    private static readonly DateTime LegacyCutoffUtc = new(2026, 11, 18, 0, 0, 0, DateTimeKind.Utc);

    private static bool IsLegacy(Course c) =>
        LegacyDemoTitles.Contains(c.Title.Trim())
        || (LegacyDraftTitles.Contains(c.Title.Trim()) && c.CreatedAtUtc < LegacyCutoffUtc);

    private static readonly JsonSerializerOptions Json = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() },
    };

    public async Task StartAsync(CancellationToken ct)
    {
        try
        {
            using var scope = scopes.CreateScope();
            var sp = scope.ServiceProvider;
            await RemoveLegacyCoursesAsync(sp, ct);
            await SeedCoursesAsync(sp, ct);
            await SeedTranslationsAsync(sp, ct);
        }
        catch (Exception ex)
        {
            // Never block the API from starting because of content seeding.
            logger.LogError(ex, "Course content seeding failed.");
        }
    }

    public Task StopAsync(CancellationToken ct) => Task.CompletedTask;

    private async Task RemoveLegacyCoursesAsync(IServiceProvider sp, CancellationToken ct)
    {
        var courses = sp.GetRequiredService<ICourseRepository>();
        var legacy = (await courses.GetAllAsync(ct))
            .Where(IsLegacy)
            .ToList();
        if (legacy.Count == 0) return;

        var ids = legacy.Select(c => c.Id).ToHashSet();
        var mongo = sp.GetService<MongoDbContext>();
        var memory = sp.GetService<InMemoryStore>();

        if (mongo is not null)
        {
            var assessmentIds = await mongo.Assessments.Find(a => ids.Contains(a.CourseId)).Project(a => a.Id).ToListAsync(ct);
            var classIds = await mongo.Classes.Find(c => ids.Contains(c.CourseId)).Project(c => c.Id).ToListAsync(ct);
            var assignmentIds = await mongo.Assignments.Find(a => ids.Contains(a.CourseId)).Project(a => a.Id).ToListAsync(ct);

            await mongo.Attempts.DeleteManyAsync(a => assessmentIds.Contains(a.AssessmentId), ct);
            await mongo.Attendance.DeleteManyAsync(a => classIds.Contains(a.ClassSessionId), ct);
            await mongo.AssignmentSubmissions.DeleteManyAsync(s => assignmentIds.Contains(s.AssignmentId), ct);
            await mongo.Assessments.DeleteManyAsync(a => ids.Contains(a.CourseId), ct);
            await mongo.Classes.DeleteManyAsync(c => ids.Contains(c.CourseId), ct);
            await mongo.Assignments.DeleteManyAsync(a => ids.Contains(a.CourseId), ct);
            await mongo.Materials.DeleteManyAsync(m => ids.Contains(m.CourseId), ct);
            await mongo.LearningResources.DeleteManyAsync(r => ids.Contains(r.CourseId), ct);
            await mongo.Enrollments.DeleteManyAsync(e => ids.Contains(e.CourseId), ct);
            await mongo.Progress.DeleteManyAsync(p => ids.Contains(p.CourseId), ct);
            await mongo.Subscriptions.DeleteManyAsync(s => ids.Contains(s.CourseId), ct);
            await mongo.Certificates.DeleteManyAsync(c => ids.Contains(c.CourseId), ct);
            await mongo.LessonProgress.DeleteManyAsync(p => ids.Contains(p.CourseId), ct);
            await mongo.Lessons.DeleteManyAsync(l => ids.Contains(l.CourseId), ct);
            await mongo.CourseUnits.DeleteManyAsync(u => ids.Contains(u.CourseId), ct);
            await mongo.Courses.DeleteManyAsync(c => ids.Contains(c.Id), ct);
        }
        else if (memory is not null)
        {
            var assessmentIds = memory.Assessments.Where(a => ids.Contains(a.CourseId)).Select(a => a.Id).ToHashSet();
            var classIds = memory.Classes.Where(c => ids.Contains(c.CourseId)).Select(c => c.Id).ToHashSet();
            var assignmentIds = memory.Assignments.Where(a => ids.Contains(a.CourseId)).Select(a => a.Id).ToHashSet();

            memory.Attempts.RemoveAll(a => assessmentIds.Contains(a.AssessmentId));
            memory.Attendance.RemoveAll(a => classIds.Contains(a.ClassSessionId));
            memory.AssignmentSubmissions.RemoveAll(s => assignmentIds.Contains(s.AssignmentId));
            memory.Assessments.RemoveAll(a => ids.Contains(a.CourseId));
            memory.Classes.RemoveAll(c => ids.Contains(c.CourseId));
            memory.Assignments.RemoveAll(a => ids.Contains(a.CourseId));
            memory.Materials.RemoveAll(m => ids.Contains(m.CourseId));
            memory.LearningResources.RemoveAll(r => ids.Contains(r.CourseId));
            memory.Enrollments.RemoveAll(e => ids.Contains(e.CourseId));
            memory.Progress.RemoveAll(p => ids.Contains(p.CourseId));
            memory.Subscriptions.RemoveAll(s => ids.Contains(s.CourseId));
            memory.Certificates.RemoveAll(c => ids.Contains(c.CourseId));
            memory.LessonProgress.RemoveAll(p => ids.Contains(p.CourseId));
            memory.Lessons.RemoveAll(l => ids.Contains(l.CourseId));
            memory.CourseUnits.RemoveAll(u => ids.Contains(u.CourseId));
            memory.Courses.RemoveAll(c => ids.Contains(c.Id));
        }
        else
        {
            return;
        }

        logger.LogWarning("Removed {Count} legacy demo/test courses: {Titles}", legacy.Count, string.Join(", ", legacy.Select(c => c.Title)));
    }

    private async Task SeedCoursesAsync(IServiceProvider sp, CancellationToken ct)
    {
        var courses = sp.GetRequiredService<ICourseRepository>();
        var curriculum = sp.GetRequiredService<ICurriculumRepository>();
        var existing = await courses.GetAllAsync(ct);

        var assembly = typeof(CourseContentSeeder).Assembly;
        foreach (var name in assembly.GetManifestResourceNames().Where(n => n.Contains(".Seed.") && n.EndsWith(".json")))
        {
            await using var stream = assembly.GetManifestResourceStream(name)!;
            var seed = await JsonSerializer.DeserializeAsync<CourseSeed>(stream, Json, ct)
                ?? throw new InvalidOperationException($"Empty course seed {name}");

            var current = existing.FirstOrDefault(c => !c.IsDeleted && string.Equals(c.Slug, seed.Slug, StringComparison.OrdinalIgnoreCase));
            if (current is not null)
            {
                await AppendNewUnitsAsync(current, seed, courses, curriculum, ct);
                continue;
            }

            var course = new Course
            {
                Title = seed.Title,
                Slug = seed.Slug,
                ShortDescription = seed.ShortDescription,
                Description = seed.Description,
                Objectives = seed.Objectives,
                Level = seed.Level,
                CefrLevel = seed.CefrLevel,
                Category = seed.Category,
                Status = CourseStatus.Published,
                Capacity = 9999,
                Language = "Italian",
                Price = 0,
                Currency = "EUR",
                DurationWeeks = seed.DurationWeeks,
                DisplayOrder = seed.DisplayOrder,
                ThumbnailUrl = seed.ThumbnailUrl,
                SeededUnitKeys = seed.Units.Select(u => u.UnitKey).ToList(),
            };
            await courses.AddAsync(course, ct);

            var lessonCount = 0;
            foreach (var u in seed.Units)
                lessonCount += await AddUnitAsync(course.Id, u, u.Order, curriculum, ct);

            logger.LogInformation("Seeded course '{Title}' with {Units} units and {Lessons} lessons.", course.Title, seed.Units.Count, lessonCount);
        }
    }

    /// <summary>
    /// A built-in course already exists: add only the units (chapters) that were added to
    /// the seed since it was loaded. Units already loaded are never touched (admins may have
    /// edited them) and units an admin deleted are not loaded again.
    /// </summary>
    private async Task AppendNewUnitsAsync(Course course, CourseSeed seed, ICourseRepository courses, ICurriculumRepository curriculum, CancellationToken ct)
    {
        if (course.SeededUnitKeys.Count == 0)
        {
            // Loaded before unit keys were recorded: everything in the seed today is already
            // in the course. Record it as the baseline so nothing is added twice.
            course.SeededUnitKeys = seed.Units.Select(u => u.UnitKey).ToList();
            await courses.UpdateAsync(course, ct);
            return;
        }

        var missing = seed.Units.Where(u => !course.SeededUnitKeys.Contains(u.UnitKey, StringComparer.OrdinalIgnoreCase)).ToList();
        if (missing.Count == 0) return;

        var units = await curriculum.GetUnitsAsync(course.Id, ct);
        var nextOrder = units.Count == 0 ? 1 : units.Max(u => u.Order) + 1;
        var lessonCount = 0;
        foreach (var u in missing)
        {
            lessonCount += await AddUnitAsync(course.Id, u, nextOrder++, curriculum, ct);
            course.SeededUnitKeys.Add(u.UnitKey);
        }
        await courses.UpdateAsync(course, ct);
        logger.LogInformation("Added {Units} new units ({Lessons} lessons) to course '{Title}': {Keys}",
            missing.Count, lessonCount, course.Title, string.Join(", ", missing.Select(u => u.UnitKey)));
    }

    private static async Task<int> AddUnitAsync(Guid courseId, UnitSeed u, int order, ICurriculumRepository curriculum, CancellationToken ct)
    {
        var unit = new CourseUnit
        {
            CourseId = courseId,
            Order = order,
            Title = u.Title,
            TitleIt = u.TitleIt,
            Description = u.Description,
            CanDo = u.CanDo,
            ImageUrl = u.ImageUrl,
            PdfUrl = u.PdfUrl,
            LiveClassFocus = u.LiveClassFocus,
            Status = u.Status,
        };
        await curriculum.AddUnitAsync(unit, ct);

        foreach (var l in u.Lessons)
        {
            l.Id = Guid.NewGuid();
            l.CourseId = courseId;
            l.UnitId = unit.Id;
            l.CreatedAtUtc = DateTime.UtcNow;
            await curriculum.AddLessonAsync(l, ct);
        }
        return u.Lessons.Count;
    }

    /// <summary>
    /// Loads the built-in Punjabi / Hindi / Urdu translations (Translations/content-translations.json,
    /// produced by ITALIA-backend/content). New texts are added; for texts already in the
    /// database only empty languages are filled — a translation edited by an admin is never
    /// replaced.
    /// </summary>
    private async Task SeedTranslationsAsync(IServiceProvider sp, CancellationToken ct)
    {
        var assembly = typeof(CourseContentSeeder).Assembly;
        var name = assembly.GetManifestResourceNames().FirstOrDefault(n => n.EndsWith(".Translations.content-translations.json", StringComparison.Ordinal));
        if (name is null) return;

        await using var stream = assembly.GetManifestResourceStream(name)!;
        var seed = await JsonSerializer.DeserializeAsync<List<TranslationSeed>>(stream, Json, ct) ?? [];
        var repo = sp.GetRequiredService<ITranslationRepository>();
        var existing = (await repo.GetAllAsync(ct)).ToDictionary(t => t.Source, StringComparer.Ordinal);

        var added = new List<ContentTranslation>();
        var filled = new List<ContentTranslation>();
        foreach (var s in seed)
        {
            var source = s.En?.Trim();
            if (string.IsNullOrEmpty(source)) continue;
            var pa = string.IsNullOrWhiteSpace(s.Pa) ? null : s.Pa.Trim();
            var hi = string.IsNullOrWhiteSpace(s.Hi) ? null : s.Hi.Trim();
            var ur = string.IsNullOrWhiteSpace(s.Ur) ? null : s.Ur.Trim();

            if (!existing.TryGetValue(source, out var row))
            {
                var item = new ContentTranslation { Source = source, Punjabi = pa, Hindi = hi, Urdu = ur };
                existing[source] = item;
                added.Add(item);
                continue;
            }

            var changed = false;
            if (string.IsNullOrWhiteSpace(row.Punjabi) && pa is not null) { row.Punjabi = pa; changed = true; }
            if (string.IsNullOrWhiteSpace(row.Hindi) && hi is not null) { row.Hindi = hi; changed = true; }
            if (string.IsNullOrWhiteSpace(row.Urdu) && ur is not null) { row.Urdu = ur; changed = true; }
            if (changed) filled.Add(row);
        }

        await repo.AddRangeAsync(added, ct);
        if (filled.Count > 0) await repo.UpdateRangeAsync(filled, ct);
        if (added.Count > 0 || filled.Count > 0)
            logger.LogInformation("Content translations: {Added} added, {Filled} filled in.", added.Count, filled.Count);
    }

    private sealed class TranslationSeed
    {
        public string? En { get; set; }
        public string? Pa { get; set; }
        public string? Hi { get; set; }
        public string? Ur { get; set; }
    }

    private sealed class CourseSeed
    {
        public string Slug { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string ShortDescription { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Objectives { get; set; } = string.Empty;
        public CourseLevel Level { get; set; }
        public string CefrLevel { get; set; } = "A1";
        public string Category { get; set; } = "Core";
        public int DurationWeeks { get; set; }
        public int DisplayOrder { get; set; }
        public string? ThumbnailUrl { get; set; }
        public List<UnitSeed> Units { get; set; } = [];
    }

    private sealed class UnitSeed
    {
        /// <summary>Stable id of the unit inside its course (e.g. "capitolo-1"). Defaults to "unit-{Order}".</summary>
        public string? Key { get; set; }
        public string UnitKey => string.IsNullOrWhiteSpace(Key) ? $"unit-{Order}" : Key;
        public int Order { get; set; }
        public string Title { get; set; } = string.Empty;
        public string TitleIt { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<string> CanDo { get; set; } = [];
        public string? ImageUrl { get; set; }
        public string? PdfUrl { get; set; }
        public string? LiveClassFocus { get; set; }
        public LessonStatus Status { get; set; }
        public List<Lesson> Lessons { get; set; } = [];
    }
}
