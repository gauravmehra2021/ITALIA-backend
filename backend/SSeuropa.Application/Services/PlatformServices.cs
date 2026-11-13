using SSeuropa.Application.DTOs;
using SSeuropa.Application.Interfaces;
using SSeuropa.Domain.Entities;
using SSeuropa.Domain.Enums;
using SSeuropa.Domain.Exceptions;

namespace SSeuropa.Application.Services;

public class SubscriptionService
{
    private readonly ISubscriptionRepository _subscriptions;
    private readonly IPaymentRepository _payments;
    private readonly ICourseRepository _courses;
    private readonly IEnrollmentRepository _enrollments;
    private readonly IPaymentGateway _paymentsGateway;
    private readonly ICurrentUserService _current;
    private readonly CourseService _courseService;

    public SubscriptionService(
        ISubscriptionRepository subscriptions,
        IPaymentRepository payments,
        ICourseRepository courses,
        IEnrollmentRepository enrollments,
        IPaymentGateway paymentsGateway,
        ICurrentUserService current,
        CourseService courseService)
    {
        _subscriptions = subscriptions;
        _payments = payments;
        _courses = courses;
        _enrollments = enrollments;
        _paymentsGateway = paymentsGateway;
        _current = current;
        _courseService = courseService;
    }

    public async Task<SubscriptionDto> CreateAsync(CreateSubscriptionRequest request, CancellationToken ct = default)
    {
        if (_current.UserId is null) throw new UnauthorizedAccessDomainException();
        var course = await _courses.GetByIdAsync(request.CourseId, ct) ?? throw new NotFoundException("Course", request.CourseId);

        var (reference, success) = await _paymentsGateway.CreateSubscriptionAsync(
            _current.UserId.Value, request.CourseId, request.PlanName, course.Price, ct);

        var subscription = new Subscription
        {
            UserId = _current.UserId.Value,
            CourseId = request.CourseId,
            PlanName = request.PlanName,
            Status = success ? SubscriptionStatus.Active : SubscriptionStatus.Pending,
            PayPalSubscriptionId = reference,
            StartsAtUtc = success ? DateTime.UtcNow : null,
            EndsAtUtc = success ? DateTime.UtcNow.AddMonths(1) : null
        };
        await _subscriptions.AddAsync(subscription, ct);

        var payment = new Payment
        {
            UserId = _current.UserId.Value,
            SubscriptionId = subscription.Id,
            Amount = course.Price,
            Currency = course.Currency,
            Status = success ? PaymentStatus.Completed : PaymentStatus.Pending,
            PayPalReference = reference,
            Description = $"{request.PlanName} – {course.Title}"
        };
        await _payments.AddAsync(payment, ct);

        if (success)
        {
            var existing = await _enrollments.GetByUserAndCourseAsync(_current.UserId.Value, request.CourseId, ct);
            if (existing is null)
                await _courseService.EnrollAsync(request.CourseId, _current.UserId.Value, ct);
        }

        return Map(subscription, course.Title);
    }

    public async Task<IReadOnlyList<SubscriptionDto>> GetMineAsync(CancellationToken ct = default)
    {
        if (_current.UserId is null) throw new UnauthorizedAccessDomainException();
        var items = _current.IsInRole(UserRole.Admin, UserRole.SuperAdmin)
            ? await _subscriptions.GetAllAsync(ct)
            : await _subscriptions.GetByUserAsync(_current.UserId.Value, ct);

        var result = new List<SubscriptionDto>();
        foreach (var s in items)
        {
            var course = await _courses.GetByIdAsync(s.CourseId, ct);
            result.Add(Map(s, course?.Title ?? ""));
        }
        return result;
    }

