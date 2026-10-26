using SSeuropa.Application.DTOs;
using SSeuropa.Application.Interfaces;
using SSeuropa.Domain.Entities;
using SSeuropa.Domain.Enums;
using SSeuropa.Domain.Exceptions;

namespace SSeuropa.Application.Services;

public class ClassService
{
    private readonly IClassRepository _classes;
    private readonly ICourseRepository _courses;
    private readonly IUserRepository _users;
    private readonly IEnrollmentRepository _enrollments;
    private readonly ICurrentUserService _current;

    public ClassService(
        IClassRepository classes,
        ICourseRepository courses,
        IUserRepository users,
        IEnrollmentRepository enrollments,
        ICurrentUserService current)
    {
        _classes = classes;
        _courses = courses;
        _users = users;
        _enrollments = enrollments;
        _current = current;
    }

    public async Task<IReadOnlyList<ClassSessionDto>> GetAllAsync(CancellationToken ct = default)
    {
        var items = await _classes.GetAllAsync(ct);
        if (_current.Role == UserRole.Mentor && _current.UserId.HasValue)
            items = items.Where(c => c.MentorId == _current.UserId).ToList();

        var result = new List<ClassSessionDto>();
        foreach (var item in items.OrderBy(c => c.ScheduledAtUtc))
            result.Add(await MapAsync(item, ct));
        return result;
    }

    public async Task<ClassSessionDto> CreateAsync(CreateClassRequest request, CancellationToken ct = default)
    {
        if (!_current.IsInRole(UserRole.Admin, UserRole.SuperAdmin, UserRole.Mentor))
            throw new UnauthorizedAccessDomainException();

        _ = await _courses.GetByIdAsync(request.CourseId, ct) ?? throw new NotFoundException("Course", request.CourseId);
        var session = new ClassSession
        {
            CourseId = request.CourseId,
            MentorId = request.MentorId,
            Title = request.Title.Trim(),
            ScheduledAtUtc = request.ScheduledAtUtc.ToUniversalTime(),
            DurationMinutes = request.DurationMinutes,
            MeetingUrl = $"https://meet.google.com/mock-{Guid.NewGuid():N}"[..35],
            MeetingProvider = "GoogleMeet",
            Status = ClassStatus.Scheduled
        };
        await _classes.AddAsync(session, ct);
        return await MapAsync(session, ct);
    }

    public async Task<ClassSessionDto> GetMeetingAccessAsync(Guid classId, CancellationToken ct = default)
    {
        var session = await _classes.GetByIdAsync(classId, ct) ?? throw new NotFoundException("Class", classId);
        if (_current.UserId is null) throw new UnauthorizedAccessDomainException();

        if (_current.IsInRole(UserRole.Admin, UserRole.SuperAdmin) ||
            (_current.Role == UserRole.Mentor && session.MentorId == _current.UserId))
            return await MapAsync(session, ct);

        var enrollment = await _enrollments.GetByUserAndCourseAsync(_current.UserId.Value, session.CourseId, ct);
        if (enrollment is null || enrollment.Status != EnrollmentStatus.Active)
            throw new UnauthorizedAccessDomainException("Enrollment required for meeting access.");

        return await MapAsync(session, ct);
    }

    private async Task<ClassSessionDto> MapAsync(ClassSession s, CancellationToken ct)
    {
        var course = await _courses.GetByIdAsync(s.CourseId, ct);
        var mentor = await _users.GetByIdAsync(s.MentorId, ct);
        return new ClassSessionDto(
            s.Id, s.CourseId, course?.Title ?? "", s.MentorId, mentor?.FullName ?? "",
            s.Title, s.ScheduledAtUtc, s.DurationMinutes, s.Status, s.MeetingUrl, s.MeetingProvider);
    }
}

public class MaterialService
{
    private readonly IMaterialRepository _materials;
    private readonly IEnrollmentRepository _enrollments;
    private readonly IStorageService _storage;
    private readonly ICurrentUserService _current;

    public MaterialService(
        IMaterialRepository materials,
        IEnrollmentRepository enrollments,
        IStorageService storage,
        ICurrentUserService current)
    {
        _materials = materials;
        _enrollments = enrollments;
        _storage = storage;
        _current = current;
    }

