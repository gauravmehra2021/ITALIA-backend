using SSeuropa.Domain.Enums;

namespace SSeuropa.Application.DTOs;

public record RegisterRequest(
    string Email,
    string Password,
    string FirstName,
    string LastName,
    PreferredLanguage PreferredLanguage = PreferredLanguage.English);

public record LoginRequest(string Email, string Password, string DeviceId, string DeviceName);

public record VerifyOtpRequest(Guid UserId, string Code, string DeviceId, string DeviceName);

public record DemoLoginRequest(string Email, string DeviceId = "demo-device", string DeviceName = "Demo Browser");

public record AuthResponse(
    string AccessToken,
    string RefreshToken,
    DateTime ExpiresAtUtc,
    UserDto User,
    bool RequiresOtp,
    Guid? PendingUserId,
    string? DemoOtpHint,
    bool IsDemonstrationOnly);

public record UserDto(
    Guid Id,
    string Email,
    string FirstName,
    string LastName,
    string FullName,
    UserRole Role,
    AccountStatus Status,
    PreferredLanguage PreferredLanguage,
    bool EmailVerified);

public record ForgotPasswordRequest(string Email);

public record ResetPasswordRequest(string ChallengeId, string Code, string NewPassword);