    public async Task<IReadOnlyList<PaymentDto>> GetPaymentsAsync(CancellationToken ct = default)
    {
        if (_current.UserId is null) throw new UnauthorizedAccessDomainException();
        var items = _current.IsInRole(UserRole.Admin, UserRole.SuperAdmin)
            ? await _payments.GetAllAsync(ct)
            : await _payments.GetByUserAsync(_current.UserId.Value, ct);
        return items.Select(p => new PaymentDto(p.Id, p.UserId, p.SubscriptionId, p.Amount, p.Currency, p.Status, p.PayPalReference, p.Description, true)).ToList();
    }

    private static SubscriptionDto Map(Subscription s, string courseTitle) =>
        new(s.Id, s.UserId, s.CourseId, courseTitle, s.PlanName, s.Status, s.PayPalSubscriptionId, s.StartsAtUtc, s.EndsAtUtc, true);
}

public class CertificateService
{
    private readonly ICertificateRepository _certificates;
    private readonly ICourseRepository _courses;
    private readonly IUserRepository _users;
    private readonly IProgressRepository _progress;
    private readonly ICurrentUserService _current;

    public CertificateService(
        ICertificateRepository certificates,
        ICourseRepository courses,
        IUserRepository users,
        IProgressRepository progress,
        ICurrentUserService current)
    {
        _certificates = certificates;
        _courses = courses;
        _users = users;
        _progress = progress;
        _current = current;
    }

    public async Task<IReadOnlyList<CertificateDto>> GetMineAsync(CancellationToken ct = default)
    {
        if (_current.UserId is null) throw new UnauthorizedAccessDomainException();
        var items = _current.IsInRole(UserRole.Admin, UserRole.SuperAdmin)
            ? await _certificates.GetAllAsync(ct)
            : await _certificates.GetByUserAsync(_current.UserId.Value, ct);
        return await MapManyAsync(items, ct);
    }

    public async Task<CertificateDto> IssueAsync(Guid userId, Guid courseId, CancellationToken ct = default)
    {
        if (!_current.IsInRole(UserRole.Admin, UserRole.SuperAdmin, UserRole.Mentor))
            throw new UnauthorizedAccessDomainException();

        var progress = await _progress.GetAsync(userId, courseId, ct);
        if (progress is null || progress.CompletionPercent < 80)
            throw new ValidationDomainException("Student must reach at least 80% completion.");

        var cert = new Certificate
        {
            UserId = userId,
            CourseId = courseId,
            CertificateCode = $"SSE-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..8].ToUpperInvariant()}",
            QrVerificationToken = Guid.NewGuid().ToString("N")
        };
        await _certificates.AddAsync(cert, ct);
        return (await MapManyAsync([cert], ct)).First();
    }

    public async Task<CertificateDto> VerifyAsync(string code, CancellationToken ct = default)
    {
        var cert = await _certificates.GetByCodeAsync(code, ct)
            ?? throw new NotFoundException($"Certificate '{code}' was not found.");
        return (await MapManyAsync([cert], ct)).First();
    }

    private async Task<IReadOnlyList<CertificateDto>> MapManyAsync(IEnumerable<Certificate> items, CancellationToken ct)
    {
        var result = new List<CertificateDto>();
        foreach (var c in items)
        {
            var user = await _users.GetByIdAsync(c.UserId, ct);
            var course = await _courses.GetByIdAsync(c.CourseId, ct);
            result.Add(new CertificateDto(c.Id, c.UserId, user?.FullName ?? "", c.CourseId, course?.Title ?? "", c.CertificateCode, c.IssuedAtUtc, c.QrVerificationToken));
        }
        return result;
    }
}

public class AiService
{
    private readonly IAiLearningService _ai;
    private readonly IAiSessionRepository _sessions;
    private readonly ICurrentUserService _current;

    public AiService(IAiLearningService ai, IAiSessionRepository sessions, ICurrentUserService current)
    {
        _ai = ai;
        _sessions = sessions;
        _current = current;
    }

