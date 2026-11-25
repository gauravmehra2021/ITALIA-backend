using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using SSeuropa.Application.Interfaces;
using SSeuropa.Application.Services;
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
            var seeds = await LoadSeedsAsync(ct);
            await SplitCoursesAsync(sp, seeds, ct);
            await SeedCoursesAsync(sp, seeds, ct);
            await EnsureModulesAsync(sp, ct);
            await RefreshBuiltInContentAsync(sp, seeds, ct);
            await SeedTranslationsAsync(sp, ct);
        }
        catch (Exception ex)
        {
            // Never block the API from starting because of content seeding.
            logger.LogError(ex, "Course content seeding failed.");
        }
    }

    public Task StopAsync(CancellationToken ct) => Task.CompletedTask;

    /// <summary>
    /// Every course follows Unit → Module → Lesson, each module starting with its video lesson.
    /// Content made before modules existed is grouped into modules once; nothing is deleted.
    /// </summary>
    private async Task EnsureModulesAsync(IServiceProvider sp, CancellationToken ct)
    {
        var courses = sp.GetRequiredService<ICourseRepository>();
        var curriculum = sp.GetRequiredService<ICurriculumRepository>();
        foreach (var course in (await courses.GetAllAsync(ct)).Where(c => !c.IsDeleted))
        {
            var changes = await SSeuropa.Application.Services.ModuleStructure.EnsureCourseAsync(curriculum, course.Id, ct);
            if (changes > 0)
                logger.LogInformation("Organised course '{Title}' into modules ({Changes} changes).", course.Title, changes);
        }
    }

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
            await mongo.CourseModules.DeleteManyAsync(m => ids.Contains(m.CourseId), ct);
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
            memory.CourseModules.RemoveAll(m => ids.Contains(m.CourseId));
            memory.CourseUnits.RemoveAll(u => ids.Contains(u.CourseId));
            memory.Courses.RemoveAll(c => ids.Contains(c.Id));
        }
        else
        {
            return;
        }

        logger.LogWarning("Removed {Count} legacy demo/test courses: {Titles}", legacy.Count, string.Join(", ", legacy.Select(c => c.Title)));
    }

    /// <summary>
    /// A built-in course that was split into smaller ones (25 Nov 2026: "Corso di Italiano", 12 chapters /
    /// 156 lessons → one course per Capitolo; the new seeds name the old slug in <c>splitFrom</c>).
    /// Runs while the old course still exists: each of its chapters moves into its new course with its
    /// modules, lessons (same ids, so admin edits and student progress are kept) and progress records.
    /// Students enrolled in the old course are enrolled in every new course and batches that had the old
    /// course get all the new ones. When nothing is left in the old course it is removed and its live
    /// classes, tests, assignments and materials go to the first new course. A chapter the old course never
    /// had is simply loaded fresh by <see cref="SeedCoursesAsync"/>.
    /// </summary>
    private async Task SplitCoursesAsync(IServiceProvider sp, IReadOnlyList<CourseSeed> seeds, CancellationToken ct)
    {
        var courses = sp.GetRequiredService<ICourseRepository>();
        var curriculum = sp.GetRequiredService<ICurriculumRepository>();
        var enrollments = sp.GetRequiredService<IEnrollmentRepository>();
        var batches = sp.GetRequiredService<IBatchRepository>();
        var mongo = sp.GetService<MongoDbContext>();
        var memory = sp.GetService<InMemoryStore>();

        foreach (var group in seeds.Where(s => !string.IsNullOrWhiteSpace(s.SplitFrom)).GroupBy(s => s.SplitFrom!, StringComparer.OrdinalIgnoreCase))
        {
            var all = await courses.GetAllAsync(ct);
            var old = all.FirstOrDefault(c => !c.IsDeleted && string.Equals(c.Slug, group.Key, StringComparison.OrdinalIgnoreCase));
            if (old is null) continue;

            var oldUnits = (await curriculum.GetUnitsAsync(old.Id, ct)).OrderBy(u => u.Order).ToList();
            var taken = new HashSet<Guid>();
            var created = new List<Course>();
            var targets = new List<Course>();
            foreach (var seed in group.OrderBy(s => s.DisplayOrder))
            {
                var already = all.FirstOrDefault(c => !c.IsDeleted && string.Equals(c.Slug, seed.Slug, StringComparison.OrdinalIgnoreCase));
                if (already is not null) { targets.Add(already); continue; }

                var matched = seed.Units
                    .Select(su => (Seed: su, Unit: FindSplitUnit(oldUnits, old.SeededUnitKeys, su, taken)))
                    .Where(x => x.Unit is not null)
                    .ToList();

                var course = NewCourse(seed);
                // Settings an admin may have set on the old course carry over.
                course.Status = old.Status;
                course.MentorId = old.MentorId;
                course.Capacity = old.Capacity;
                course.Price = old.Price;
                course.Currency = old.Currency;
                course.PaymentUrl = old.PaymentUrl;
                course.OpenToAll = old.OpenToAll;
                course.EnrolledCount = old.EnrolledCount;
                await courses.AddAsync(course, ct);

                // Each unit is moved from the old course; one the old course never loaded (a chapter
                // added since) is loaded from the seed. One an admin deleted there stays deleted.
                var order = 1;
                foreach (var su in seed.Units)
                {
                    var unit = matched.FirstOrDefault(m => m.Seed == su).Unit;
                    if (unit is not null)
                        await MoveUnitAsync(unit, course.Id, order++, curriculum, mongo, memory, ct);
                    else if (!old.SeededUnitKeys.Contains(su.UnitKey, StringComparer.OrdinalIgnoreCase))
                        await AddUnitAsync(course.Id, su, order++, curriculum, ct);
                }
                created.Add(course);
                targets.Add(course);
            }
            if (targets.Count == 0) continue;

            // Students of the old course keep access to every chapter.
            var oldEnrollments = (await enrollments.GetAllAsync(ct)).Where(e => e.CourseId == old.Id && !e.IsDeleted).ToList();
            foreach (var course in targets)
                foreach (var e in oldEnrollments)
                    if (await enrollments.GetByUserAndCourseAsync(e.UserId, course.Id, ct) is null)
                        await enrollments.AddAsync(new Enrollment
                        {
                            UserId = e.UserId,
                            CourseId = course.Id,
                            Status = e.Status,
                            ActivatedAtUtc = e.ActivatedAtUtc,
                            SubscriptionId = e.SubscriptionId,
                        }, ct);

            var leftover = (await curriculum.GetUnitsAsync(old.Id, ct)).Count;
            var removeOld = leftover == 0;

            // Batches: the new courses take the old course's place, in chapter order.
            foreach (var batch in (await batches.GetAllAsync(ct)).Where(b => b.CourseIds.Contains(old.Id)))
            {
                var at = batch.CourseIds.IndexOf(old.Id);
                var add = targets.Select(c => c.Id).Where(id => !batch.CourseIds.Contains(id)).ToList();
                batch.CourseIds.InsertRange(at + 1, add);
                if (removeOld) batch.CourseIds.Remove(old.Id);
                await batches.UpdateAsync(batch, ct);
            }

            if (removeOld)
            {
                await ReassignCourseAsync(mongo, memory, old.Id, targets[0].Id, ct);
                await courses.DeleteAsync(old.Id, ct);
            }
            else
            {
                logger.LogWarning("Course '{Title}' still has {Count} units that belong to no new course; it was kept.", old.Title, leftover);
            }

            // The other built-in courses now come after the new ones in the catalogue.
            if (created.Count > 0)
                foreach (var seed in seeds.Where(s => string.IsNullOrWhiteSpace(s.SplitFrom)))
                {
                    var other = all.FirstOrDefault(c => !c.IsDeleted && string.Equals(c.Slug, seed.Slug, StringComparison.OrdinalIgnoreCase));
                    if (other is null || other.DisplayOrder >= seed.DisplayOrder) continue;
                    other.DisplayOrder = seed.DisplayOrder;
                    await courses.UpdateAsync(other, ct);
                }

            logger.LogWarning("Split course '{Title}' into {Count} courses ({Created} created, {Students} students enrolled in each){Removed}.",
                old.Title, targets.Count, created.Count, oldEnrollments.Count, removeOld ? "; old course removed" : string.Empty);
        }
    }

    private static readonly System.Text.RegularExpressions.Regex ChapterNumber =
        new(@"^\s*(?:Capitolo|Chapter|Unit|Unità)\s+(\d+)\b", System.Text.RegularExpressions.RegexOptions.IgnoreCase);

    private static readonly System.Text.RegularExpressions.Regex KeyNumber = new(@"-(\d+)$");

    /// <summary>The unit of the old course that a seed unit stands for: same title, else same chapter number, else same position.</summary>
    private static CourseUnit? FindSplitUnit(List<CourseUnit> oldUnits, List<string> oldKeys, UnitSeed seed, HashSet<Guid> taken)
    {
        var free = oldUnits.Where(u => !taken.Contains(u.Id)).ToList();
        var unit = free.FirstOrDefault(u => string.Equals(u.Title.Trim(), seed.Title.Trim(), StringComparison.OrdinalIgnoreCase)
                                         || string.Equals(u.TitleIt.Trim(), seed.TitleIt.Trim(), StringComparison.OrdinalIgnoreCase));

        var key = KeyNumber.Match(seed.UnitKey);
        if (unit is null && key.Success)
            unit = free.FirstOrDefault(u =>
                (ChapterNumber.Match(u.TitleIt) is { Success: true } a && a.Groups[1].Value == key.Groups[1].Value)
                || (ChapterNumber.Match(u.Title) is { Success: true } b && b.Groups[1].Value == key.Groups[1].Value));

        var index = oldKeys.FindIndex(k => string.Equals(k, seed.UnitKey, StringComparison.OrdinalIgnoreCase));
        if (unit is null && index >= 0 && oldUnits.Count == oldKeys.Count && !taken.Contains(oldUnits[index].Id))
            unit = oldUnits[index];

        if (unit is not null) taken.Add(unit.Id);
        return unit;
    }

    private static async Task MoveUnitAsync(CourseUnit unit, Guid courseId, int order, ICurriculumRepository curriculum,
        MongoDbContext? mongo, InMemoryStore? memory, CancellationToken ct)
    {
        foreach (var module in await curriculum.GetModulesByUnitAsync(unit.Id, ct))
        {
            module.CourseId = courseId;
            await curriculum.UpdateModuleAsync(module, ct);
        }
        foreach (var lesson in await curriculum.GetLessonsByUnitAsync(unit.Id, ct))
        {
            lesson.CourseId = courseId;
            await curriculum.UpdateLessonAsync(lesson, ct);
        }
        if (mongo is not null)
            await mongo.LessonProgress.UpdateManyAsync(p => p.UnitId == unit.Id,
                Builders<LessonProgress>.Update.Set(p => p.CourseId, courseId), cancellationToken: ct);
        else if (memory is not null)
            foreach (var p in memory.LessonProgress.Where(p => p.UnitId == unit.Id)) p.CourseId = courseId;

        unit.CourseId = courseId;
        unit.Order = order;
        await curriculum.UpdateUnitAsync(unit, ct);
    }

    /// <summary>Moves everything else that belongs to a course (classes, tests, materials…) to another course.</summary>
    private static async Task ReassignCourseAsync(MongoDbContext? mongo, InMemoryStore? memory, Guid from, Guid to, CancellationToken ct)
    {
        if (mongo is not null)
        {
            Task Move<T>(IMongoCollection<T> c, System.Linq.Expressions.Expression<Func<T, Guid>> field) =>
                c.UpdateManyAsync(Builders<T>.Filter.Eq(field, from), Builders<T>.Update.Set(field, to), cancellationToken: ct);
            await Move(mongo.Classes, x => x.CourseId);
            await Move(mongo.Assignments, x => x.CourseId);
            await Move(mongo.Assessments, x => x.CourseId);
            await Move(mongo.Materials, x => x.CourseId);
            await Move(mongo.LearningResources, x => x.CourseId);
            await Move(mongo.Subscriptions, x => x.CourseId);
            await Move(mongo.Certificates, x => x.CourseId);
            await Move(mongo.Progress, x => x.CourseId);
            await Move(mongo.Lessons, x => x.CourseId);
            await Move(mongo.CourseModules, x => x.CourseId);
            await Move(mongo.LessonProgress, x => x.CourseId);
            await mongo.Enrollments.DeleteManyAsync(e => e.CourseId == from, ct); // copied to every new course
        }
        else if (memory is not null)
        {
            foreach (var x in memory.Classes.Where(x => x.CourseId == from)) x.CourseId = to;
            foreach (var x in memory.Assignments.Where(x => x.CourseId == from)) x.CourseId = to;
            foreach (var x in memory.Assessments.Where(x => x.CourseId == from)) x.CourseId = to;
            foreach (var x in memory.Materials.Where(x => x.CourseId == from)) x.CourseId = to;
            foreach (var x in memory.LearningResources.Where(x => x.CourseId == from)) x.CourseId = to;
            foreach (var x in memory.Subscriptions.Where(x => x.CourseId == from)) x.CourseId = to;
            foreach (var x in memory.Certificates.Where(x => x.CourseId == from)) x.CourseId = to;
            foreach (var x in memory.Progress.Where(x => x.CourseId == from)) x.CourseId = to;
            foreach (var x in memory.Lessons.Where(x => x.CourseId == from)) x.CourseId = to;
            foreach (var x in memory.CourseModules.Where(x => x.CourseId == from)) x.CourseId = to;
            foreach (var x in memory.LessonProgress.Where(x => x.CourseId == from)) x.CourseId = to;
            memory.Enrollments.RemoveAll(e => e.CourseId == from);
        }
    }

    private static Course NewCourse(CourseSeed seed) => new()
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

    private static async Task<List<CourseSeed>> LoadSeedsAsync(CancellationToken ct)
    {
        var assembly = typeof(CourseContentSeeder).Assembly;
        var seeds = new List<CourseSeed>();
        foreach (var name in assembly.GetManifestResourceNames().Where(n => n.Contains(".Seed.") && n.EndsWith(".json")).Order(StringComparer.Ordinal))
        {
            await using var stream = assembly.GetManifestResourceStream(name)!;
            seeds.Add(await JsonSerializer.DeserializeAsync<CourseSeed>(stream, Json, ct)
                ?? throw new InvalidOperationException($"Empty course seed {name}"));
        }
        return seeds;
    }

    private async Task SeedCoursesAsync(IServiceProvider sp, IReadOnlyList<CourseSeed> seeds, CancellationToken ct)
    {
        var courses = sp.GetRequiredService<ICourseRepository>();
        var curriculum = sp.GetRequiredService<ICurriculumRepository>();
        var existing = await courses.GetAllAsync(ct);

        foreach (var seed in seeds)
        {
            var current = existing.FirstOrDefault(c => !c.IsDeleted && string.Equals(c.Slug, seed.Slug, StringComparison.OrdinalIgnoreCase));
            if (current is not null)
            {
                await AppendNewUnitsAsync(current, seed, courses, curriculum, ct);
                continue;
            }

            var course = NewCourse(seed);
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

        // The new chapters can change what the course covers (e.g. A0 → A2). Refresh the
        // course texts too, but only while the title is still one of the built-in ones:
        // a course an admin has renamed keeps all of its texts.
        if (seed.PreviousTitles.Contains(course.Title.Trim(), StringComparer.OrdinalIgnoreCase))
        {
            course.Title = seed.Title;
            course.ShortDescription = seed.ShortDescription;
            course.Description = seed.Description;
            course.Objectives = seed.Objectives;
            course.CefrLevel = seed.CefrLevel;
            course.DurationWeeks = seed.DurationWeeks;
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
    /// <summary>Block titles that used to carry Punjabi (25 Nov 2026), after <see cref="PunjabiText.Plain"/>.</summary>
    private static readonly Dictionary<string, string> RenamedTitles = new(StringComparer.Ordinal)
    {
        ["Il verbo ESSERE (to be · Hona)"] = "Il verbo ESSERE (to be)",
        ["ESSERE (to be · Hona) e AVERE (to have · Kol hona)"] = "ESSERE (to be) e AVERE (to have)",
    };

    private static readonly JsonSerializerOptions RawJson = new()
    {
        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
    };

    /// <summary>
    /// Small fixes to built-in lessons that are already in the database (they are never reloaded,
    /// so admin edits are kept). Runs on every start and changes a lesson only when needed:
    ///  - Roman Punjabi in plain letters (Main, Tusi, haan — not Maiṁ, Tusīṁ, hāṁ), also in translations;
    ///  - picture cards without a picture get the seed's picture (a picture an admin chose stays);
    ///  - grammar tables that had only a Punjabi column get the seed's version with an English column.
    /// </summary>
    private async Task RefreshBuiltInContentAsync(IServiceProvider sp, IReadOnlyList<CourseSeed> seeds, CancellationToken ct)
    {
        var courses = sp.GetRequiredService<ICourseRepository>();
        var curriculum = sp.GetRequiredService<ICurriculumRepository>();
        var changed = 0;
        foreach (var course in (await courses.GetAllAsync(ct)).Where(c => !c.IsDeleted))
        {
            var seed = seeds.FirstOrDefault(s => string.Equals(s.Slug, course.Slug, StringComparison.OrdinalIgnoreCase));
            var seedBlocks = seed?.Units.SelectMany(u => u.Lessons).SelectMany(l => l.Blocks).ToList() ?? [];
            var pictures = seedBlocks.Where(b => b.Type == LessonBlockType.Vocabulary)
                .SelectMany(b => b.Items.Where(i => !string.IsNullOrEmpty(i.ImageUrl)).Select(i => (Key: $"{b.Title}|{i.Italian}", i.ImageUrl)))
                .GroupBy(x => x.Key).ToDictionary(g => g.Key, g => g.First().ImageUrl!);
            var tables = seedBlocks.Where(b => b.Type == LessonBlockType.Table && b.Table.Count > 0).ToList();

            foreach (var lesson in await curriculum.GetLessonsByCourseAsync(course.Id, ct))
            {
                var before = JsonSerializer.Serialize(lesson, RawJson);
                var plain = PunjabiText.Plain(before)!;
                var fixedLesson = plain == before ? lesson : JsonSerializer.Deserialize<Lesson>(plain, RawJson)!;

                foreach (var b in fixedLesson.Blocks)
                {
                    if (b.Title is not null && RenamedTitles.TryGetValue(b.Title, out var title)) b.Title = title;
                    if (b.Type == LessonBlockType.Vocabulary)
                        foreach (var item in b.Items.Where(i => string.IsNullOrEmpty(i.ImageUrl)))
                            if (pictures.TryGetValue($"{b.Title}|{item.Italian}", out var url)) item.ImageUrl = url;
                    if (b.Type == LessonBlockType.Table && b.Table.Count > 0 && b.Table[0].Contains("Punjabi") && !b.Table[0].Contains("English"))
                    {
                        // The seed's table is this one plus an English column.
                        var match = tables.FirstOrDefault(t => t.Title == b.Title && WithoutColumn(t.Table, t.Table[0].IndexOf("English")) == Flat(b.Table));
                        if (match is not null) b.Table = match.Table.Select(r => r.ToList()).ToList();
                    }
                }

                if (JsonSerializer.Serialize(fixedLesson, RawJson) == before) continue;
                // Change the stored lesson itself (only texts and blocks; ids, order and module stay).
                lesson.Title = fixedLesson.Title;
                lesson.TitleIt = fixedLesson.TitleIt;
                lesson.Summary = fixedLesson.Summary;
                lesson.Homework = fixedLesson.Homework;
                lesson.Blocks = fixedLesson.Blocks;
                lesson.Exercises = fixedLesson.Exercises;
                await curriculum.UpdateLessonAsync(lesson, ct);
                changed++;
            }
        }

        var translations = sp.GetRequiredService<ITranslationRepository>();
        var rows = (await translations.GetAllAsync(ct)).Where(t => PunjabiText.Plain(t.Punjabi) != t.Punjabi).ToList();
        foreach (var t in rows) t.Punjabi = PunjabiText.Plain(t.Punjabi);
        if (rows.Count > 0) await translations.UpdateRangeAsync(rows, ct);

        if (changed > 0 || rows.Count > 0)
            logger.LogInformation("Built-in content refreshed: {Lessons} lessons, {Translations} Punjabi translations.", changed, rows.Count);
    }

    private static string Flat(IEnumerable<IEnumerable<string>> table) => string.Join("\n", table.Select(r => string.Join("\t", r)));

    private static string WithoutColumn(List<List<string>> table, int column) =>
        column < 0 ? string.Empty : Flat(table.Select(r => r.Where((_, i) => i != column)));

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
        /// <summary>Earlier built-in titles of this course (see AppendNewUnitsAsync).</summary>
        public List<string> PreviousTitles { get; set; } = [];
        /// <summary>Slug of the course this one was split out of (see SplitCoursesAsync).</summary>
        public string? SplitFrom { get; set; }
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
