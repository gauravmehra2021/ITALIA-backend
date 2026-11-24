using System.Text.RegularExpressions;
using SSeuropa.Application.Interfaces;
using SSeuropa.Domain.Entities;
using SSeuropa.Domain.Enums;

namespace SSeuropa.Application.Services;

/// <summary>
/// Keeps the Unit → Module → Lesson structure consistent:
///   • every lesson belongs to a module of its unit,
///   • every module starts with exactly one module video lesson (test modules have none),
///   • Lesson.Order runs 1…n through the whole unit in module order (video first),
///     so everything that sorts lessons by unit and Order stays correct.
///
/// Content made before modules existed is grouped automatically, once, on startup:
/// lessons titled "Lesson N · …" (the school's classroom Lezioni) become one module per
/// Lezione, a chapter test gets its own module, and other units get one module each.
/// Students' progress is never touched.
/// </summary>
public static partial class ModuleStructure
{
    public static bool HasVideo(Lesson l) =>
        l.Blocks.Any(b => b.Type == LessonBlockType.Video && !string.IsNullOrWhiteSpace(b.MediaUrl));

    /// <summary>A module video that has no video yet: shown as "coming soon" and never blocks students.</summary>
    public static bool IsPendingVideo(Lesson l) => l.IsModuleVideo && !HasVideo(l);

    public static string VideoTitle(string moduleTitle) => $"Video: {moduleTitle}";

    private const string TestModuleDescription = "Chapter test: pass with 60% to finish the chapter.";

    public static Lesson NewVideoLesson(CourseModule module, LessonStatus status) => new()
    {
        CourseId = module.CourseId,
        UnitId = module.UnitId,
        ModuleId = module.Id,
        IsModuleVideo = true,
        Kind = LessonKind.Video,
        Title = VideoTitle(module.Title),
        Summary = "Watch the video of this module first.",
        EstimatedMinutes = 5,
        XpReward = 10,
        Status = status,
        Order = 0,
    };

    [GeneratedRegex(@"^\s*(?:lesson|lezione)\s+(\d+)\s*[·:\-–—.]\s*(.+)$", RegexOptions.IgnoreCase)]
    private static partial Regex NumberedLesson();

    /// <summary>Makes every unit of the course follow the module structure. Returns the number of changes.</summary>
    public static async Task<int> EnsureCourseAsync(ICurriculumRepository repo, Guid courseId, CancellationToken ct = default)
    {
        var changes = 0;
        foreach (var unit in await repo.GetUnitsAsync(courseId, ct))
            changes += await EnsureUnitAsync(repo, unit, ct);
        return changes;
    }

    public static async Task<int> EnsureUnitAsync(ICurriculumRepository repo, CourseUnit unit, CancellationToken ct = default)
    {
        var changes = 0;
        var modules = (await repo.GetModulesByUnitAsync(unit.Id, ct)).OrderBy(m => m.Order).ToList();
        var lessons = (await repo.GetLessonsByUnitAsync(unit.Id, ct)).OrderBy(l => l.Order).ToList();
        var moduleIds = modules.Select(m => m.Id).ToHashSet();
        var loose = lessons.Where(l => l.ModuleId is not { } id || !moduleIds.Contains(id)).ToList();

        if (loose.Count > 0)
        {
            if (modules.Count > 0)
            {
                // Lessons without a module in a unit that has modules: add them to the last module.
                var last = modules[^1];
                foreach (var l in loose)
                {
                    l.ModuleId = last.Id;
                    l.IsModuleVideo = false;
                    l.Order = int.MaxValue / 2;
                    await repo.UpdateLessonAsync(l, ct);
                    changes++;
                }
            }
            else
            {
                var order = 0;
                foreach (var group in Group(unit, loose))
                {
                    var module = new CourseModule
                    {
                        CourseId = unit.CourseId,
                        UnitId = unit.Id,
                        Order = ++order,
                        Title = group.Title,
                        Description = group.Description,
                        IsTest = group.Lessons.Count > 0 && group.Lessons.All(l => l.Kind == LessonKind.Exam),
                    };
                    await repo.AddModuleAsync(module, ct);
                    modules.Add(module);
                    foreach (var l in group.Lessons)
                    {
                        l.ModuleId = module.Id;
                        l.IsModuleVideo = false;
                        await repo.UpdateLessonAsync(l, ct);
                    }
                    changes++;
                }
            }
        }

        // Exactly one module video per module — except test modules, which have none.
        foreach (var module in modules)
        {
            var videos = lessons.Where(l => l.ModuleId == module.Id && l.IsModuleVideo).OrderBy(l => l.Order).ToList();
            var others = lessons.Where(l => l.ModuleId == module.Id && !l.IsModuleVideo).ToList();

            // Chapter-test modules made automatically before test modules existed become test modules
            // (once: an admin can still give a test module a video afterwards).
            if (!module.IsTest && module.Description == TestModuleDescription && others.Count > 0
                && others.All(l => l.Kind == LessonKind.Exam) && videos.All(IsPendingVideo))
            {
                module.IsTest = true;
                await repo.UpdateModuleAsync(module, ct);
                changes++;
            }

            if (module.IsTest)
            {
                // Placeholders that were never filled can go (they can't have progress); a real video stays.
                foreach (var pending in videos.Where(IsPendingVideo))
                {
                    await repo.DeleteLessonAsync(pending.Id, ct);
                    lessons.Remove(pending);
                    changes++;
                }
                continue;
            }

            if (videos.Count == 0)
            {
                var video = NewVideoLesson(module, unit.Status);
                await repo.AddLessonAsync(video, ct);
                lessons.Add(video);
                changes++;
            }
            foreach (var extra in videos.Skip(1))
            {
                extra.IsModuleVideo = false;
                await repo.UpdateLessonAsync(extra, ct);
                changes++;
            }
        }

        changes += await NormalizeUnitAsync(repo, unit.Id, ct);
        return changes;
    }