    public async Task<AiResponseDto> AskAsync(AiRequest request, CancellationToken ct = default)
    {
        if (_current.UserId is null) throw new UnauthorizedAccessDomainException();
        var allowed = new[] { "tutor", "translator", "conversation", "speaking", "practice" };
        if (!allowed.Contains(request.Mode, StringComparer.OrdinalIgnoreCase))
            throw new ValidationDomainException("Unsupported AI mode.");

        var response = await _ai.GenerateResponseAsync(request.Mode, request.Prompt, request.SourceLanguage, request.TargetLanguage, ct);
        var session = new AiSession
        {
            UserId = _current.UserId.Value,
            Mode = request.Mode.ToLowerInvariant(),
            Prompt = request.Prompt,
            Response = response,
            TokensUsed = Math.Max(20, request.Prompt.Length / 4)
        };
        await _sessions.AddAsync(session, ct);
        return new AiResponseDto(session.Id, session.Mode, session.Prompt, session.Response, session.TokensUsed, true);
    }

    public async Task<IReadOnlyList<AiResponseDto>> GetHistoryAsync(CancellationToken ct = default)
    {
        if (_current.UserId is null) throw new UnauthorizedAccessDomainException();
        var items = await _sessions.GetByUserAsync(_current.UserId.Value, ct);
        return items.Select(s => new AiResponseDto(s.Id, s.Mode, s.Prompt, s.Response, s.TokensUsed, true)).ToList();
    }
}

public class CommunicationService
{
    private readonly INotificationRepository _notifications;
    private readonly IAnnouncementRepository _announcements;
    private readonly ISupportTicketRepository _tickets;
    private readonly IUserRepository _users;
    private readonly IAuditLogRepository _audit;
    private readonly ICurrentUserService _current;
    private readonly IClassRepository _classes;
    private readonly IEnrollmentRepository _enrollments;
    private readonly ICourseRepository _courses;

    public CommunicationService(
        INotificationRepository notifications,
        IAnnouncementRepository announcements,
        ISupportTicketRepository tickets,
        IUserRepository users,
        IAuditLogRepository audit,
        ICurrentUserService current,
        IClassRepository classes,
        IEnrollmentRepository enrollments,
        ICourseRepository courses)
    {
        _notifications = notifications;
        _announcements = announcements;
        _tickets = tickets;
        _users = users;
        _audit = audit;
        _current = current;
        _classes = classes;
        _enrollments = enrollments;
        _courses = courses;
    }

    public async Task<IReadOnlyList<NotificationDto>> GetNotificationsAsync(CancellationToken ct = default)
    {
        if (_current.UserId is null) throw new UnauthorizedAccessDomainException();
        var items = await _notifications.GetByUserAsync(_current.UserId.Value, ct);
        return items.Select(n => new NotificationDto(n.Id, n.UserId, n.Title, n.Message, n.Type, n.IsRead, n.CreatedAtUtc)).ToList();
    }

    public async Task MarkNotificationReadAsync(Guid id, CancellationToken ct = default)
    {
        await _notifications.MarkReadAsync(id, ct);
    }

