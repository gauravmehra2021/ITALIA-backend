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
    DateTime CreatedAtUtc,
    string? PaymentUrl = null,
    string? Slug = null,
    string? ShortDescription = null,
    string? Objectives = null,
    int DisplayOrder = 0,
    int ModulesCount = 0,
    int LessonsCount = 0,
    string CefrLevel = "A1",
    string Category = "Core",
    bool OpenToAll = false);

public record CourseStatsDto(
    int TotalCourses,
    int PublishedCourses,
    int DraftCourses,
    int TotalModules,
    int TotalLessons,
    int TotalEnrolled);

public record CreateCourseRequest(
    string Title,
    string Description,
    CourseLevel Level,
    int Capacity = 9999,
    Guid? MentorId = null,
    string Language = "Italian",
    decimal Price = 0,
    string Currency = "EUR",
    int DurationWeeks = 0,
    string? ThumbnailUrl = null,
    string? PaymentUrl = null,
    string? Slug = null,
    string? ShortDescription = null,
    string? Objectives = null,
    int DisplayOrder = 0,
    CourseStatus Status = CourseStatus.Draft,
    string? CefrLevel = null,
    string? Category = null,
    bool? OpenToAll = null);

public record UpdateCourseRequest(
    string Title,
    string Description,
    CourseLevel Level,
    CourseStatus Status,
    int Capacity = 9999,
    Guid? MentorId = null,
    string Language = "Italian",
    decimal Price = 0,
    string Currency = "EUR",
    int DurationWeeks = 0,
    string? ThumbnailUrl = null,
    string? PaymentUrl = null,
    string? Slug = null,
    string? ShortDescription = null,
    string? Objectives = null,
    int DisplayOrder = 0,
    string? CefrLevel = null,
    string? Category = null,
    bool? OpenToAll = null);

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
    int DurationMinutes,
    string? MeetingUrl = null);

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
    string? AccessUrl,
    string ChapterTitle = "Chapter 1: Getting Started",
    int ChapterNumber = 1,
    string? VideoUrl = null,
    string? AudioUrl = null,
    string? PdfUrl = null,
    string? ContentText = null,
    string? PracticalTask = null,
    int DurationMinutes = 15);

public record CreateMaterialRequest(
    Guid CourseId,
    string Title,
    string Description,
    MaterialType Type,
    int SortOrder,
    string ChapterTitle = "Chapter 1: Getting Started",
    int ChapterNumber = 1,
    string? VideoUrl = null,
    string? AudioUrl = null,
    string? PdfUrl = null,
    string? ContentText = null,
    string? PracticalTask = null,
    int DurationMinutes = 15);

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

/// <summary>Operations overview shown to Admins (and embedded in the Super Admin dashboard).</summary>
public record AdminDashboardDto(
    int TotalStudents,
    int TotalCourses,
    int PublishedCourses,
    int ActiveEnrollments,
    int UpcomingClasses,
    int NewEnquiries,
    int InFollowUpEnquiries,
    int EnrolledEnquiries);

/// <summary>Team overview for the single Super Admin, plus the operations overview.</summary>
public record SuperAdminDashboardDto(
    int Admins,
    int Staff,
    int Teachers,
    int Students,
    int SuspendedTeamAccounts,
    AdminDashboardDto Operations);

/// <summary>Teacher (internal role: Mentor) dashboard.</summary>
public record MentorDashboardDto(
    int AssignedClasses,
    int AssignedStudents,
    int Materials,
    int PendingAttendance,
    int UpcomingClasses = 0,
    int CoursesTaught = 0);

public record StaffDashboardDto(
    int TotalEnquiries,
    int NewEnquiries,
    int InFollowUpEnquiries,
    int EnrolledEnquiries,
    int ClosedEnquiries,
    int UnreadNotifications);

public record AuditLogDto(Guid Id, Guid? ActorUserId, string Action, string Resource, string Details, DateTime CreatedAtUtc);

public record SessionDto(Guid Id, Guid UserId, string DeviceId, string DeviceName, DateTime ExpiresAtUtc, bool IsRevoked, DateTime CreatedAtUtc);

public record UpdateUserStatusRequest(bool IsActive);

public record UpdateUserRoleRequest(UserRole Role);

public record FileUploadResponse(string Url, string FileName, long FileSize, string ContentType);

public record CreateUserByAdminRequest(
    string Email,
    string Password,
    string FirstName,
    string LastName,
    UserRole Role,
    PreferredLanguage PreferredLanguage = PreferredLanguage.English,
    string? PhoneNumber = null);

public record AssignmentDto(
    Guid Id,
    Guid CourseId,
    string ChapterTitle,
    string Title,
    string Description,
    string? ReferenceFileUrl,
    DateTime? DueDateUtc,
    int TotalMarks,
    bool IsActive,
    DateTime CreatedAtUtc);

public record CreateAssignmentRequest(
    Guid CourseId,
    string ChapterTitle,
    string Title,
    string Description,
    string? ReferenceFileUrl,
    DateTime? DueDateUtc,
    int TotalMarks);

public record AssignmentSubmissionDto(
    Guid Id,
    Guid AssignmentId,
    Guid StudentId,
    string StudentName,
    string StudentEmail,
    string? SubmissionText,
    string? SubmissionFileUrl,
    DateTime SubmittedAtUtc,
    int? MarksObtained,
    string? TeacherFeedback,
    DateTime? GradedAtUtc,
    string Status);

public record SubmitAssignmentRequest(
    string? SubmissionText,
    string? SubmissionFileUrl);

public record GradeAssignmentRequest(
    int MarksObtained,
    string? TeacherFeedback);
