using SSeuropa.Application.DTOs;
using SSeuropa.Application.Interfaces;
using SSeuropa.Domain.Entities;
using SSeuropa.Domain.Enums;
using SSeuropa.Domain.Exceptions;

namespace SSeuropa.Application.Services;

/// <summary>
/// Live classes. A class is scheduled for one batch: only the students in that batch see it, can open
/// its meeting link and get its alerts. Teachers see the classes they teach and the classes of batches they lead.
/// </summary>
public class ClassService
{
    private readonly IClassRepository _classes;
    private readonly ICourseRepository _courses;
    private readonly IUserRepository _users;
    private readonly IEnrollmentRepository _enrollments;
    private readonly IBatchRepository _batches;
    private readonly IAuditLogRepository _audit;
    private readonly ICurrentUserService _current;

    private const string DefaultMeetingUrl = "https://meet.google.com/uyj-tfhh-zsq";

    public ClassService(
        IClassRepository classes,
        ICourseRepository courses,
        IUserRepository users,
        IEnrollmentRepository enrollments,
        IBatchRepository batches,
        IAuditLogRepository audit,
        ICurrentUserService current)
    {
        _classes = classes;
        _courses = courses;
        _users = users;
        _enrollments = enrollments;
        _batches = batches;
        _audit = audit;
        _current = current;
    }

    private bool IsTeam => _current.IsInRole(UserRole.Admin, UserRole.SuperAdmin, UserRole.Mentor, UserRole.Staff);

    public async Task<IReadOnlyList<ClassSessionDto>> GetAllAsync(CancellationToken ct = default)
    {
        if (_current.UserId is not { } userId) throw new UnauthorizedAccessDomainException();
        IEnumerable<ClassSession> items = await _classes.GetAllAsync(ct);

        if (_current.Role == UserRole.User)
        {
            // Students only see the classes meant for them (their batches), never other groups' meeting links.
            var me = await _users.GetByIdAsync(userId, ct) ?? throw new UnauthorizedAccessDomainException();
            items = ClassAudience.ForStudent(items, me, await _enrollments.GetByUserAsync(userId, ct))
                .Where(c => c.Status != ClassStatus.Cancelled);
        }
        else if (_current.Role == UserRole.Mentor && _current.ModuleAccess(Common.TeamPermissions.Classes) != Common.AccessLevel.Full)
        {
            var myBatches = (await _batches.GetAllAsync(ct)).Where(b => b.MentorId == userId).Select(b => b.Id).ToHashSet();
            items = items.Where(c => c.MentorId == userId || (c.BatchId is { } b && myBatches.Contains(b)));
        }

        return await MapManyAsync(items.OrderBy(c => c.ScheduledAtUtc), ct);
    }

    public async Task<ClassSessionDto> CreateAsync(CreateClassRequest request, CancellationToken ct = default)
    {
        if (!IsTeam || _current.UserId is not { } userId) throw new UnauthorizedAccessDomainException();

        if (request.BatchId is not { } batchId || batchId == Guid.Empty)
            throw new ValidationDomainException("Choose the batch this class is for.");
        var batch = await _batches.GetByIdAsync(batchId, ct)
            ?? throw new ValidationDomainException("That batch no longer exists.");

        var courseId = Guid.Empty;
        if (request.CourseId is { } requestedCourse && requestedCourse != Guid.Empty)
        {
            var course = await _courses.GetByIdAsync(requestedCourse, ct) ?? throw new NotFoundException("Course", requestedCourse);
            courseId = course.Id;
        }

        // Teacher: the one chosen, else the batch's teacher, else whoever schedules it.
        var mentorId = request.MentorId is { } m && m != Guid.Empty ? m : batch.MentorId ?? userId;
        var mentor = await _users.GetByIdAsync(mentorId, ct);
        if (mentor is null || mentor.IsDeleted || mentor.Role == UserRole.User)
            throw new ValidationDomainException("Choose a teacher from your team.");

        var title = (request.Title ?? "").Trim();
        if (title.Length == 0) title = $"{batch.Name} — live class";

        var session = new ClassSession
        {
            BatchId = batch.Id,
            CourseId = courseId,
            MentorId = mentorId,
            Title = title,
            ScheduledAtUtc = request.ScheduledAtUtc.ToUniversalTime(),
            DurationMinutes = request.DurationMinutes,
            MeetingUrl = !string.IsNullOrWhiteSpace(request.MeetingUrl) ? request.MeetingUrl.Trim() : DefaultMeetingUrl,
            MeetingProvider = "GoogleMeet",
            Status = ClassStatus.Scheduled
        };
        await _classes.AddAsync(session, ct);
        await _audit.AddAsync(new AuditLog
        {
            ActorUserId = userId,
            Action = "class_scheduled",
            Resource = "class",
            Details = $"Scheduled '{session.Title}' for batch '{batch.Name}' at {session.ScheduledAtUtc:yyyy-MM-dd HH:mm} UTC"
        }, ct);
        return await MapAsync(session, ct);
    }