    public async Task<SendNotificationResultDto> SendNotificationAsync(SendNotificationRequest request, CancellationToken ct = default)
    {
        if (_current.UserId is null) throw new UnauthorizedAccessDomainException();
        var senderRole = _current.Role;

        if (senderRole == UserRole.User)
            throw new UnauthorizedAccessDomainException("Students are not permitted to dispatch platform notifications.");

        if (string.IsNullOrWhiteSpace(request.Title) || string.IsNullOrWhiteSpace(request.Message))
            throw new ValidationDomainException("Title and message are required.");

        var allUsers = await _users.GetAllAsync(ct);
        var targetUsers = new List<User>();
        string targetDesc;

        // Special handling if manual notification is for a class session
        if (request.ClassSessionId.HasValue)
        {
            var session = await _classes.GetByIdAsync(request.ClassSessionId.Value, ct)
                ?? throw new NotFoundException("Class session", request.ClassSessionId.Value);

            if (senderRole == UserRole.Mentor && session.MentorId != _current.UserId)
                throw new UnauthorizedAccessDomainException("Teachers can only notify students for their own assigned classes.");

            var enrollments = (await _enrollments.GetAllAsync(ct))
                .Where(e => e.CourseId == session.CourseId && e.Status == EnrollmentStatus.Active)
                .ToList();

            var studentIds = enrollments.Select(e => e.UserId).ToHashSet();
            targetUsers = allUsers.Where(u => studentIds.Contains(u.Id)).ToList();
            targetDesc = $"Enrolled students of class '{session.Title}'";
        }
        else if (request.TargetUserId.HasValue)
        {
            var target = allUsers.FirstOrDefault(u => u.Id == request.TargetUserId.Value)
                ?? throw new NotFoundException("Target user", request.TargetUserId.Value);

            // Role restrictions:
            // SuperAdmin can send to: Admin, Mentor, Staff, Student
            // Admin can send to: Mentor, Staff, Student
            // Teacher (Mentor) and Staff can send to: Student only
            if (senderRole is UserRole.Mentor or UserRole.Staff && target.Role != UserRole.User)
                throw new ValidationDomainException("Teachers and Admissions Staff can only send notifications to students.");

            if (senderRole == UserRole.Admin && target.Role is UserRole.Admin or UserRole.SuperAdmin)
                throw new ValidationDomainException("Admins cannot send notifications to other Admins or Super Admins.");

            targetUsers.Add(target);
            targetDesc = $"User {target.FullName} ({target.Role})";
        }
        else if (request.TargetRole.HasValue)
        {
            var role = request.TargetRole.Value;
            if (senderRole is UserRole.Mentor or UserRole.Staff && role != UserRole.User)
                throw new ValidationDomainException("Teachers and Admissions Staff can only send notifications to students.");

            if (senderRole == UserRole.Admin && role is UserRole.Admin or UserRole.SuperAdmin)
                throw new ValidationDomainException("Admins can only send notifications to teachers and students.");

            targetUsers = allUsers.Where(u => u.Role == role).ToList();
            targetDesc = $"All {role} accounts";
        }
        else
        {
            // Default audience based on sender privilege
            targetUsers = senderRole switch
            {
                UserRole.Mentor => allUsers.Where(u => u.Role == UserRole.User).ToList(),
                UserRole.Staff => allUsers.Where(u => u.Role == UserRole.User).ToList(),
                UserRole.Admin => allUsers.Where(u => u.Role is UserRole.Mentor or UserRole.Staff or UserRole.User).ToList(),
                UserRole.SuperAdmin => allUsers.Where(u => u.Role is UserRole.Admin or UserRole.Mentor or UserRole.Staff or UserRole.User).ToList(),
                _ => []
            };
            targetDesc = senderRole switch
            {
                UserRole.Mentor => "All students",
                UserRole.Staff => "All students",
                UserRole.Admin => "Teachers, staff, and students",
                UserRole.SuperAdmin => "All platform members",
                _ => "Selected recipients"
            };
        }

        foreach (var user in targetUsers)
        {
            await _notifications.AddAsync(new Notification
            {
                UserId = user.Id,
                Title = request.Title.Trim(),
                Message = request.Message.Trim(),
                Type = request.Type,
                IsRead = false,
                CreatedAtUtc = DateTime.UtcNow
            }, ct);
        }

        await _audit.AddAsync(new AuditLog
        {
            ActorUserId = _current.UserId.Value,
            Action = "dispatch_notification",
            Resource = "notification",
            Details = $"Dispatched '{request.Title}' to {targetUsers.Count} recipient(s) ({targetDesc})"
        }, ct);

        return new SendNotificationResultDto(targetUsers.Count, targetDesc);
    }

