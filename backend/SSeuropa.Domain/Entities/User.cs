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

    public string FullName => $"{FirstName} {LastName}".Trim();
}