    /// <summary>
    /// Renumbers modules 1…n and lessons 1…n through the unit: by module, module video first,
    /// then the lessons in their current order.
    /// </summary>
    public static async Task<int> NormalizeUnitAsync(ICurriculumRepository repo, Guid unitId, CancellationToken ct = default)
    {
        var changes = 0;
        var modules = (await repo.GetModulesByUnitAsync(unitId, ct)).OrderBy(m => m.Order).ToList();
        for (var i = 0; i < modules.Count; i++)
        {
            if (modules[i].Order == i + 1) continue;
            modules[i].Order = i + 1;
            await repo.UpdateModuleAsync(modules[i], ct);
            changes++;
        }

        var position = modules.Select((m, i) => (m.Id, i)).ToDictionary(x => x.Id, x => x.i);
        var lessons = (await repo.GetLessonsByUnitAsync(unitId, ct))
            .OrderBy(l => l.ModuleId is { } id && position.TryGetValue(id, out var p) ? p : -1)
            .ThenBy(l => l.IsModuleVideo ? 0 : 1)
            .ThenBy(l => l.Order)
            .ToList();
        for (var i = 0; i < lessons.Count; i++)
        {
            if (lessons[i].Order == i + 1) continue;
            lessons[i].Order = i + 1;
            await repo.UpdateLessonAsync(lessons[i], ct);
            changes++;
        }
        return changes;
    }

    private sealed record LessonGroup(string Title, string Description, List<Lesson> Lessons);

    private static List<LessonGroup> Group(CourseUnit unit, List<Lesson> lessons)
    {
        var study = lessons.Where(l => l.Kind != LessonKind.Exam).ToList();
        var matches = study.ToDictionary(l => l.Id, l => NumberedLesson().Match(l.Title));
        var numbered = study.Count > 0
            && matches.Values.All(m => m.Success)
            && matches.Values.Select(m => m.Groups[1].Value).Distinct().Count() >= 2;

        if (!numbered)
        {
            return [new LessonGroup(
                unit.Title,
                string.Join(" · ", lessons.Where(l => l.Kind != LessonKind.Exam).Select(l => l.Title)),
                lessons)];
        }

        // One module per classroom lesson ("Lezione"); each test in its own module.
        var groups = new List<(string Key, string Title, List<Lesson> Lessons)>();
        foreach (var l in lessons)
        {
            var key = l.Kind == LessonKind.Exam ? $"exam:{l.Id}" : $"n:{matches[l.Id].Groups[1].Value}";
            var group = groups.FirstOrDefault(g => g.Key == key);
            if (group.Key is null)
            {
                var title = l.Kind == LessonKind.Exam ? l.Title : $"Lezione {matches[l.Id].Groups[1].Value}";
                group = (key, title, []);
                groups.Add(group);
            }
            group.Lessons.Add(l);
        }

        return groups.Select(g => new LessonGroup(
            g.Title,
            g.Key.StartsWith("exam:")
                ? TestModuleDescription
                : string.Join(" · ", g.Lessons.Select(l => matches[l.Id].Groups[2].Value.Trim())),
            g.Lessons)).ToList();
    }
}