    /// <summary>Cancels a class: it disappears for students and can't be joined. Teachers can cancel only their own classes.</summary>
    public async Task<ClassSessionDto> CancelAsync(Guid classId, CancellationToken ct = default)
    {
        if (!IsTeam || _current.UserId is not { } userId) throw new UnauthorizedAccessDomainException();
        var session = await _classes.GetByIdAsync(classId, ct) ?? throw new NotFoundException("Class", classId);
        if (_current.Role == UserRole.Mentor && session.MentorId != userId && _current.ModuleAccess(Common.TeamPermissions.Classes) != Common.AccessLevel.Full)
            throw new UnauthorizedAccessDomainException("Teachers can cancel only their own classes.");
        if (session.Status == ClassStatus.Completed)
            throw new ValidationDomainException("This class has already taken place.");

        session.Status = ClassStatus.Cancelled;
        session.UpdatedAtUtc = DateTime.UtcNow;
        await _classes.UpdateAsync(session, ct);
        await _audit.AddAsync(new AuditLog { ActorUserId = userId, Action = "class_cancelled", Resource = "class", Details = $"Cancelled '{session.Title}'" }, ct);
        return await MapAsync(session, ct);
    }

    public async Task<ClassSessionDto> GetMeetingAccessAsync(Guid classId, CancellationToken ct = default)
    {
        var session = await _classes.GetByIdAsync(classId, ct) ?? throw new NotFoundException("Class", classId);
        if (_current.UserId is not { } userId) throw new UnauthorizedAccessDomainException();
        if (IsTeam) return await MapAsync(session, ct);

        if (session.Status == ClassStatus.Cancelled)
            throw new ValidationDomainException("This class was cancelled.");
        var me = await _users.GetByIdAsync(userId, ct) ?? throw new UnauthorizedAccessDomainException();
        var active = (await _enrollments.GetByUserAsync(userId, ct))
            .Where(e => e.Status == EnrollmentStatus.Active).Select(e => e.CourseId).ToHashSet();
        if (!ClassAudience.IsFor(session, me, active))
            throw new UnauthorizedAccessDomainException("This class is for another batch.");

        return await MapAsync(session, ct);
    }

    private async Task<ClassSessionDto> MapAsync(ClassSession s, CancellationToken ct) =>
        (await MapManyAsync([s], ct))[0];

    private async Task<List<ClassSessionDto>> MapManyAsync(IEnumerable<ClassSession> sessions, CancellationToken ct)
    {
        var list = sessions.ToList();
        if (list.Count == 0) return [];
        var users = (await _users.GetAllAsync(ct)).ToDictionary(u => u.Id);
        var courses = (await _courses.GetAllAsync(ct)).ToDictionary(c => c.Id);
        var batches = (await _batches.GetAllAsync(ct)).ToDictionary(b => b.Id);
        var studentCounts = users.Values
            .Where(u => u.Role == UserRole.User && !u.IsDeleted)
            .SelectMany(u => u.AllBatchIds())
            .GroupBy(id => id).ToDictionary(g => g.Key, g => g.Count());

        return list.Select(s =>
        {
            var batch = s.BatchId is { } b ? batches.GetValueOrDefault(b) : null;
            return new ClassSessionDto(
                s.Id, s.CourseId, courses.GetValueOrDefault(s.CourseId)?.Title ?? "",
                s.MentorId, users.GetValueOrDefault(s.MentorId)?.FullName ?? "",
                s.Title, s.ScheduledAtUtc, s.DurationMinutes, s.Status, s.MeetingUrl, s.MeetingProvider,
                batch?.Id, batch?.Name, batch is null ? 0 : studentCounts.GetValueOrDefault(batch.Id));
        }).ToList();
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
        foreach (var m in items.OrderBy(x => x.ChapterNumber).ThenBy(x => x.SortOrder))
        {
            string? url = m.VideoUrl ?? m.AudioUrl ?? m.PdfUrl;
            if (string.IsNullOrEmpty(url) && m.IsProtected && !string.IsNullOrEmpty(m.StorageKey))
                url = await _storage.GetProtectedAccessUrlAsync(m.StorageKey, TimeSpan.FromMinutes(15), ct);
            result.Add(new MaterialDto(
                m.Id, m.CourseId, m.Title, m.Description, m.Type, m.IsProtected, m.SortOrder, url,
                m.ChapterTitle, m.ChapterNumber, m.VideoUrl, m.AudioUrl, m.PdfUrl, m.ContentText, m.PracticalTask, m.DurationMinutes));
        }
        return result;
    }

