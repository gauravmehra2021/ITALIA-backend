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
    bool IsDeleted);

public record CreateStudentRequest(
    string FirstName,
    string LastName,
    string Email,
    string? PhoneNumber = null,
    Guid? BatchId = null,
    PreferredLanguage PreferredLanguage = PreferredLanguage.English);

/// <summary>The temporary password is returned once so the team can also share it by phone if the email doesn't arrive.</summary>
public record StudentPasswordResultDto(StudentDto Student, string TemporaryPassword);

public record UpdateStudentRequest(
    string FirstName,
    string LastName,
    string? PhoneNumber,
    Guid? BatchId,
    PreferredLanguage? PreferredLanguage = null);

public record MoveStudentsRequest(List<Guid> StudentIds, Guid? BatchId);

public record MoveStudentsResultDto(int Moved, string? BatchName);

public record SetStudentStatusRequest(bool IsActive);

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
    DateTime CreatedAtUtc);

public record TeacherOptionDto(Guid Id, string FullName, UserRole Role);

public record SaveBatchRequest(
    string Name,
    string Level,
    string? Description = null,
    Guid? MentorId = null,
    List<Guid>? CourseIds = null,
    DateTime? StartDateUtc = null);

// ── Students: own account ──────────────────────────────────────────────────

public record ChangePasswordRequest(string CurrentPassword, string NewPassword);