    public async Task<SendNotificationResultDto> NotifyClassStartingAsync(Guid classSessionId, CancellationToken ct = default)
    {
        if (_current.UserId is null) throw new UnauthorizedAccessDomainException();
        var session = await _classes.GetByIdAsync(classSessionId, ct)
            ?? throw new NotFoundException("Class session", classSessionId);

        if (_current.Role == UserRole.Mentor && session.MentorId != _current.UserId)
            throw new UnauthorizedAccessDomainException("Teachers can only notify students for their assigned class.");

        if (!_current.IsInRole(UserRole.Mentor, UserRole.Admin, UserRole.SuperAdmin))
            throw new UnauthorizedAccessDomainException();

        var course = await _courses.GetByIdAsync(session.CourseId, ct);
        var enrollments = (await _enrollments.GetAllAsync(ct))
            .Where(e => e.CourseId == session.CourseId && e.Status == EnrollmentStatus.Active)
            .ToList();

        var allUsers = await _users.GetAllAsync(ct);
        var studentIds = enrollments.Select(e => e.UserId).ToHashSet();
        var targetStudents = allUsers.Where(u => studentIds.Contains(u.Id)).ToList();

        var title = $"Class Starting Soon: {session.Title}";
        var body = $"Your live class for '{course?.Title ?? "your course"}' is starting now! Join here: {session.MeetingUrl}";

        foreach (var student in targetStudents)
        {
            await _notifications.AddAsync(new Notification
            {
                UserId = student.Id,
                Title = title,
                Message = body,
                Type = NotificationType.Class,
                IsRead = false,
                CreatedAtUtc = DateTime.UtcNow
            }, ct);
        }

        await _audit.AddAsync(new AuditLog
        {
            ActorUserId = _current.UserId.Value,
            Action = "class_start_alert",
            Resource = "class",
            Details = $"Sent class start alert for '{session.Title}' to {targetStudents.Count} student(s)"
        }, ct);

        return new SendNotificationResultDto(targetStudents.Count, $"Enrolled students of {session.Title}");
    }

    public async Task<IReadOnlyList<AnnouncementDto>> GetAnnouncementsAsync(CancellationToken ct = default)
    {
        var items = await _announcements.GetPublishedAsync(ct);
        return items.Select(a => new AnnouncementDto(a.Id, a.Title, a.Body, a.CreatedByUserId, a.IsPublished, a.CreatedAtUtc)).ToList();
    }

    public async Task<SupportTicketDto> CreateTicketAsync(CreateSupportTicketRequest request, CancellationToken ct = default)
    {
        if (_current.UserId is null) throw new UnauthorizedAccessDomainException();
        var ticket = new SupportTicket
        {
            UserId = _current.UserId.Value,
            Subject = request.Subject.Trim(),
            Description = request.Description.Trim()
        };
        await _tickets.AddAsync(ticket, ct);
        var user = await _users.GetByIdAsync(ticket.UserId, ct);
        return new SupportTicketDto(ticket.Id, ticket.UserId, user?.FullName ?? "", ticket.Subject, ticket.Description, ticket.Status, ticket.AdminResponse, ticket.CreatedAtUtc);
    }

    public async Task<IReadOnlyList<SupportTicketDto>> GetTicketsAsync(CancellationToken ct = default)
    {
        if (_current.UserId is null) throw new UnauthorizedAccessDomainException();
        var items = _current.IsInRole(UserRole.Staff, UserRole.Admin, UserRole.SuperAdmin)
            ? await _tickets.GetAllAsync(ct)
            : await _tickets.GetByUserAsync(_current.UserId.Value, ct);

        var result = new List<SupportTicketDto>();
        foreach (var t in items)
        {
            var user = await _users.GetByIdAsync(t.UserId, ct);
            result.Add(new SupportTicketDto(t.Id, t.UserId, user?.FullName ?? "", t.Subject, t.Description, t.Status, t.AdminResponse, t.CreatedAtUtc));
        }
        return result;
    }