    public async Task<IReadOnlyList<MaterialDto>> GetByCourseAsync(Guid courseId, CancellationToken ct = default)
    {
        await EnsureCourseAccessAsync(courseId, ct);
        var items = await _materials.GetByCourseAsync(courseId, ct);
        var result = new List<MaterialDto>();
        foreach (var m in items.OrderBy(x => x.SortOrder))
        {
            string? url = null;
            if (m.IsProtected)
                url = await _storage.GetProtectedAccessUrlAsync(m.StorageKey, TimeSpan.FromMinutes(15), ct);
            result.Add(new MaterialDto(m.Id, m.CourseId, m.Title, m.Description, m.Type, m.IsProtected, m.SortOrder, url));
        }
        return result;
    }

    public async Task<MaterialDto> CreateAsync(CreateMaterialRequest request, CancellationToken ct = default)
    {
        if (!_current.IsInRole(UserRole.Admin, UserRole.SuperAdmin, UserRole.Mentor))
            throw new UnauthorizedAccessDomainException();

        var material = new Material
        {
            CourseId = request.CourseId,
            Title = request.Title.Trim(),
            Description = request.Description.Trim(),
            Type = request.Type,
            SortOrder = request.SortOrder,
            StorageKey = $"mock/{request.CourseId}/{Guid.NewGuid():N}",
            IsProtected = true
        };
        await _materials.AddAsync(material, ct);
        var url = await _storage.GetProtectedAccessUrlAsync(material.StorageKey, TimeSpan.FromMinutes(15), ct);
        return new MaterialDto(material.Id, material.CourseId, material.Title, material.Description, material.Type, material.IsProtected, material.SortOrder, url);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        if (!_current.IsInRole(UserRole.Admin, UserRole.SuperAdmin, UserRole.Mentor))
            throw new UnauthorizedAccessDomainException();
        await _materials.DeleteAsync(id, ct);
    }

    private async Task EnsureCourseAccessAsync(Guid courseId, CancellationToken ct)
    {
        if (_current.IsInRole(UserRole.Admin, UserRole.SuperAdmin, UserRole.Mentor)) return;
        if (_current.UserId is null) throw new UnauthorizedAccessDomainException();
        var enrollment = await _enrollments.GetByUserAndCourseAsync(_current.UserId.Value, courseId, ct);
        if (enrollment is null || enrollment.Status != EnrollmentStatus.Active)
            throw new UnauthorizedAccessDomainException("Enrollment required.");
    }
}

public class AssessmentService
{
    private readonly IAssessmentRepository _assessments;
    private readonly IEnrollmentRepository _enrollments;
    private readonly ICurrentUserService _current;

    public AssessmentService(IAssessmentRepository assessments, IEnrollmentRepository enrollments, ICurrentUserService current)
    {
        _assessments = assessments;
        _enrollments = enrollments;
        _current = current;
    }

    public async Task<IReadOnlyList<AssessmentDto>> GetByCourseAsync(Guid courseId, CancellationToken ct = default)
    {
        var items = await _assessments.GetByCourseAsync(courseId, ct);
        return items.Select(Map).ToList();
    }

    public async Task<IReadOnlyList<AssessmentDto>> GetAllAsync(CancellationToken ct = default)
    {
        var items = await _assessments.GetAllAsync(ct);
        return items.Select(Map).ToList();
    }