    public async Task<MaterialDto> CreateAsync(CreateMaterialRequest request, CancellationToken ct = default)
    {
        if (!_current.IsInRole(UserRole.Admin, UserRole.SuperAdmin, UserRole.Mentor, UserRole.Staff))
            throw new UnauthorizedAccessDomainException();

        var material = new Material
        {
            CourseId = request.CourseId,
            Title = request.Title.Trim(),
            Description = request.Description.Trim(),
            Type = request.Type,
            SortOrder = request.SortOrder,
            ChapterTitle = string.IsNullOrWhiteSpace(request.ChapterTitle) ? "Chapter 1: Getting Started" : request.ChapterTitle.Trim(),
            ChapterNumber = request.ChapterNumber > 0 ? request.ChapterNumber : 1,
            VideoUrl = request.VideoUrl,
            AudioUrl = request.AudioUrl,
            PdfUrl = request.PdfUrl,
            ContentText = request.ContentText,
            PracticalTask = request.PracticalTask,
            DurationMinutes = request.DurationMinutes > 0 ? request.DurationMinutes : 15,
            StorageKey = request.VideoUrl ?? request.AudioUrl ?? request.PdfUrl ?? $"uploads/{request.CourseId}/{Guid.NewGuid():N}",
            IsProtected = true
        };
        await _materials.AddAsync(material, ct);
        var url = material.VideoUrl ?? material.AudioUrl ?? material.PdfUrl ?? await _storage.GetProtectedAccessUrlAsync(material.StorageKey, TimeSpan.FromMinutes(15), ct);
        return new MaterialDto(
            material.Id, material.CourseId, material.Title, material.Description, material.Type, material.IsProtected, material.SortOrder, url,
            material.ChapterTitle, material.ChapterNumber, material.VideoUrl, material.AudioUrl, material.PdfUrl, material.ContentText, material.PracticalTask, material.DurationMinutes);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        if (!_current.IsInRole(UserRole.Admin, UserRole.SuperAdmin, UserRole.Mentor, UserRole.Staff))
            throw new UnauthorizedAccessDomainException();
        await _materials.DeleteAsync(id, ct);
    }

