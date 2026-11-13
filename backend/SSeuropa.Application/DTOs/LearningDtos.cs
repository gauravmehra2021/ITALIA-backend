using SSeuropa.Domain.Enums;

namespace SSeuropa.Application.DTOs;

public record CourseDto(
    Guid Id,
    string Title,
    string Description,
    CourseLevel Level,
    CourseStatus Status,
    int Capacity,
    int EnrolledCount,
    Guid? MentorId,
    string? MentorName,
    string Language,
    decimal Price,
    string Currency,
    string? ThumbnailUrl,
    int DurationWeeks,
    DateTime CreatedAtUtc);

public record CreateCourseRequest(
    string Title,
    string Description,
    CourseLevel Level,
    int Capacity,
    Guid? MentorId,
    string Language,
    decimal Price,
    string Currency,
    int DurationWeeks);

public record UpdateCourseRequest(
    string Title,
    string Description,
    CourseLevel Level,
    CourseStatus Status,
    int Capacity,
    Guid? MentorId,
    string Language,
    decimal Price,
    string Currency,
    int DurationWeeks);

public record ClassSessionDto(
    Guid Id,
    Guid CourseId,
    string CourseTitle,
    Guid MentorId,
    string MentorName,
    string Title,
    DateTime ScheduledAtUtc,
    int DurationMinutes,
    ClassStatus Status,
    string? MeetingUrl,
    string? MeetingProvider);

public record CreateClassRequest(
    Guid CourseId,
    Guid MentorId,
    string Title,
    DateTime ScheduledAtUtc,
    int DurationMinutes);

public record EnrollmentDto(
    Guid Id,
    Guid UserId,
    string UserName,
    Guid CourseId,
    string CourseTitle,
    EnrollmentStatus Status,
    DateTime? ActivatedAtUtc);

public record MaterialDto(
    Guid Id,
    Guid CourseId,
    string Title,
    string Description,
    MaterialType Type,
    bool IsProtected,
    int SortOrder,
    string? AccessUrl);

public record CreateMaterialRequest(
    Guid CourseId,
    string Title,
    string Description,
    MaterialType Type,
    int SortOrder);

public record AssessmentDto(
    Guid Id,
    Guid CourseId,
    string Title,
    string Description,
    int PassingScore,
    int TimeLimitMinutes,
    IReadOnlyList<QuestionDto> Questions);

public record QuestionDto(Guid Id, string Prompt, IReadOnlyList<string> Options, int Points);

public record SubmitAttemptRequest(IReadOnlyList<int> SelectedAnswers);

public record TestAttemptDto(
    Guid Id,
    Guid AssessmentId,
    Guid UserId,
    int Score,
    int MaxScore,
    bool Passed,
    DateTime StartedAtUtc,
    DateTime? CompletedAtUtc);

public record AttendanceDto(Guid Id, Guid ClassSessionId, Guid UserId, string UserName, bool Present, DateTime? MarkedAtUtc);

public record MarkAttendanceRequest(Guid ClassSessionId, Guid UserId, bool Present);

public record ProgressDto(
    Guid Id,
    Guid UserId,
    Guid CourseId,
    string CourseTitle,
    decimal CompletionPercent,
    int StreakDays,
    decimal GrammarScore,
    decimal VocabularyScore,
    decimal SpeakingScore,
    DateTime? LastActivityAtUtc);

public record SubscriptionDto(
    Guid Id,
    Guid UserId,
    Guid CourseId,
    string CourseTitle,
    string PlanName,
    SubscriptionStatus Status,
    string? PayPalSubscriptionId,
    DateTime? StartsAtUtc,
    DateTime? EndsAtUtc,
    bool IsMocked);

public record CreateSubscriptionRequest(Guid CourseId, string PlanName);

public record PaymentDto(
    Guid Id,
    Guid UserId,
    Guid? SubscriptionId,
    decimal Amount,
    string Currency,
    PaymentStatus Status,
    string? PayPalReference,
    string Description,
    bool IsMocked);

public record CertificateDto(
    Guid Id,
    Guid UserId,
    string UserName,
    Guid CourseId,
    string CourseTitle,
    string CertificateCode,
    DateTime IssuedAtUtc,
    string QrVerificationToken);

public record NotificationDto(Guid Id, Guid? UserId, string Title, string Message, NotificationType Type, bool IsRead, DateTime CreatedAtUtc);

public record SendNotificationRequest(
    string Title,
    string Message,
    NotificationType Type = NotificationType.Announcement,
    Guid? TargetUserId = null,
    UserRole? TargetRole = null,
    Guid? ClassSessionId = null
);

public record SendNotificationResultDto(int SentCount, string TargetDescription);

public record AnnouncementDto(Guid Id, string Title, string Body, Guid CreatedByUserId, bool IsPublished, DateTime CreatedAtUtc);

public record SupportTicketDto(
    Guid Id,
    Guid UserId,
    string UserName,
    string Subject,
    string Description,
    SupportTicketStatus Status,
    string? AdminResponse,
    DateTime CreatedAtUtc);

public record CreateSupportTicketRequest(string Subject, string Description);

public record AiRequest(string Mode, string Prompt, string? SourceLanguage, string? TargetLanguage);

public record AiResponseDto(Guid SessionId, string Mode, string Prompt, string Response, int TokensUsed, bool IsMocked);

public record DashboardStatsDto(
    int EnrolledCourses,
    int UpcomingClasses,
    int CompletedTests,
    int Certificates,
    decimal AverageProgress,
    int UnreadNotifications);

public record AdminDashboardDto(
    int TotalUsers,
    int TotalCourses,
    int ActiveEnrollments,
    int OpenTickets,
    decimal TotalRevenue,
    int ActiveSubscriptions);

public record MentorDashboardDto(
    int AssignedClasses,
    int AssignedStudents,
    int Materials,
    int PendingAttendance);

public record StaffDashboardDto(
    int TotalEnquiries,
    int NewEnquiries,
    int InFollowUpEnquiries,
    int EnrolledEnquiries,
    int OpenTickets,
    int UnreadNotifications);

public record AuditLogDto(Guid Id, Guid? ActorUserId, string Action, string Resource, string Details, DateTime CreatedAtUtc);

public record SessionDto(Guid Id, Guid UserId, string DeviceId, string DeviceName, DateTime ExpiresAtUtc, bool IsRevoked, DateTime CreatedAtUtc);

public record UpdateUserStatusRequest(bool IsActive);

public record UpdateUserRoleRequest(UserRole Role);
