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
    CourseService courseService)
{
    // Stars from a lesson score. 50% is the pass mark.
    public static int StarsFor(int scorePercent) => scorePercent switch
    {
        >= 90 => 3,
        >= 70 => 2,
        >= 50 => 1,
        _ => 0
    };

    private bool IsStaff => current.IsInRole(UserRole.Mentor, UserRole.Admin, UserRole.SuperAdmin);

    // ══════════════════════════════════════════════════════════════════════
    //  Staff: outline & editing
    // ══════════════════════════════════════════════════════════════════════

    public async Task<CourseOutlineDto> GetStaffOutlineAsync(Guid courseId, CancellationToken ct = default)
    {
        EnsureStaff();
        var course = await GetCourseAsync(courseId, ct);
        var units = await curriculum.GetUnitsAsync(courseId, ct);
        var lessons = await curriculum.GetLessonsByCourseAsync(courseId, ct);

        var unitDtos = units.Select(u => ToUnitDto(u, lessons.Where(l => l.UnitId == u.Id).OrderBy(l => l.Order)
            .Select(l => ToSummary(l)).ToList())).ToList();

        return new CourseOutlineDto(
            await courseService.GetByIdAsync(course.Id, ct), unitDtos, lessons.Count, 0, 0, 0, lessons.Count * 3, 0, false, null);
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
        return ToUnitDto(unit, []);
    }

    public async Task<UnitDto> UpdateUnitAsync(Guid unitId, SaveUnitRequest request, CancellationToken ct = default)
    {
        var unit = await curriculum.GetUnitAsync(unitId, ct) ?? throw new NotFoundException("Unit", unitId);
        EnsureCanEdit(await GetCourseAsync(unit.CourseId, ct));
        Require(request.Title, "Unit title");

        Apply(unit, request);
        await curriculum.UpdateUnitAsync(unit, ct);
        var lessons = await curriculum.GetLessonsByUnitAsync(unit.Id, ct);
        return ToUnitDto(unit, lessons.Select(l => ToSummary(l)).ToList());
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

        var existing = await curriculum.GetLessonsByUnitAsync(unitId, ct);
        var lesson = new Lesson
        {
            CourseId = unit.CourseId,
            UnitId = unit.Id,
            Order = existing.Count == 0 ? 1 : existing.Max(l => l.Order) + 1
        };
        Apply(lesson, request);
        await curriculum.AddLessonAsync(lesson, ct);
        return await ToLessonDtoAsync(lesson, null, ct);
    }

    public async Task<LessonDto> UpdateLessonAsync(Guid lessonId, SaveLessonRequest request, CancellationToken ct = default)
    {
        var lesson = await curriculum.GetLessonAsync(lessonId, ct) ?? throw new NotFoundException("Lesson", lessonId);
        EnsureCanEdit(await GetCourseAsync(lesson.CourseId, ct));
        Require(request.Title, "Lesson title");

        if (request.UnitId.HasValue && request.UnitId.Value != lesson.UnitId)
        {
            var newUnit = await curriculum.GetUnitAsync(request.UnitId.Value, ct) ?? throw new NotFoundException("Unit", request.UnitId.Value);
            if (newUnit.CourseId != lesson.CourseId)
                throw new ValidationDomainException("A lesson can only move to a unit of the same course.");
            var siblings = await curriculum.GetLessonsByUnitAsync(newUnit.Id, ct);
            lesson.UnitId = newUnit.Id;
            lesson.Order = siblings.Count == 0 ? 1 : siblings.Max(l => l.Order) + 1;
        }

        Apply(lesson, request);
        await curriculum.UpdateLessonAsync(lesson, ct);
        return await ToLessonDtoAsync(lesson, null, ct);
    }

    public async Task DeleteLessonAsync(Guid lessonId, CancellationToken ct = default)
    {
        var lesson = await curriculum.GetLessonAsync(lessonId, ct) ?? throw new NotFoundException("Lesson", lessonId);
        EnsureCanEdit(await GetCourseAsync(lesson.CourseId, ct));
        await curriculum.DeleteLessonAsync(lessonId, ct);
        await RenumberLessonsAsync(lesson.UnitId, ct);
    }

    public async Task MoveLessonAsync(Guid lessonId, int direction, CancellationToken ct = default)
    {
        var lesson = await curriculum.GetLessonAsync(lessonId, ct) ?? throw new NotFoundException("Lesson", lessonId);
        EnsureCanEdit(await GetCourseAsync(lesson.CourseId, ct));
        var lessons = (await curriculum.GetLessonsByUnitAsync(lesson.UnitId, ct)).ToList();
        var index = lessons.FindIndex(l => l.Id == lessonId);
        var target = index + Math.Sign(direction);
        if (target < 0 || target >= lessons.Count) return;
        (lessons[index], lessons[target]) = (lessons[target], lessons[index]);
        for (var i = 0; i < lessons.Count; i++)
        {
            if (lessons[i].Order == i + 1) continue;
            lessons[i].Order = i + 1;
            await curriculum.UpdateLessonAsync(lessons[i], ct);
        }
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
        var totalLessons = (await curriculum.GetLessonsByCourseAsync(courseId, ct)).Count(l => l.Status == LessonStatus.Published);
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
        var all = (await courses.GetAllAsync(ct))
            .Where(c => c.Status == CourseStatus.Published && !c.IsDeleted)
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

        var isEnrolled = await IsEnrolledAsync(userId, courseId, ct);
        var (units, lessons) = await GetPublishedAsync(courseId, ct);
        var progress = (await curriculum.GetProgressAsync(userId, courseId, ct)).ToDictionary(p => p.LessonId);
        var unlocked = ComputeUnlocked(lessons, progress);

        var unitDtos = units.Select(u =>
        {
            var unitLessons = lessons.Where(l => l.UnitId == u.Id).ToList();
            var summaries = unitLessons.Select(l =>
            {
                progress.TryGetValue(l.Id, out var p);
                var open = IsStaff || (isEnrolled && unlocked.Contains(l.Id)) || l.IsFreePreview;
                return ToSummary(l, p, locked: !open);
            }).ToList();
            return ToUnitDto(u, summaries,
                completed: unitLessons.Count(l => progress.TryGetValue(l.Id, out var p) && p.Completed),
                stars: unitLessons.Sum(l => progress.TryGetValue(l.Id, out var p) ? p.Stars : 0));
        }).ToList();

        var completedCount = lessons.Count(l => progress.TryGetValue(l.Id, out var p) && p.Completed);
        var next = lessons.FirstOrDefault(l => !(progress.TryGetValue(l.Id, out var p) && p.Completed));

        return new CourseOutlineDto(
            await courseService.GetByIdAsync(courseId, ct),
            unitDtos,
            lessons.Count,
            completedCount,
            Percent(completedCount, lessons.Count),
            progress.Values.Sum(p => p.Stars),
            lessons.Count * 3,
            progress.Values.Sum(p => p.XpEarned),
            isEnrolled,
            next?.Id);
    }

    public async Task<LessonDto> GetLessonAsync(Guid lessonId, CancellationToken ct = default)
    {
        var userId = RequireUser();
        var lesson = await curriculum.GetLessonAsync(lessonId, ct) ?? throw new NotFoundException("Lesson", lessonId);

        if (IsStaff)
            return await ToLessonDtoAsync(lesson, await curriculum.GetLessonProgressAsync(userId, lessonId, ct), ct);

        var course = await GetCourseAsync(lesson.CourseId, ct);
        if (course.Status != CourseStatus.Published || lesson.Status != LessonStatus.Published)
            throw new NotFoundException("Lesson", lessonId);

        if (!lesson.IsFreePreview)
        {
            if (!await IsEnrolledAsync(userId, lesson.CourseId, ct))
                throw new UnauthorizedAccessDomainException("Enroll in this course to open this lesson.");

            var (_, lessons) = await GetPublishedAsync(lesson.CourseId, ct);
            var progress = (await curriculum.GetProgressAsync(userId, lesson.CourseId, ct)).ToDictionary(p => p.LessonId);
            if (!ComputeUnlocked(lessons, progress).Contains(lessonId))
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

        var score = Math.Clamp(request.ScorePercent, 0, 100);
        // Lessons without exercises (e.g. a video) count as fully completed.
        if (lesson.Exercises.Count == 0) score = 100;
        var stars = StarsFor(score);
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
        var (units, lessons) = await GetPublishedAsync(lesson.CourseId, ct);
        var all = (await curriculum.GetProgressAsync(userId, lesson.CourseId, ct)).ToDictionary(p => p.LessonId);
        var completedCount = lessons.Count(l => all.TryGetValue(l.Id, out var p) && p.Completed);
        var coursePercent = Percent(completedCount, lessons.Count);
        var streak = await UpdateCourseProgressAsync(userId, lesson.CourseId, coursePercent, all.Values, now, ct);

        var unitLessons = lessons.Where(l => l.UnitId == lesson.UnitId).ToList();
        var unitCompleted = !wasCompleted && progress.Completed
            && unitLessons.All(l => all.TryGetValue(l.Id, out var p) && p.Completed);
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
            completedCount == lessons.Count && lessons.Count > 0);
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
        var (_, lessons) = await GetPublishedAsync(c.Id, ct);
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
        var unlocked = new HashSet<Guid>();
        for (var i = 0; i < orderedLessons.Count; i++)
        {
            var open = i == 0
                || (progress.TryGetValue(orderedLessons[i - 1].Id, out var prev) && prev.Completed)
                || (progress.TryGetValue(orderedLessons[i].Id, out var self) && self.Completed);
            if (open) unlocked.Add(orderedLessons[i].Id);
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

        return new LessonDto(
            l.Id, l.CourseId, l.UnitId, unit?.Title ?? string.Empty, l.Order, l.Title, l.TitleIt, l.Kind, l.Summary,
            l.EstimatedMinutes, l.XpReward, l.Status, l.IsFreePreview, l.Blocks, l.Exercises, l.Homework,
            index > 0 ? ordered[index - 1].Id : null,
            index >= 0 && index + 1 < ordered.Count ? ordered[index + 1].Id : null,
            p?.Stars, p?.BestScorePercent, p?.Completed);
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
        locked);

    private static UnitDto ToUnitDto(CourseUnit u, List<LessonSummaryDto> lessons, int completed = 0, int stars = 0) => new(
        u.Id, u.CourseId, u.Order, u.Title, u.TitleIt, u.Description, u.CanDo, u.ImageUrl, u.PdfUrl, u.LiveClassFocus,
        u.Status, lessons, completed, stars);

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
        lesson.Kind = r.Kind;
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

    private async Task RenumberLessonsAsync(Guid unitId, CancellationToken ct)
    {
        var lessons = await curriculum.GetLessonsByUnitAsync(unitId, ct);
        for (var i = 0; i < lessons.Count; i++)
        {
            if (lessons[i].Order == i + 1) continue;
            lessons[i].Order = i + 1;
            await curriculum.UpdateLessonAsync(lessons[i], ct);
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
