using SSeuropa.Domain.Enums;

namespace SSeuropa.Application.DTOs;

// ── Students (admin panel) ──────────────────────────────────────────────────

public record StudentDto(
    Guid Id,
    string FirstName,
    string LastName,
    string FullName,
    string Email,
    string? PhoneNumber,
    AccountStatus Status,
    bool EmailVerified,
    PreferredLanguage PreferredLanguage,
    Guid? BatchId,
    string? BatchName,
    bool AddedByStaff,
    bool MustChangePassword,
    DateTime CreatedAtUtc,
    DateTime? LastLoginAtUtc,
    int ActiveCourses,
    int ProgressPercent,
    bool IsDeleted,
    /// <summary>Every batch of the student (BatchId / BatchName above are the first one, for older apps).</summary>
    IReadOnlyList<StudentBatchDto>? Batches = null,
    /// <summary>Paid enquiry turned into an account; access (batch + login) not given yet.</summary>
    bool AwaitingLogin = false);

public record StudentBatchDto(Guid Id, string Name, string? StartTime, string? EndTime);

public record CreateStudentRequest(
    string FirstName,
    string LastName,
    string Email,
    string? PhoneNumber = null,
    Guid? BatchId = null,
    PreferredLanguage PreferredLanguage = PreferredLanguage.English,
    /// <summary>All batches to place the student in (wins over BatchId).</summary>
    List<Guid>? BatchIds = null);

/// <summary>The temporary password is returned once so the team can also share it by phone if the email doesn't arrive.</summary>
public record StudentPasswordResultDto(StudentDto Student, string TemporaryPassword);

public record UpdateStudentRequest(
    string FirstName,
    string LastName,
    string? PhoneNumber,
    Guid? BatchId,
    PreferredLanguage? PreferredLanguage = null,
    /// <summary>All batches of the student (wins over BatchId). Empty list = no batch.</summary>
    List<Guid>? BatchIds = null);

/// <summary>Mode: "replace" (default) = only this batch; "add" = also join this batch; "remove" = leave this batch.</summary>
public record MoveStudentsRequest(List<Guid> StudentIds, Guid? BatchId, string? Mode = null);

public record MoveStudentsResultDto(int Moved, string? BatchName);

public record SetStudentStatusRequest(bool IsActive);

/// <summary>Give a paid student access: the batches to place them in (at least one).</summary>
public record GiveAccessRequest(List<Guid>? BatchIds);

/// <summary>TemporaryPassword is null when the student signed up themselves (they keep their own password).</summary>
public record StudentAccessResultDto(StudentDto Student, string? TemporaryPassword);

// ── Batches ────────────────────────────────────────────────────────────────

public record BatchCourseDto(Guid Id, string Title, string CefrLevel, CourseStatus Status);

public record BatchDto(
    Guid Id,
    string Name,
    string Level,
    string Description,
    Guid? MentorId,
    string? MentorName,
    List<BatchCourseDto> Courses,
    DateTime? StartDateUtc,
    int StudentCount,
    DateTime CreatedAtUtc,
    string? StartTime = null,
    string? EndTime = null);

public record TeacherOptionDto(Guid Id, string FullName, UserRole Role);

public record SaveBatchRequest(
    string Name,
    /// <summary>No longer shown in the admin panel; kept so older batches and clients still work.</summary>
    string? Level = null,
    string? Description = null,
    Guid? MentorId = null,
    List<Guid>? CourseIds = null,
    DateTime? StartDateUtc = null,
    /// <summary>Usual class time, "HH:mm" 24-hour (e.g. "11:00" to "12:00"). Optional.</summary>
    string? StartTime = null,
    string? EndTime = null);

// ── Students: own account ──────────────────────────────────────────────────

public record ChangePasswordRequest(string CurrentPassword, string NewPassword);