    public async Task<AssessmentDto> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var item = await _assessments.GetByIdAsync(id, ct) ?? throw new NotFoundException("Assessment", id);
        return Map(item);
    }

    public async Task<TestAttemptDto> SubmitAsync(Guid assessmentId, SubmitAttemptRequest request, CancellationToken ct = default)
    {
        if (_current.UserId is null) throw new UnauthorizedAccessDomainException();
        var assessment = await _assessments.GetByIdAsync(assessmentId, ct)
            ?? throw new NotFoundException("Assessment", assessmentId);

        var enrollment = await _enrollments.GetByUserAndCourseAsync(_current.UserId.Value, assessment.CourseId, ct);
        if (enrollment is null || enrollment.Status != EnrollmentStatus.Active)
        {
            if (!_current.IsInRole(UserRole.Admin, UserRole.SuperAdmin, UserRole.Mentor))
                throw new UnauthorizedAccessDomainException("Enrollment required.");
        }

        if (request.SelectedAnswers.Count != assessment.Questions.Count)
            throw new ValidationDomainException("Answer count must match question count.");

        var score = 0;
        var max = 0;
        for (var i = 0; i < assessment.Questions.Count; i++)
        {
            var q = assessment.Questions[i];
            max += q.Points;
            if (request.SelectedAnswers[i] == q.CorrectOptionIndex)
                score += q.Points;
        }

        var attempt = new TestAttempt
        {
            AssessmentId = assessmentId,
            UserId = _current.UserId.Value,
            Score = score,
            MaxScore = max,
            Passed = max == 0 ? false : (score * 100 / max) >= assessment.PassingScore,
            CompletedAtUtc = DateTime.UtcNow,
            SelectedAnswers = request.SelectedAnswers.ToList()
        };
        await _assessments.AddAttemptAsync(attempt, ct);
        return new TestAttemptDto(attempt.Id, attempt.AssessmentId, attempt.UserId, attempt.Score, attempt.MaxScore, attempt.Passed, attempt.StartedAtUtc, attempt.CompletedAtUtc);
    }

    public async Task<IReadOnlyList<TestAttemptDto>> GetMyAttemptsAsync(CancellationToken ct = default)
    {
        if (_current.UserId is null) throw new UnauthorizedAccessDomainException();
        var items = await _assessments.GetAttemptsByUserAsync(_current.UserId.Value, ct);
        return items.Select(a => new TestAttemptDto(a.Id, a.AssessmentId, a.UserId, a.Score, a.MaxScore, a.Passed, a.StartedAtUtc, a.CompletedAtUtc)).ToList();
    }

    private static AssessmentDto Map(Assessment a) => new(
        a.Id, a.CourseId, a.Title, a.Description, a.PassingScore, a.TimeLimitMinutes,
        a.Questions.Select(q => new QuestionDto(q.Id, q.Prompt, q.Options, q.Points)).ToList());
}

public class ProgressService
{
    private readonly IProgressRepository _progress;
    private readonly ICourseRepository _courses;
    private readonly IAttendanceRepository _attendance;
    private readonly IUserRepository _users;
    private readonly IClassRepository _classes;
    private readonly ICurrentUserService _current;

    public ProgressService(
        IProgressRepository progress,
        ICourseRepository courses,
        IAttendanceRepository attendance,
        IUserRepository users,
        IClassRepository classes,
        ICurrentUserService current)
    {
        _progress = progress;
        _courses = courses;
        _attendance = attendance;
        _users = users;
        _classes = classes;
        _current = current;
    }

    public async Task<IReadOnlyList<ProgressDto>> GetMyProgressAsync(CancellationToken ct = default)
    {
        if (_current.UserId is null) throw new UnauthorizedAccessDomainException();
        var items = await _progress.GetByUserAsync(_current.UserId.Value, ct);
        var result = new List<ProgressDto>();
        foreach (var p in items)
        {
            var course = await _courses.GetByIdAsync(p.CourseId, ct);
            result.Add(new ProgressDto(p.Id, p.UserId, p.CourseId, course?.Title ?? "", p.CompletionPercent, p.StreakDays, p.GrammarScore, p.VocabularyScore, p.SpeakingScore, p.LastActivityAtUtc));
        }
        return result;
    }

    public async Task<AttendanceDto> MarkAttendanceAsync(MarkAttendanceRequest request, CancellationToken ct = default)
    {
        if (!_current.IsInRole(UserRole.Mentor, UserRole.Admin, UserRole.SuperAdmin))
            throw new UnauthorizedAccessDomainException();

        _ = await _classes.GetByIdAsync(request.ClassSessionId, ct) ?? throw new NotFoundException("Class", request.ClassSessionId);
        var user = await _users.GetByIdAsync(request.UserId, ct) ?? throw new NotFoundException("User", request.UserId);

        var record = new AttendanceRecord
        {
            ClassSessionId = request.ClassSessionId,
            UserId = request.UserId,
            Present = request.Present,
            MarkedAtUtc = DateTime.UtcNow
        };
        await _attendance.UpsertAsync(record, ct);
        return new AttendanceDto(record.Id, record.ClassSessionId, record.UserId, user.FullName, record.Present, record.MarkedAtUtc);
    }

    public async Task<IReadOnlyList<AttendanceDto>> GetAttendanceAsync(Guid classSessionId, CancellationToken ct = default)
    {
        var items = await _attendance.GetByClassAsync(classSessionId, ct);
        var result = new List<AttendanceDto>();
        foreach (var a in items)
        {
            var user = await _users.GetByIdAsync(a.UserId, ct);
            result.Add(new AttendanceDto(a.Id, a.ClassSessionId, a.UserId, user?.FullName ?? "", a.Present, a.MarkedAtUtc));
        }
        return result;
    }
}