    public async Task<IReadOnlyList<AuditLogDto>> GetAuditLogsAsync(CancellationToken ct = default)
    {
        if (!_current.IsInRole(UserRole.SuperAdmin))
            throw new UnauthorizedAccessDomainException("Audit logs are accessible to Super Administrators only.");
        var items = await _audit.GetRecentAsync(100, ct);
        return items.Select(a => new AuditLogDto(a.Id, a.ActorUserId, a.Action, a.Resource, a.Details, a.CreatedAtUtc)).ToList();
    }
}

public class DashboardService
{
    private readonly IEnrollmentRepository _enrollments;
    private readonly IClassRepository _classes;
    private readonly IAssessmentRepository _assessments;
    private readonly ICertificateRepository _certificates;
    private readonly IProgressRepository _progress;
    private readonly INotificationRepository _notifications;
    private readonly IUserRepository _users;
    private readonly ICourseRepository _courses;
    private readonly ISupportTicketRepository _tickets;
    private readonly IPaymentRepository _payments;
    private readonly ISubscriptionRepository _subscriptions;
    private readonly IMaterialRepository _materials;
    private readonly IEnquiryRepository _enquiries;
    private readonly ICurrentUserService _current;

    public DashboardService(
        IEnrollmentRepository enrollments,
        IClassRepository classes,
        IAssessmentRepository assessments,
        ICertificateRepository certificates,
        IProgressRepository progress,
        INotificationRepository notifications,
        IUserRepository users,
        ICourseRepository courses,
        ISupportTicketRepository tickets,
        IPaymentRepository payments,
        ISubscriptionRepository subscriptions,
        IMaterialRepository materials,
        IEnquiryRepository enquiries,
        ICurrentUserService current)
    {
        _enrollments = enrollments;
        _classes = classes;
        _assessments = assessments;
        _certificates = certificates;
        _progress = progress;
        _notifications = notifications;
        _users = users;
        _courses = courses;
        _tickets = tickets;
        _payments = payments;
        _subscriptions = subscriptions;
        _materials = materials;
        _enquiries = enquiries;
        _current = current;
    }

    public async Task<DashboardStatsDto> GetStudentAsync(CancellationToken ct = default)
    {
        if (_current.UserId is null) throw new UnauthorizedAccessDomainException();
        var userId = _current.UserId.Value;
        var enrollments = await _enrollments.GetByUserAsync(userId, ct);
        var classes = await _classes.GetAllAsync(ct);
        var enrolledCourseIds = enrollments.Select(e => e.CourseId).ToHashSet();
        var upcoming = classes.Count(c => enrolledCourseIds.Contains(c.CourseId) && c.ScheduledAtUtc > DateTime.UtcNow);
        var attempts = await _assessments.GetAttemptsByUserAsync(userId, ct);
        var certs = await _certificates.GetByUserAsync(userId, ct);
        var progress = await _progress.GetByUserAsync(userId, ct);
        var notes = await _notifications.GetByUserAsync(userId, ct);

        return new DashboardStatsDto(
            enrollments.Count(e => e.Status == EnrollmentStatus.Active),
            upcoming,
            attempts.Count,
            certs.Count,
            progress.Count == 0 ? 0 : Math.Round(progress.Average(p => p.CompletionPercent), 1),
            notes.Count(n => !n.IsRead));
    }

    public async Task<AdminDashboardDto> GetAdminAsync(CancellationToken ct = default)
    {
        if (!_current.IsInRole(UserRole.Admin, UserRole.SuperAdmin))
            throw new UnauthorizedAccessDomainException();

        var users = await _users.GetAllAsync(ct);
        var courses = await _courses.GetAllAsync(ct);
        var enrollments = await _enrollments.GetAllAsync(ct);
        var tickets = await _tickets.GetAllAsync(ct);
        var payments = await _payments.GetAllAsync(ct);
        var subscriptions = await _subscriptions.GetAllAsync(ct);

        return new AdminDashboardDto(
            users.Count,
            courses.Count,
            enrollments.Count(e => e.Status == EnrollmentStatus.Active),
            tickets.Count(t => t.Status is SupportTicketStatus.Open or SupportTicketStatus.InProgress),
            payments.Where(p => p.Status == PaymentStatus.Completed).Sum(p => p.Amount),
            subscriptions.Count(s => s.Status == SubscriptionStatus.Active));
    }

