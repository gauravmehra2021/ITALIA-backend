using SSeuropa.Domain.Common;

namespace SSeuropa.Domain.Entities;

public class OtpChallenge : EntityBase
{
    public Guid UserId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Purpose { get; set; } = "login";
    public DateTime ExpiresAtUtc { get; set; }
    public bool IsUsed { get; set; }
    public DateTime? UsedAtUtc { get; set; }
}
