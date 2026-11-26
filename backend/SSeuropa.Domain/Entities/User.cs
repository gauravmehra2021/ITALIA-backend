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

    /// <summary>
    /// Students only: the student's first batch. Kept for older records and apps; the full list is
    /// <see cref="BatchIds"/>. Always change batches through <see cref="SetBatches"/>.
    /// </summary>
    public Guid? BatchId { get; set; }

    /// <summary>Students only: every batch the student is in (e.g. a morning and an evening batch).</summary>
    public List<Guid> BatchIds { get; set; } = [];

    /// <summary>All batches of this student, including a batch saved before students could be in several.</summary>
    public IReadOnlyList<Guid> AllBatchIds()
    {
        var all = new List<Guid>(BatchIds ?? []);
        if (BatchId is { } legacy && !all.Contains(legacy)) all.Insert(0, legacy);
        return all;
    }

    public bool IsInBatch(Guid batchId) => AllBatchIds().Contains(batchId);

    public void SetBatches(IEnumerable<Guid> batchIds)
    {
        BatchIds = batchIds.Distinct().ToList();
        BatchId = BatchIds.Count > 0 ? BatchIds[0] : null;
    }
    /// <summary>The account was created by the team, not by signing up. These students get a temporary password by email.</summary>
    public bool AddedByStaff { get; set; }
    /// <summary>The student must choose a new password before using the portal (set whenever a temporary password is issued).</summary>
    public bool MustChangePassword { get; set; }

    /// <summary>
    /// Students only: their enquiry was marked Paid, so the account exists, but nobody has given access yet
    /// (no login details sent). Cleared when a team member presses "Send login" / "Give access".
    /// </summary>
    public bool AwaitingLogin { get; set; }

    /// <summary>
    /// Team members only: per-module access set by the Super Admin ("None" / "View" / "Full").
    /// Null = the role's default access. Always capped by the role.
    /// </summary>
    public Dictionary<string, string>? Permissions { get; set; }

    /// <summary>Sign-in tokens issued before this moment are refused (set when the Super Admin resets a password, suspends or re-roles a member).</summary>
    public DateTime? AccessRevokedAtUtc { get; set; }

    public string FullName => $"{FirstName} {LastName}".Trim();
}