    public async Task<MentorDashboardDto> GetMentorAsync(CancellationToken ct = default)
    {
        if (_current.UserId is null || _current.Role != UserRole.Mentor)
            throw new UnauthorizedAccessDomainException();

        var classes = await _classes.GetByMentorAsync(_current.UserId.Value, ct);
        var courses = await _courses.GetAllAsync(ct);
        var mentorCourses = courses.Where(c => c.MentorId == _current.UserId).ToList();
        var enrollments = await _enrollments.GetAllAsync(ct);
        var studentIds = enrollments.Where(e => mentorCourses.Any(c => c.Id == e.CourseId)).Select(e => e.UserId).Distinct().Count();
        var materialCount = 0;
        foreach (var c in mentorCourses)
            materialCount += (await _materials.GetByCourseAsync(c.Id, ct)).Count;

        return new MentorDashboardDto(classes.Count, studentIds, materialCount, classes.Count(c => c.Status == ClassStatus.Completed));
    }

    public async Task<StaffDashboardDto> GetStaffAsync(CancellationToken ct = default)
    {
        if (_current.UserId is null || !_current.IsInRole(UserRole.Staff, UserRole.Admin, UserRole.SuperAdmin))
            throw new UnauthorizedAccessDomainException();

        var enquiries = await _enquiries.GetAllAsync(ct);
        var tickets = await _tickets.GetAllAsync(ct);
        var notes = await _notifications.GetByUserAsync(_current.UserId.Value, ct);

        return new StaffDashboardDto(
            enquiries.Count,
            enquiries.Count(e => e.Status == EnquiryStatus.New),
            enquiries.Count(e => e.Status is EnquiryStatus.Contacted or EnquiryStatus.Qualified),
            enquiries.Count(e => e.Status == EnquiryStatus.Enrolled),
            tickets.Count(t => t.Status is SupportTicketStatus.Open or SupportTicketStatus.InProgress),
            notes.Count(n => !n.IsRead));
    }

    public async Task<IReadOnlyList<UserDto>> GetUsersAsync(CancellationToken ct = default)
    {
        if (!_current.IsInRole(UserRole.Admin, UserRole.SuperAdmin))
            throw new UnauthorizedAccessDomainException();
        var users = await _users.GetAllAsync(ct);
        return users.Select(AuthService.Map).ToList();
    }

    public async Task<UserDto> UpdateUserStatusAsync(Guid userId, bool isActive, CancellationToken ct = default)
    {
        if (!_current.IsInRole(UserRole.Admin, UserRole.SuperAdmin))
            throw new UnauthorizedAccessDomainException();
        var user = await _users.GetByIdAsync(userId, ct)
            ?? throw new NotFoundException("User", userId);
        user.Status = isActive ? AccountStatus.Active : AccountStatus.Suspended;
        await _users.UpdateAsync(user, ct);
        return AuthService.Map(user);
    }

    public async Task<UserDto> UpdateUserRoleAsync(Guid userId, UserRole role, CancellationToken ct = default)
    {
        if (!_current.IsInRole(UserRole.Admin, UserRole.SuperAdmin))
            throw new UnauthorizedAccessDomainException();
        var user = await _users.GetByIdAsync(userId, ct)
            ?? throw new NotFoundException("User", userId);
        user.Role = role;
        await _users.UpdateAsync(user, ct);
        return AuthService.Map(user);
    }
}
