using SSeuropa.Application.DTOs;
using SSeuropa.Application.Interfaces;
using SSeuropa.Domain.Entities;
using SSeuropa.Domain.Enums;
using SSeuropa.Domain.Exceptions;

namespace SSeuropa.Application.Services;

/// <summary>
/// Course curriculum (units → lessons → blocks + exercises) and student progress.
///
/// Staff (Teacher/Admin/Super Admin) build and publish content. Students see only
/// published content of courses they're enrolled in (plus free-preview lessons),
/// unlock lessons in order, and earn stars/XP per lesson.
/// </summary>
public class CurriculumService(
    ICurriculumRepository curriculum,
    ICourseRepository courses,
    IEnrollmentRepository enrollments,
    IProgressRepository courseProgress,
    IUserRepository users,
    ICurrentUserService current,
    CourseService courseService,
    CourseAccessService access)
{
    // Stars from a lesson score. 50% is the pass mark.
    public static int StarsFor(int scorePercent) => scorePercent switch
    {
        >= 90 => 3,
        >= 70 => 2,
        >= 50 => 1,
        _ => 0
    };

    /// <summary>Chapter tests (Exam lessons) need 60% to pass, as in the school's paper tests.</summary>
    public const int ExamPassPercent = 60;

    public static int StarsFor(int scorePercent, LessonKind kind) =>
        kind == LessonKind.Exam && scorePercent < ExamPassPercent ? 0 : StarsFor(scorePercent);

    private bool IsStaff => current.IsInRole(UserRole.Mentor, UserRole.Admin, UserRole.SuperAdmin, UserRole.Staff);

    /// <summary>Students with UnlockAllLessons (e.g. the demo account) can open any lesson of a course they're enrolled in.</summary>
    private async Task<bool> UnlocksAllLessonsAsync(Guid userId, CancellationToken ct) =>
        (await users.GetByIdAsync(userId, ct))?.UnlockAllLessons == true;

    // ══════════════════════════════════════════════════════════════════════
    //  Staff: outline & editing
    // ══════════════════════════════════════════════════════════════════════

    public async Task<CourseOutlineDto> GetStaffOutlineAsync(Guid courseId, CancellationToken ct = default)
    {
        EnsureStaff();
        var course = await GetCourseAsync(courseId, ct);
        var units = await curriculum.GetUnitsAsync(courseId, ct);
        var modules = await curriculum.GetModulesAsync(courseId, ct);
        var lessons = await curriculum.GetLessonsByCourseAsync(courseId, ct);

        var unitDtos = units.Select(u =>
        {
            var summaries = lessons.Where(l => l.UnitId == u.Id).OrderBy(l => l.Order).Select(l => ToSummary(l)).ToList();
            return ToUnitDto(u, summaries, modules: StaffModules(modules.Where(m => m.UnitId == u.Id), summaries, lessons));
        }).ToList();

        var required = lessons.Count(l => !ModuleStructure.IsPendingVideo(l));
        return new CourseOutlineDto(
            await courseService.GetByIdAsync(course.Id, ct), unitDtos, required, 0, 0, 0, required * 3, 0, false, null);
    }

    private static List<ModuleDto> StaffModules(IEnumerable<CourseModule> modules, List<LessonSummaryDto> summaries, IReadOnlyList<Lesson> lessons) =>
        modules.OrderBy(m => m.Order).Select(m =>
        {
            var mine = summaries.Where(s => s.ModuleId == m.Id).ToList();
            var video = lessons.FirstOrDefault(l => l.ModuleId == m.Id && l.IsModuleVideo);
            return new ModuleDto(m.Id, m.UnitId, m.Order, m.Title, m.TitleIt, m.Description, m.ImageUrl, mine,
                mine.Count(s => !s.VideoPending), VideoReady: video is not null && ModuleStructure.HasVideo(video), IsTest: m.IsTest);
        }).ToList();

    // ── Modules ────────────────────────────────────────────────────────────

    public async Task<ModuleDto> CreateModuleAsync(Guid unitId, SaveModuleRequest request, CancellationToken ct = default)
    {
        var unit = await curriculum.GetUnitAsync(unitId, ct) ?? throw new NotFoundException("Unit", unitId);
        EnsureCanEdit(await GetCourseAsync(unit.CourseId, ct));
        Require(request.Title, "Module title");

        var existing = await curriculum.GetModulesByUnitAsync(unitId, ct);
        var module = new CourseModule
        {
            CourseId = unit.CourseId,
            UnitId = unit.Id,
            Order = existing.Count == 0 ? 1 : existing.Max(m => m.Order) + 1,
        };
        Apply(module, request);
        await curriculum.AddModuleAsync(module, ct);
        // Every module starts with its video lesson (the video itself can be added later) — test modules have none.
        if (!module.IsTest)
            await curriculum.AddLessonAsync(ModuleStructure.NewVideoLesson(module, unit.Status), ct);
        await ModuleStructure.NormalizeUnitAsync(curriculum, unit.Id, ct);
        return await GetStaffModuleAsync(module, ct);
    }

    public async Task<ModuleDto> UpdateModuleAsync(Guid moduleId, SaveModuleRequest request, CancellationToken ct = default)
    {
        var module = await curriculum.GetModuleAsync(moduleId, ct) ?? throw new NotFoundException("Module", moduleId);
        EnsureCanEdit(await GetCourseAsync(module.CourseId, ct));
        Require(request.Title, "Module title");

        var oldTitle = module.Title;
        Apply(module, request);
        await curriculum.UpdateModuleAsync(module, ct);

        // Keep the default name of the module video in step with the module name.
        var video = (await curriculum.GetLessonsByUnitAsync(module.UnitId, ct)).FirstOrDefault(l => l.ModuleId == module.Id && l.IsModuleVideo);
        if (video is not null && video.Title == ModuleStructure.VideoTitle(oldTitle) && oldTitle != module.Title)
        {
            video.Title = ModuleStructure.VideoTitle(module.Title);
            await curriculum.UpdateLessonAsync(video, ct);
        }

        // Switched to or from "test module": add or remove the (empty) video lesson.
        if (await curriculum.GetUnitAsync(module.UnitId, ct) is { } unit)
            await ModuleStructure.EnsureUnitAsync(curriculum, unit, ct);
        return await GetStaffModuleAsync(module, ct);
    }

    public async Task MoveModuleAsync(Guid moduleId, int direction, CancellationToken ct = default)
    {
        var module = await curriculum.GetModuleAsync(moduleId, ct) ?? throw new NotFoundException("Module", moduleId);
        EnsureCanEdit(await GetCourseAsync(module.CourseId, ct));
        var modules = (await curriculum.GetModulesByUnitAsync(module.UnitId, ct)).OrderBy(m => m.Order).ToList();
        var index = modules.FindIndex(m => m.Id == moduleId);
        var target = index + Math.Sign(direction);
        if (target < 0 || target >= modules.Count) return;
        (modules[index], modules[target]) = (modules[target], modules[index]);
        for (var i = 0; i < modules.Count; i++)
        {
            if (modules[i].Order == i + 1) continue;
            modules[i].Order = i + 1;
            await curriculum.UpdateModuleAsync(modules[i], ct);
        }
        await ModuleStructure.NormalizeUnitAsync(curriculum, module.UnitId, ct);
    }

    public async Task DeleteModuleAsync(Guid moduleId, CancellationToken ct = default)
    {
        var module = await curriculum.GetModuleAsync(moduleId, ct) ?? throw new NotFoundException("Module", moduleId);
        EnsureCanEdit(await GetCourseAsync(module.CourseId, ct));
        await curriculum.DeleteModuleAsync(moduleId, ct);
        await ModuleStructure.NormalizeUnitAsync(curriculum, module.UnitId, ct);
    }

    private async Task<ModuleDto> GetStaffModuleAsync(CourseModule module, CancellationToken ct)
    {
        var lessons = await curriculum.GetLessonsByUnitAsync(module.UnitId, ct);
        var summaries = lessons.OrderBy(l => l.Order).Select(l => ToSummary(l)).ToList();
        return StaffModules([module], summaries, lessons)[0];
    }

    /// <summary>The module a new lesson goes into: the one asked for, or the last module of the unit (created if the unit has none).</summary>
    private async Task<CourseModule> ResolveModuleAsync(CourseUnit unit, Guid? moduleId, CancellationToken ct)
    {
        if (moduleId is { } id)
        {
            var chosen = await curriculum.GetModuleAsync(id, ct) ?? throw new NotFoundException("Module", id);
            if (chosen.UnitId != unit.Id) throw new ValidationDomainException("That module belongs to another unit.");
            return chosen;
        }
        var modules = await curriculum.GetModulesByUnitAsync(unit.Id, ct);
        if (modules.Count > 0) return modules.OrderBy(m => m.Order).Last();

        var module = new CourseModule { CourseId = unit.CourseId, UnitId = unit.Id, Order = 1, Title = "Module 1" };
        await curriculum.AddModuleAsync(module, ct);
        await curriculum.AddLessonAsync(ModuleStructure.NewVideoLesson(module, unit.Status), ct);
        return module;
    }

    public async Task<UnitDto> CreateUnitAsync(Guid courseId, SaveUnitRequest request, CancellationToken ct = default)
    {
        var course = await GetCourseAsync(courseId, ct);
        EnsureCanEdit(course);
        Require(request.Title, "Unit title");

        var existing = await curriculum.GetUnitsAsync(courseId, ct);
        var unit = new CourseUnit
        {
            CourseId = courseId,
            Order = existing.Count == 0 ? 1 : existing.Max(u => u.Order) + 1
        };
        Apply(unit, request);
        await curriculum.AddUnitAsync(unit, ct);
        // A new unit starts with its first module (and that module's video lesson).
        await ResolveModuleAsync(unit, null, ct);
        return await GetStaffUnitAsync(unit, ct);
    }

    private async Task<UnitDto> GetStaffUnitAsync(CourseUnit unit, CancellationToken ct)
    {
        var lessons = await curriculum.GetLessonsByUnitAsync(unit.Id, ct);
        var summaries = lessons.OrderBy(l => l.Order).Select(l => ToSummary(l)).ToList();
        return ToUnitDto(unit, summaries, modules: StaffModules(await curriculum.GetModulesByUnitAsync(unit.Id, ct), summaries, lessons));
    }

    public async Task<UnitDto> UpdateUnitAsync(Guid unitId, SaveUnitRequest request, CancellationToken ct = default)
    {
        var unit = await curriculum.GetUnitAsync(unitId, ct) ?? throw new NotFoundException("Unit", unitId);
        EnsureCanEdit(await GetCourseAsync(unit.CourseId, ct));
        Require(request.Title, "Unit title");

        Apply(unit, request);
        await curriculum.UpdateUnitAsync(unit, ct);
        return await GetStaffUnitAsync(unit, ct);
    }

    public async Task DeleteUnitAsync(Guid unitId, CancellationToken ct = default)
    {
        var unit = await curriculum.GetUnitAsync(unitId, ct) ?? throw new NotFoundException("Unit", unitId);
        EnsureCanEdit(await GetCourseAsync(unit.CourseId, ct));
        await curriculum.DeleteUnitAsync(unitId, ct);
        await RenumberUnitsAsync(unit.CourseId, ct);
    }

    public async Task MoveUnitAsync(Guid unitId, int direction, CancellationToken ct = default)
    {
        var unit = await curriculum.GetUnitAsync(unitId, ct) ?? throw new NotFoundException("Unit", unitId);
        EnsureCanEdit(await GetCourseAsync(unit.CourseId, ct));
        var units = (await curriculum.GetUnitsAsync(unit.CourseId, ct)).ToList();
        var index = units.FindIndex(u => u.Id == unitId);
        var target = index + Math.Sign(direction);
        if (target < 0 || target >= units.Count) return;
        (units[index], units[target]) = (units[target], units[index]);
        for (var i = 0; i < units.Count; i++)
        {
            if (units[i].Order == i + 1) continue;
            units[i].Order = i + 1;
            await curriculum.UpdateUnitAsync(units[i], ct);
        }
    }

    public async Task<LessonDto> CreateLessonAsync(Guid unitId, SaveLessonRequest request, CancellationToken ct = default)
    {
        var unit = await curriculum.GetUnitAsync(unitId, ct) ?? throw new NotFoundException("Unit", unitId);
        EnsureCanEdit(await GetCourseAsync(unit.CourseId, ct));
        Require(request.Title, "Lesson title");

        var module = await ResolveModuleAsync(unit, request.ModuleId, ct);
        var lesson = new Lesson
        {
            CourseId = unit.CourseId,
            UnitId = unit.Id,
            ModuleId = module.Id,
            Order = int.MaxValue / 2, // last in its module; renumbered below
        };
        Apply(lesson, request with { Order = null });
        await curriculum.AddLessonAsync(lesson, ct);
        await ModuleStructure.NormalizeUnitAsync(curriculum, unit.Id, ct);
        return await ToLessonDtoAsync(lesson, null, ct);
    }

    public async Task<LessonDto> CreateLessonInModuleAsync(Guid moduleId, SaveLessonRequest request, CancellationToken ct = default)
    {
        var module = await curriculum.GetModuleAsync(moduleId, ct) ?? throw new NotFoundException("Module", moduleId);
        return await CreateLessonAsync(module.UnitId, request with { ModuleId = module.Id }, ct);
    }

    public async Task<LessonDto> UpdateLessonAsync(Guid lessonId, SaveLessonRequest request, CancellationToken ct = default)
    {
        var lesson = await curriculum.GetLessonAsync(lessonId, ct) ?? throw new NotFoundException("Lesson", lessonId);
        EnsureCanEdit(await GetCourseAsync(lesson.CourseId, ct));
        Require(request.Title, "Lesson title");

        var oldUnitId = lesson.UnitId;
        var targetUnitId = request.UnitId ?? lesson.UnitId;
        var moving = targetUnitId != lesson.UnitId || (request.ModuleId.HasValue && request.ModuleId != lesson.ModuleId);
        if (moving)
        {
            if (lesson.IsModuleVideo)
                throw new ValidationDomainException("The module video always stays first in its own module.");
            var newUnit = await curriculum.GetUnitAsync(targetUnitId, ct) ?? throw new NotFoundException("Unit", targetUnitId);
            if (newUnit.CourseId != lesson.CourseId)
                throw new ValidationDomainException("A lesson can only move to a unit of the same course.");
            var module = await ResolveModuleAsync(newUnit, request.ModuleId, ct);
            lesson.UnitId = newUnit.Id;
            lesson.ModuleId = module.Id;
            lesson.Order = int.MaxValue / 2; // last in the new module
        }

        Apply(lesson, moving ? request with { Order = null } : request);
        await curriculum.UpdateLessonAsync(lesson, ct);
        await ModuleStructure.NormalizeUnitAsync(curriculum, lesson.UnitId, ct);
        if (oldUnitId != lesson.UnitId) await ModuleStructure.NormalizeUnitAsync(curriculum, oldUnitId, ct);
        return await ToLessonDtoAsync(lesson, null, ct);
    }

    public async Task DeleteLessonAsync(Guid lessonId, CancellationToken ct = default)
    {
        var lesson = await curriculum.GetLessonAsync(lessonId, ct) ?? throw new NotFoundException("Lesson", lessonId);
        EnsureCanEdit(await GetCourseAsync(lesson.CourseId, ct));
        if (lesson.IsModuleVideo)
            throw new ValidationDomainException("Every module starts with its video lesson. Open it to add or change the video, or delete the whole module.");
        await curriculum.DeleteLessonAsync(lessonId, ct);
        await ModuleStructure.NormalizeUnitAsync(curriculum, lesson.UnitId, ct);
    }

    /// <summary>Moves a lesson up or down inside its module. The module video always stays first.</summary>
    public async Task MoveLessonAsync(Guid lessonId, int direction, CancellationToken ct = default)
    {
        var lesson = await curriculum.GetLessonAsync(lessonId, ct) ?? throw new NotFoundException("Lesson", lessonId);
        EnsureCanEdit(await GetCourseAsync(lesson.CourseId, ct));
        if (lesson.IsModuleVideo)
            throw new ValidationDomainException("The module video always comes first in its module.");
        var siblings = (await curriculum.GetLessonsByUnitAsync(lesson.UnitId, ct))
            .Where(l => l.ModuleId == lesson.ModuleId && !l.IsModuleVideo)
            .OrderBy(l => l.Order)
            .ToList();
        var index = siblings.FindIndex(l => l.Id == lessonId);
        var target = index + Math.Sign(direction);
        if (index < 0 || target < 0 || target >= siblings.Count) return;
        (siblings[index].Order, siblings[target].Order) = (siblings[target].Order, siblings[index].Order);
        await curriculum.UpdateLessonAsync(siblings[index], ct);
        await curriculum.UpdateLessonAsync(siblings[target], ct);
        await ModuleStructure.NormalizeUnitAsync(curriculum, lesson.UnitId, ct);
    }

    /// <summary>Publishes or unpublishes every unit and lesson of a course in one go.</summary>
    public async Task SetCurriculumStatusAsync(Guid courseId, LessonStatus status, CancellationToken ct = default)
    {
        EnsureCanEdit(await GetCourseAsync(courseId, ct));
        foreach (var unit in await curriculum.GetUnitsAsync(courseId, ct))
        {
            unit.Status = status;
            await curriculum.UpdateUnitAsync(unit, ct);
        }
        foreach (var lesson in await curriculum.GetLessonsByCourseAsync(courseId, ct))
        {
            lesson.Status = status;
            await curriculum.UpdateLessonAsync(lesson, ct);
        }
    }

    public async Task<List<StudentCourseProgressDto>> GetStudentProgressAsync(Guid courseId, CancellationToken ct = default)
    {
        EnsureStaff();
        await GetCourseAsync(courseId, ct);
        var totalLessons = (await curriculum.GetLessonsByCourseAsync(courseId, ct)).Count(l => l.Status == LessonStatus.Published && !ModuleStructure.IsPendingVideo(l));
        var progress = await curriculum.GetProgressForCourseAsync(courseId, ct);
        var courseEnrollments = (await enrollments.GetAllAsync(ct)).Where(e => e.CourseId == courseId).ToList();

        var userIds = courseEnrollments.Select(e => e.UserId).Union(progress.Select(p => p.UserId)).Distinct();
        var result = new List<StudentCourseProgressDto>();
        foreach (var userId in userIds)
        {
            var user = await users.GetByIdAsync(userId, ct);
            if (user is null) continue;
            var mine = progress.Where(p => p.UserId == userId).ToList();
            var completed = mine.Count(p => p.Completed);
            result.Add(new StudentCourseProgressDto(
                userId,
                user.FullName,
                user.Email,
                completed,
                totalLessons,
                Percent(completed, totalLessons),
                mine.Sum(p => p.Stars),
                mine.Sum(p => p.XpEarned),
                mine.Count == 0 ? 0 : (int)Math.Round(mine.Average(p => p.BestScorePercent)),
                mine.Count == 0 ? null : mine.Max(p => p.LastAttemptAtUtc),
                courseEnrollments.FirstOrDefault(e => e.UserId == userId)?.ActivatedAtUtc));
        }

        return result.OrderByDescending(r => r.CompletionPercent).ThenBy(r => r.FullName).ToList();
    }

    // ══════════════════════════════════════════════════════════════════════
    //  Students: catalogue, outline, lessons, results
    // ══════════════════════════════════════════════════════════════════════

    public async Task<List<CourseCardDto>> GetCatalogueAsync(CancellationToken ct = default)
    {
        var userId = RequireUser();
        // Students see only their batch's courses, courses open to all, and courses they're already in.
        var visible = await access.GetVisibleCourseIdsAsync(userId, ct);
        var all = (await courses.GetAllAsync(ct))
            .Where(c => c.Status == CourseStatus.Published && !c.IsDeleted)
            .Where(c => visible is null || visible.Contains(c.Id))
            .OrderBy(c => c.DisplayOrder).ThenBy(c => c.Title)
            .ToList();

        var cards = new List<CourseCardDto>();
        foreach (var c in all)
            cards.Add(await BuildCardAsync(c, userId, ct));
        return cards;
    }

    public async Task<CourseOutlineDto> GetStudentOutlineAsync(Guid courseId, CancellationToken ct = default)
    {
        var userId = RequireUser();
        var course = await GetCourseAsync(courseId, ct);
        if (course.Status != CourseStatus.Published && !IsStaff)
            throw new NotFoundException("Course", courseId);
        if (!IsStaff && !await access.CanSeeAsync(userId, courseId, ct))
            throw new UnauthorizedAccessDomainException("This course is not part of your batch.");

        var isEnrolled = await IsEnrolledAsync(userId, courseId, ct);
        var (units, lessons) = await GetPublishedAsync(courseId, ct);
        var progress = (await curriculum.GetProgressAsync(userId, courseId, ct)).ToDictionary(p => p.LessonId);
        var unlocked = await UnlocksAllLessonsAsync(userId, ct)
            ? lessons.Select(l => l.Id).ToHashSet()
            : ComputeUnlocked(lessons, progress);

        var modules = await curriculum.GetModulesAsync(courseId, ct);
        bool Done(Lesson l) => progress.TryGetValue(l.Id, out var p) && p.Completed;

        var unitDtos = units.Select(u =>
        {
            var unitLessons = lessons.Where(l => l.UnitId == u.Id).ToList();
            var summaries = unitLessons.Select(l =>
            {
                progress.TryGetValue(l.Id, out var p);
                var open = IsStaff || (isEnrolled && unlocked.Contains(l.Id)) || l.IsFreePreview;
                return ToSummary(l, p, locked: !open);
            }).ToList();

            // Module cards: a module is open when one of its lessons is open, i.e. the previous
            // module is complete (lessons unlock strictly in order through the whole course).
            var moduleDtos = modules.Where(m => m.UnitId == u.Id).OrderBy(m => m.Order).Select(m =>
            {
                var mine = summaries.Where(s => s.ModuleId == m.Id).ToList();
                var required = unitLessons.Where(l => l.ModuleId == m.Id && !ModuleStructure.IsPendingVideo(l)).ToList();
                var video = unitLessons.FirstOrDefault(l => l.ModuleId == m.Id && l.IsModuleVideo);
                return new ModuleDto(m.Id, m.UnitId, m.Order, m.Title, m.TitleIt, m.Description, m.ImageUrl, mine,
                    required.Count,
                    required.Count(Done),
                    required.Sum(l => progress.TryGetValue(l.Id, out var p) ? p.Stars : 0),
                    Locked: mine.Count > 0 && mine.All(s => s.Locked == true),
                    Completed: required.Count > 0 && required.All(Done),
                    VideoReady: video is not null && ModuleStructure.HasVideo(video),
                    IsTest: m.IsTest);
            }).Where(m => m.Lessons.Count > 0).ToList();

            var unitRequired = unitLessons.Where(l => !ModuleStructure.IsPendingVideo(l)).ToList();
            return ToUnitDto(u, summaries,
                completed: unitRequired.Count(Done),
                stars: unitRequired.Sum(l => progress.TryGetValue(l.Id, out var p) ? p.Stars : 0),
                modules: moduleDtos);
        }).ToList();

        var requiredLessons = Required(lessons);
        var completedCount = requiredLessons.Count(Done);
        var next = requiredLessons.FirstOrDefault(l => !Done(l));

        return new CourseOutlineDto(
            await courseService.GetByIdAsync(courseId, ct),
            unitDtos,
            requiredLessons.Count,
            completedCount,
            Percent(completedCount, requiredLessons.Count),
            requiredLessons.Sum(l => progress.TryGetValue(l.Id, out var p) ? p.Stars : 0),
            requiredLessons.Count * 3,
            progress.Values.Sum(p => p.XpEarned),
            isEnrolled,
            next?.Id);
    }

    /// <summary>Lessons that count for progress: everything except module videos that have no video yet.</summary>
    private static List<Lesson> Required(IEnumerable<Lesson> lessons) => lessons.Where(l => !ModuleStructure.IsPendingVideo(l)).ToList();

    public async Task<LessonDto> GetLessonAsync(Guid lessonId, CancellationToken ct = default)
    {
        var userId = RequireUser();
        var lesson = await curriculum.GetLessonAsync(lessonId, ct) ?? throw new NotFoundException("Lesson", lessonId);

        if (IsStaff)
            return await ToLessonDtoAsync(lesson, await curriculum.GetLessonProgressAsync(userId, lessonId, ct), ct);

        var course = await GetCourseAsync(lesson.CourseId, ct);
        if (course.Status != CourseStatus.Published || lesson.Status != LessonStatus.Published)
            throw new NotFoundException("Lesson", lessonId);
        // Free-preview lessons too: only for courses the student may see.
        if (!await access.CanSeeAsync(userId, lesson.CourseId, ct))
            throw new UnauthorizedAccessDomainException("This course is not part of your batch.");

        if (!lesson.IsFreePreview)
        {
            if (!await IsEnrolledAsync(userId, lesson.CourseId, ct))
                throw new UnauthorizedAccessDomainException("Enroll in this course to open this lesson.");

            var (_, lessons) = await GetPublishedAsync(lesson.CourseId, ct);
            var progress = (await curriculum.GetProgressAsync(userId, lesson.CourseId, ct)).ToDictionary(p => p.LessonId);
            if (!ComputeUnlocked(lessons, progress).Contains(lessonId) && !await UnlocksAllLessonsAsync(userId, ct))
                throw new UnauthorizedAccessDomainException("Finish the previous lesson to unlock this one.");
        }

        return await ToLessonDtoAsync(lesson, await curriculum.GetLessonProgressAsync(userId, lessonId, ct), ct);
    }

    public async Task<LessonResultDto> CompleteLessonAsync(Guid lessonId, CompleteLessonRequest request, CancellationToken ct = default)
    {
        var userId = RequireUser();
        // Same access rules as opening the lesson.
        await GetLessonAsync(lessonId, ct);
        var lesson = (await curriculum.GetLessonAsync(lessonId, ct))!;
        if (ModuleStructure.IsPendingVideo(lesson))
            throw new ValidationDomainException("This video isn't ready yet. You can go on to the next lesson.");

        var score = Math.Clamp(request.ScorePercent, 0, 100);
        // Lessons without exercises (e.g. a video) count as fully completed.
        if (lesson.Exercises.Count == 0) score = 100;
        var stars = StarsFor(score, lesson.Kind);
        var now = DateTime.UtcNow;

        var progress = await curriculum.GetLessonProgressAsync(userId, lessonId, ct) ?? new LessonProgress
        {
            UserId = userId,
            CourseId = lesson.CourseId,
            UnitId = lesson.UnitId,
            LessonId = lesson.Id
        };

        var wasCompleted = progress.Completed;
        var isNewBest = score > progress.BestScorePercent || progress.Attempts == 0;
        var xpBefore = progress.XpEarned;

        progress.Attempts += 1;
        progress.LastAttemptAtUtc = now;
        progress.UnitId = lesson.UnitId;
        if (score > progress.BestScorePercent) progress.BestScorePercent = score;
        if (stars > progress.Stars) progress.Stars = stars;
        // XP: full reward for 3 stars, proportional below. Only improvements add XP.
        var xpForAttempt = (int)Math.Round(lesson.XpReward * stars / 3.0);
        if (xpForAttempt > progress.XpEarned) progress.XpEarned = xpForAttempt;
        if (stars >= 1 && !progress.Completed)
        {
            progress.Completed = true;
            progress.CompletedAtUtc = now;
        }

        await curriculum.UpsertProgressAsync(progress, ct);

        // Course-level progress + daily streak (shared with dashboards and the mobile app).
        var (_, published) = await GetPublishedAsync(lesson.CourseId, ct);
        var lessons = Required(published);
        var all = (await curriculum.GetProgressAsync(userId, lesson.CourseId, ct)).ToDictionary(p => p.LessonId);
        bool Done(Lesson l) => all.TryGetValue(l.Id, out var p) && p.Completed;
        var completedCount = lessons.Count(Done);
        var coursePercent = Percent(completedCount, lessons.Count);
        var streak = await UpdateCourseProgressAsync(userId, lesson.CourseId, coursePercent, all.Values, now, ct);

        var justCompleted = !wasCompleted && progress.Completed;
        var unitCompleted = justCompleted && lessons.Where(l => l.UnitId == lesson.UnitId).All(Done);
        var moduleCompleted = justCompleted && lesson.ModuleId.HasValue
            && lessons.Where(l => l.ModuleId == lesson.ModuleId).All(Done);
        var index = lessons.FindIndex(l => l.Id == lesson.Id);
        var next = index >= 0 && index + 1 < lessons.Count ? lessons[index + 1] : null;

        var totalXp = (await curriculum.GetAllProgressForUserAsync(userId, ct)).Sum(p => p.XpEarned);

        return new LessonResultDto(
            lesson.Id,
            score,
            stars,
            progress.Completed,
            isNewBest,
            progress.XpEarned - xpBefore,
            totalXp,
            streak,
            coursePercent,
            next?.Id,
            unitCompleted,
            completedCount == lessons.Count && lessons.Count > 0,
            moduleCompleted);
    }

    /// <summary>Languages the student portal is translated into.</summary>
    private static readonly PreferredLanguage[] PortalLanguages = [PreferredLanguage.English, PreferredLanguage.Punjabi, PreferredLanguage.Hindi, PreferredLanguage.Urdu];

    /// <summary>Saves the language the student wants the portal in, and returns it.</summary>
    public async Task<PreferredLanguage> SetPreferredLanguageAsync(SetLanguageRequest request, CancellationToken ct = default)
    {
        var userId = RequireUser();
        if (!Enum.TryParse<PreferredLanguage>(request.Language?.Trim(), true, out var language)
            || !Enum.IsDefined(language) || !PortalLanguages.Contains(language))
            throw new ValidationDomainException("Choose English, Punjabi, Hindi or Urdu.");

        var user = await users.GetByIdAsync(userId, ct) ?? throw new NotFoundException("User", userId);
        if (user.PreferredLanguage != language)
        {
            user.PreferredLanguage = language;
            await users.UpdateAsync(user, ct);
        }
        return language;
    }

    public async Task<LearnerSummaryDto> GetLearnerSummaryAsync(CancellationToken ct = default)
    {
        var userId = RequireUser();
        var allProgress = await curriculum.GetAllProgressForUserAsync(userId, ct);
        var cards = await GetCatalogueAsync(ct);
        var mine = cards.Where(c => c.IsEnrolled).ToList();

        // Words learned = vocabulary in completed lessons.
        var words = 0;
        foreach (var p in allProgress.Where(p => p.Completed))
        {
            var lesson = await curriculum.GetLessonAsync(p.LessonId, ct);
            if (lesson is not null)
                words += lesson.Blocks.Where(b => b.Type == LessonBlockType.Vocabulary).Sum(b => b.Items.Count);
        }

        var lastActivity = allProgress.Count == 0 ? (DateTime?)null : allProgress.Max(p => p.LastAttemptAtUtc);
        var courseRecords = await courseProgress.GetByUserAsync(userId, ct);
        var streak = lastActivity.HasValue && lastActivity.Value.Date >= DateTime.UtcNow.Date.AddDays(-1)
            ? Math.Max(1, courseRecords.Count == 0 ? 1 : courseRecords.Max(r => r.StreakDays))
            : 0;

        var lastCourseId = allProgress.OrderByDescending(p => p.LastAttemptAtUtc).FirstOrDefault()?.CourseId;
        var continueCourse = mine.FirstOrDefault(c => c.Course.Id == lastCourseId && c.CompletionPercent < 100)
            ?? mine.FirstOrDefault(c => c.CompletionPercent < 100)
            ?? mine.FirstOrDefault();

        return new LearnerSummaryDto(
            allProgress.Sum(p => p.XpEarned),
            streak,
            allProgress.Count(p => p.Completed),
            allProgress.Sum(p => p.Stars),
            words,
            mine,
            continueCourse);
    }

    // ══════════════════════════════════════════════════════════════════════
    //  Helpers
    // ══════════════════════════════════════════════════════════════════════

    private async Task<CourseCardDto> BuildCardAsync(Course c, Guid userId, CancellationToken ct)
    {
        var lessons = Required((await GetPublishedAsync(c.Id, ct)).Lessons);
        var progress = (await curriculum.GetProgressAsync(userId, c.Id, ct)).ToDictionary(p => p.LessonId);
        var completed = lessons.Count(l => progress.TryGetValue(l.Id, out var p) && p.Completed);
        var next = lessons.FirstOrDefault(l => !(progress.TryGetValue(l.Id, out var p) && p.Completed));
        return new CourseCardDto(
            await courseService.GetByIdAsync(c.Id, ct),
            await IsEnrolledAsync(userId, c.Id, ct),
            Percent(completed, lessons.Count),
            completed,
            lessons.Count,
            next?.Id,
            next?.Title);
    }

    private async Task<int> UpdateCourseProgressAsync(
        Guid userId, Guid courseId, int coursePercent, IEnumerable<LessonProgress> lessonProgress, DateTime now, CancellationToken ct)
    {
        var records = await courseProgress.GetByUserAsync(userId, ct);
        var latest = records.Where(r => r.LastActivityAtUtc.HasValue).OrderByDescending(r => r.LastActivityAtUtc).FirstOrDefault();
        var today = now.Date;
        int streak;
        if (latest?.LastActivityAtUtc is { } last && last.Date == today) streak = Math.Max(1, latest.StreakDays);
        else if (latest?.LastActivityAtUtc is { } prev && prev.Date == today.AddDays(-1)) streak = latest.StreakDays + 1;
        else streak = 1;

        var record = records.FirstOrDefault(r => r.CourseId == courseId) ?? new LearningProgress { UserId = userId, CourseId = courseId };
        record.CompletionPercent = coursePercent;
        record.StreakDays = streak;
        record.LastActivityAtUtc = now;
        var scored = lessonProgress.Where(p => p.Attempts > 0).ToList();
        if (scored.Count > 0) record.VocabularyScore = (decimal)scored.Average(p => p.BestScorePercent);
        await courseProgress.UpsertAsync(record, ct);
        return streak;
    }

    /// <summary>Published units and their published lessons, in course order.</summary>
    private async Task<(List<CourseUnit> Units, List<Lesson> Lessons)> GetPublishedAsync(Guid courseId, CancellationToken ct)
    {
        var units = (await curriculum.GetUnitsAsync(courseId, ct)).Where(u => u.Status == LessonStatus.Published).OrderBy(u => u.Order).ToList();
        var unitOrder = units.Select((u, i) => (u.Id, i)).ToDictionary(x => x.Id, x => x.i);
        var lessons = (await curriculum.GetLessonsByCourseAsync(courseId, ct))
            .Where(l => l.Status == LessonStatus.Published && unitOrder.ContainsKey(l.UnitId))
            .OrderBy(l => unitOrder[l.UnitId]).ThenBy(l => l.Order)
            .ToList();
        return (units, lessons);
    }

    /// <summary>A lesson is unlocked when it's the first lesson, or the lesson before it is completed.</summary>
    private static HashSet<Guid> ComputeUnlocked(List<Lesson> orderedLessons, Dictionary<Guid, LessonProgress> progress)
    {
        // A module video that has no video yet ("coming soon") is passed as soon as it is open,
        // so it never blocks the next lesson. Modules open in order as a consequence: the first
        // lesson of a module opens only when the last lesson of the previous module is completed.
        var unlocked = new HashSet<Guid>();
        var previousPassed = true;
        foreach (var lesson in orderedLessons)
        {
            var completed = progress.TryGetValue(lesson.Id, out var self) && self.Completed;
            var open = previousPassed || completed;
            if (open) unlocked.Add(lesson.Id);
            previousPassed = completed || (open && ModuleStructure.IsPendingVideo(lesson));
        }
        return unlocked;
    }

    private async Task<bool> IsEnrolledAsync(Guid userId, Guid courseId, CancellationToken ct)
    {
        var e = await enrollments.GetByUserAndCourseAsync(userId, courseId, ct);
        return e is not null && e.Status == EnrollmentStatus.Active;
    }

    private async Task<LessonDto> ToLessonDtoAsync(Lesson l, LessonProgress? p, CancellationToken ct)
    {
        var unit = await curriculum.GetUnitAsync(l.UnitId, ct);
        var ordered = IsStaff
            ? (await curriculum.GetLessonsByCourseAsync(l.CourseId, ct)).ToList()
            : (await GetPublishedAsync(l.CourseId, ct)).Lessons;
        if (IsStaff)
        {
            var units = (await curriculum.GetUnitsAsync(l.CourseId, ct)).Select((u, i) => (u.Id, u.Order)).ToDictionary(x => x.Id, x => x.Order);
            ordered = ordered.OrderBy(x => units.GetValueOrDefault(x.UnitId)).ThenBy(x => x.Order).ToList();
        }
        var index = ordered.FindIndex(x => x.Id == l.Id);

        // Students skip module videos that aren't ready yet when going back and forth.
        bool Visible(Lesson x) => IsStaff || !ModuleStructure.IsPendingVideo(x);
        Guid? previous = null, next = null;
        for (var i = index - 1; i >= 0; i--) if (Visible(ordered[i])) { previous = ordered[i].Id; break; }
        if (index >= 0) for (var i = index + 1; i < ordered.Count; i++) if (Visible(ordered[i])) { next = ordered[i].Id; break; }

        var module = l.ModuleId is { } moduleId ? await curriculum.GetModuleAsync(moduleId, ct) : null;

        return new LessonDto(
            l.Id, l.CourseId, l.UnitId, unit?.Title ?? string.Empty, l.Order, l.Title, l.TitleIt, l.Kind, l.Summary,
            l.EstimatedMinutes, l.XpReward, l.Status, l.IsFreePreview, l.Blocks, l.Exercises, l.Homework,
            previous,
            next,
            p?.Stars, p?.BestScorePercent, p?.Completed,
            l.ModuleId, module?.Title, l.IsModuleVideo, ModuleStructure.IsPendingVideo(l));
    }

    private static LessonSummaryDto ToSummary(Lesson l, LessonProgress? p = null, bool? locked = null) => new(
        l.Id, l.UnitId, l.Order, l.Title, l.TitleIt, l.Kind, l.Summary, l.EstimatedMinutes, l.XpReward, l.Status,
        l.IsFreePreview,
        l.Blocks.Where(b => b.Type == LessonBlockType.Vocabulary).Sum(b => b.Items.Count),
        l.Exercises.Count,
        l.Blocks.Any(b => b.Type == LessonBlockType.Video && !string.IsNullOrWhiteSpace(b.MediaUrl)),
        l.Blocks.Any(b => b.Type is LessonBlockType.Audio or LessonBlockType.Vocabulary or LessonBlockType.Dialogue or LessonBlockType.Roleplay),
        l.Blocks.Any(b => b.Type == LessonBlockType.Pdf && !string.IsNullOrWhiteSpace(b.MediaUrl)),
        locked is null ? null : p?.Completed ?? false,
        locked is null ? null : p?.Stars ?? 0,
        locked is null ? null : p?.BestScorePercent ?? 0,
        locked,
        l.ModuleId,
        l.IsModuleVideo,
        ModuleStructure.IsPendingVideo(l));

    private static UnitDto ToUnitDto(CourseUnit u, List<LessonSummaryDto> lessons, int completed = 0, int stars = 0, List<ModuleDto>? modules = null) => new(
        u.Id, u.CourseId, u.Order, u.Title, u.TitleIt, u.Description, u.CanDo, u.ImageUrl, u.PdfUrl, u.LiveClassFocus,
        u.Status, lessons, completed, stars, modules ?? []);

    private static void Apply(CourseModule module, SaveModuleRequest r)
    {
        module.Title = r.Title.Trim();
        module.TitleIt = r.TitleIt?.Trim() ?? string.Empty;
        module.Description = r.Description?.Trim() ?? string.Empty;
        module.ImageUrl = Blank(r.ImageUrl);
        if (r.IsTest.HasValue) module.IsTest = r.IsTest.Value;
    }

    private static void Apply(CourseUnit unit, SaveUnitRequest r)
    {
        unit.Title = r.Title.Trim();
        unit.TitleIt = r.TitleIt?.Trim() ?? string.Empty;
        unit.Description = r.Description?.Trim() ?? string.Empty;
        unit.CanDo = (r.CanDo ?? []).Select(s => s.Trim()).Where(s => s.Length > 0).ToList();
        unit.ImageUrl = Blank(r.ImageUrl);
        unit.PdfUrl = Blank(r.PdfUrl);
        unit.LiveClassFocus = Blank(r.LiveClassFocus);
        unit.Status = r.Status;
        if (r.Order is > 0) unit.Order = r.Order.Value;
    }

    private static void Apply(Lesson lesson, SaveLessonRequest r)
    {
        lesson.Title = r.Title.Trim();
        lesson.TitleIt = r.TitleIt?.Trim() ?? string.Empty;
        // The module video is always a video lesson.
        lesson.Kind = lesson.IsModuleVideo ? LessonKind.Video : r.Kind;
        lesson.Summary = r.Summary?.Trim() ?? string.Empty;
        lesson.EstimatedMinutes = Math.Clamp(r.EstimatedMinutes, 1, 180);
        lesson.XpReward = Math.Clamp(r.XpReward, 0, 500);
        lesson.Status = r.Status;
        lesson.IsFreePreview = r.IsFreePreview;
        lesson.Blocks = r.Blocks ?? [];
        lesson.Exercises = r.Exercises ?? [];
        lesson.Homework = Blank(r.Homework);
        if (r.Order is > 0) lesson.Order = r.Order.Value;
        ValidateExercises(lesson.Exercises);
    }

    private static void ValidateExercises(List<LessonExercise> exercises)
    {
        for (var i = 0; i < exercises.Count; i++)
        {
            var e = exercises[i];
            var n = i + 1;
            switch (e.Type)
            {
                case ExerciseType.MatchPairs:
                    if (e.Pairs.Count < 2) throw new ValidationDomainException($"Exercise {n}: add at least 2 pairs to match.");
                    break;
                case ExerciseType.WordOrder:
                    if (string.IsNullOrWhiteSpace(e.Answer)) throw new ValidationDomainException($"Exercise {n}: enter the correct sentence.");
                    break;
                default:
                    if (e.Options.Count < 2) throw new ValidationDomainException($"Exercise {n}: add at least 2 answer options.");
                    if (e.CorrectIndex < 0 || e.CorrectIndex >= e.Options.Count)
                        throw new ValidationDomainException($"Exercise {n}: choose which option is correct.");
                    break;
            }
        }
    }

    private async Task RenumberUnitsAsync(Guid courseId, CancellationToken ct)
    {
        var units = await curriculum.GetUnitsAsync(courseId, ct);
        for (var i = 0; i < units.Count; i++)
        {
            if (units[i].Order == i + 1) continue;
            units[i].Order = i + 1;
            await curriculum.UpdateUnitAsync(units[i], ct);
        }
    }

    private async Task<Course> GetCourseAsync(Guid courseId, CancellationToken ct) =>
        await courses.GetByIdAsync(courseId, ct) is { IsDeleted: false } c ? c : throw new NotFoundException("Course", courseId);

    private void EnsureStaff()
    {
        if (!IsStaff) throw new UnauthorizedAccessDomainException("Teacher or Admin role required.");
    }

    /// <summary>Admins edit everything; teachers edit their own courses and courses without an assigned teacher.</summary>
    private void EnsureCanEdit(Course course)
    {
        EnsureStaff();
        if (current.Role == UserRole.Mentor && course.MentorId.HasValue && course.MentorId != current.UserId)
            throw new UnauthorizedAccessDomainException("Teachers can only edit their own courses.");
    }

    private Guid RequireUser() => current.UserId ?? throw new UnauthorizedAccessDomainException();

    private static void Require(string? value, string field)
    {
        if (string.IsNullOrWhiteSpace(value)) throw new ValidationDomainException($"{field} is required.");
    }

    private static string? Blank(string? s) => string.IsNullOrWhiteSpace(s) ? null : s.Trim();

    private static int Percent(int part, int total) => total == 0 ? 0 : (int)Math.Round(part * 100.0 / total);
}
