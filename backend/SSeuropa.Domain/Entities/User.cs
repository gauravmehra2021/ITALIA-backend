using SSeuropa.Domain.Common;
using SSeuropa.Domain.Enums;

namespace SSeuropa.Domain.Entities;

public class User : EntityBase
{
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.User;
    public AccountStatus Status { get; set; } = AccountStatus.PendingVerification;
    public PreferredLanguage PreferredLanguage { get; set; } = PreferredLanguage.English;
    public bool EmailVerified { get; set; }
    public string? PhoneNumber { get; set; }
    public DateTime? LastLoginAtUtc { get; set; }
    public bool IsProtectedSuperAdmin { get; set; } = false;
    /// <summary>
    /// Every lesson of an enrolled course is open, without finishing the previous one first
    /// (e.g. the demo student used to review the content). Normal students unlock lessons in order.
    /// </summary>
    public bool UnlockAllLessons { get; set; }

    /// <summary>Students only: the batch the student belongs to (null = not placed in a batch yet).</summary>
    public Guid? BatchId { get; set; }
    /// <summary>The account was created by the team, not by signing up. These students get a temporary password by email.</summary>
    public bool AddedByStaff { get; set; }
    /// <summary>The student must choose a new password before using the portal (set whenever a temporary password is issued).</summary>
    public bool MustChangePassword { get; set; }

    public string FullName => $"{FirstName} {LastName}".Trim();
}
