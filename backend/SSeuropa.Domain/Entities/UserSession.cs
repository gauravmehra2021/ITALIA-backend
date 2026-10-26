using SSeuropa.Domain.Common;

namespace SSeuropa.Domain.Entities;

public class UserSession : EntityBase
{
    public Guid UserId { get; set; }
    public string DeviceId { get; set; } = string.Empty;
    public string DeviceName { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime ExpiresAtUtc { get; set; }
    public bool IsRevoked { get; set; }
    public DateTime? RevokedAtUtc { get; set; }
    public string? IpAddress { get; set; }
    public bool IsFirstDevice { get; set; }
}