    private async Task EnsureCourseAccessAsync(Guid courseId, CancellationToken ct)
    {
        if (_current.IsInRole(UserRole.Admin, UserRole.SuperAdmin, UserRole.Mentor, UserRole.Staff)) return;
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
            if (!_current.IsInRole(UserRole.Admin, UserRole.SuperAdmin, UserRole.Mentor, UserRole.Staff))
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
        if (!_current.IsInRole(UserRole.Mentor, UserRole.Admin, UserRole.SuperAdmin, UserRole.Staff))
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

public class AssignmentService
{
    private readonly IAssignmentRepository _assignments;
    private readonly ICourseRepository _courses;
    private readonly IEnrollmentRepository _enrollments;
    private readonly IUserRepository _users;
    private readonly IEmailService _emailService;
    private readonly ICurrentUserService _current;

    public AssignmentService(
        IAssignmentRepository assignments,
        ICourseRepository courses,
        IEnrollmentRepository enrollments,
        IUserRepository users,
        IEmailService emailService,
        ICurrentUserService current)
    {
        _assignments = assignments;
        _courses = courses;
        _enrollments = enrollments;
        _users = users;
        _emailService = emailService;
        _current = current;
    }

    public async Task<IReadOnlyList<AssignmentDto>> GetByCourseAsync(Guid courseId, CancellationToken ct = default)
    {
        var items = await _assignments.GetByCourseAsync(courseId, ct);
        return items.Select(Map).ToList();
    }

    public async Task<AssignmentDto> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var item = await _assignments.GetByIdAsync(id, ct) ?? throw new NotFoundException("Assignment", id);
        return Map(item);
    }

    public async Task<AssignmentDto> CreateAsync(CreateAssignmentRequest request, CancellationToken ct = default)
    {
        if (!_current.IsInRole(UserRole.Admin, UserRole.SuperAdmin, UserRole.Mentor, UserRole.Staff))
            throw new UnauthorizedAccessDomainException();

        _ = await _courses.GetByIdAsync(request.CourseId, ct) ?? throw new NotFoundException("Course", request.CourseId);

        var assignment = new Assignment
        {
            CourseId = request.CourseId,
            ChapterTitle = string.IsNullOrWhiteSpace(request.ChapterTitle) ? "Chapter 1: Practical Assignments" : request.ChapterTitle.Trim(),
            Title = request.Title.Trim(),
            Description = request.Description.Trim(),
            ReferenceFileUrl = request.ReferenceFileUrl,
            DueDateUtc = request.DueDateUtc,
            TotalMarks = request.TotalMarks > 0 ? request.TotalMarks : 100,
            IsActive = true
        };

        await _assignments.AddAsync(assignment, ct);
        return Map(assignment);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        if (!_current.IsInRole(UserRole.Admin, UserRole.SuperAdmin, UserRole.Mentor, UserRole.Staff))
            throw new UnauthorizedAccessDomainException();
        await _assignments.DeleteAsync(id, ct);
    }

    public async Task<AssignmentSubmissionDto> SubmitAsync(Guid assignmentId, SubmitAssignmentRequest request, CancellationToken ct = default)
    {
        if (_current.UserId is null) throw new UnauthorizedAccessDomainException();
        var assignment = await _assignments.GetByIdAsync(assignmentId, ct)
            ?? throw new NotFoundException("Assignment", assignmentId);

        var user = await _users.GetByIdAsync(_current.UserId.Value, ct)
            ?? throw new NotFoundException("User", _current.UserId.Value);

        var existing = await _assignments.GetStudentSubmissionForAssignmentAsync(assignmentId, _current.UserId.Value, ct);
        if (existing is not null)
        {
            existing.SubmissionText = request.SubmissionText;
            existing.SubmissionFileUrl = request.SubmissionFileUrl;
            existing.SubmittedAtUtc = DateTime.UtcNow;
            existing.Status = "Submitted";
            await _assignments.UpdateSubmissionAsync(existing, ct);
            return MapSubmission(existing);
        }

        var submission = new AssignmentSubmission
        {
            AssignmentId = assignmentId,
            StudentId = user.Id,
            StudentName = user.FullName,
            StudentEmail = user.Email,
            SubmissionText = request.SubmissionText,
            SubmissionFileUrl = request.SubmissionFileUrl,
            SubmittedAtUtc = DateTime.UtcNow,
            Status = "Submitted"
        };

        await _assignments.AddSubmissionAsync(submission, ct);
        return MapSubmission(submission);
    }

    public async Task<IReadOnlyList<AssignmentSubmissionDto>> GetSubmissionsForAssignmentAsync(Guid assignmentId, CancellationToken ct = default)
    {
        if (!_current.IsInRole(UserRole.Admin, UserRole.SuperAdmin, UserRole.Mentor, UserRole.Staff))
            throw new UnauthorizedAccessDomainException();

        var items = await _assignments.GetSubmissionsByAssignmentAsync(assignmentId, ct);
        return items.Select(MapSubmission).ToList();
    }

    public async Task<IReadOnlyList<AssignmentSubmissionDto>> GetMySubmissionsAsync(CancellationToken ct = default)
    {
        if (_current.UserId is null) throw new UnauthorizedAccessDomainException();
        var items = await _assignments.GetSubmissionsByStudentAsync(_current.UserId.Value, ct);
        return items.Select(MapSubmission).ToList();
    }

    public async Task<AssignmentSubmissionDto> GradeSubmissionAsync(Guid submissionId, GradeAssignmentRequest request, CancellationToken ct = default)
    {
        if (!_current.IsInRole(UserRole.Admin, UserRole.SuperAdmin, UserRole.Mentor, UserRole.Staff))
            throw new UnauthorizedAccessDomainException();

        var submission = await _assignments.GetSubmissionByIdAsync(submissionId, ct)
            ?? throw new NotFoundException("Submission", submissionId);

        var assignment = await _assignments.GetByIdAsync(submission.AssignmentId, ct);

        submission.MarksObtained = request.MarksObtained;
        submission.TeacherFeedback = request.TeacherFeedback;
        submission.GradedAtUtc = DateTime.UtcNow;
        submission.GradedByMentorId = _current.UserId;
        submission.Status = "Graded";

        await _assignments.UpdateSubmissionAsync(submission, ct);

        // Notify student via email
        if (!string.IsNullOrWhiteSpace(submission.StudentEmail) && assignment is not null)
        {
            await _emailService.SendAssignmentFeedbackAsync(
                submission.StudentEmail,
                submission.StudentName,
                assignment.Title,
                request.MarksObtained,
                request.TeacherFeedback,
                ct);
        }

        return MapSubmission(submission);
    }

    private static AssignmentDto Map(Assignment a) => new(
        a.Id, a.CourseId, a.ChapterTitle, a.Title, a.Description,
        a.ReferenceFileUrl, a.DueDateUtc, a.TotalMarks, a.IsActive, a.CreatedAtUtc);

    private static AssignmentSubmissionDto MapSubmission(AssignmentSubmission s) => new(
        s.Id, s.AssignmentId, s.StudentId, s.StudentName, s.StudentEmail,
        s.SubmissionText, s.SubmissionFileUrl, s.SubmittedAtUtc,
        s.MarksObtained, s.TeacherFeedback, s.GradedAtUtc, s.Status);
}

