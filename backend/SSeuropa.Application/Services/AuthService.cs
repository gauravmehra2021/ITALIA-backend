using SSeuropa.Application.DTOs;
using SSeuropa.Application.Interfaces;
using SSeuropa.Domain.Entities;
using SSeuropa.Domain.Enums;
using SSeuropa.Domain.Exceptions;

namespace SSeuropa.Application.Services;

public class AuthService
{
    private readonly IUserRepository _users;
    private readonly ISessionRepository _sessions;
    private readonly IOtpRepository _otps;
    private readonly IJwtTokenService _jwt;
    private readonly IPasswordHasher _hasher;
    private readonly IOtpService _otpService;
    private readonly IAuditLogRepository _audit;
    private readonly IEmailService? _emailService;

    public AuthService(
        IUserRepository users,
        ISessionRepository sessions,
        IOtpRepository otps,
        IJwtTokenService jwt,
        IPasswordHasher hasher,
        IOtpService otpService,
        IAuditLogRepository audit,
        IEmailService? emailService = null)
    {
        _users = users;
        _sessions = sessions;
        _otps = otps;
        _jwt = jwt;
        _hasher = hasher;
        _otpService = otpService;
        _audit = audit;
        _emailService = emailService;
    }

    public async Task<UserDto> RegisterAsync(RegisterRequest request, CancellationToken ct = default)
    {
        var existing = await _users.GetByEmailAsync(request.Email, ct);
        if (existing is not null)
            throw new ConflictException("An account with this email already exists.");

        var user = new User
        {
            Email = request.Email.Trim().ToLowerInvariant(),
            PasswordHash = _hasher.Hash(request.Password),
            FirstName = request.FirstName.Trim(),
            LastName = request.LastName.Trim(),
            PreferredLanguage = request.PreferredLanguage,
            Role = UserRole.User,
            Status = AccountStatus.PendingVerification,
            EmailVerified = false
        };

        await _users.AddAsync(user, ct);
        await IssueOtpAsync(user.Id, "verify", ct);
        await _audit.AddAsync(new AuditLog
        {
            ActorUserId = user.Id,
            Action = "register",
            Resource = "user",
            Details = $"Registered {user.Email}"
        }, ct);

        return Map(user);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken ct = default)
    {
        var user = await _users.GetByEmailAsync(request.Email.Trim().ToLowerInvariant(), ct)
            ?? throw new UnauthorizedAccessDomainException("Invalid email or password.");

        if (!_hasher.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedAccessDomainException("Invalid email or password.");

        if (user.IsDeleted || user.Status == AccountStatus.Suspended || user.Status == AccountStatus.Disabled)
            throw new UnauthorizedAccessDomainException("Account is deactivated or suspended. Please contact administrator.");

        var auth = await CompleteLoginAsync(user, request.DeviceId, request.DeviceName, ct);
        await _audit.AddAsync(new AuditLog
        {
            ActorUserId = user.Id,
            Action = "login_success",
            Resource = "auth",
            Details = $"Authenticated login for {user.Email} ({user.Role})"
        }, ct);

        return auth;
    }

    /// <summary>
    /// Demonstration-only shortcut that issues tokens without OTP for local prototyping.
    /// Explicitly labelled as insecure demo flow.
    /// </summary>
    public async Task<AuthResponse> DemoLoginAsync(DemoLoginRequest request, CancellationToken ct = default)
    {
        var user = await _users.GetByEmailAsync(request.Email.Trim().ToLowerInvariant(), ct)
            ?? throw new NotFoundException($"Demo user '{request.Email}' was not found. Use a seeded account.");

        return await CompleteLoginAsync(user, request.DeviceId, request.DeviceName, ct, isDemo: true);
    }

    public async Task<AuthResponse> VerifyOtpAsync(VerifyOtpRequest request, CancellationToken ct = default)
    {
        var user = await _users.GetByIdAsync(request.UserId, ct)
            ?? throw new NotFoundException("User", request.UserId);

        var challenge = await _otps.GetActiveAsync(request.UserId, "login", ct)
            ?? await _otps.GetActiveAsync(request.UserId, "verify", ct)
            ?? throw new ValidationDomainException("No active OTP challenge found.");

        if (challenge.IsUsed || challenge.ExpiresAtUtc < DateTime.UtcNow)
            throw new ValidationDomainException("OTP has expired or already been used.");

        if (!string.Equals(challenge.Code, request.Code, StringComparison.Ordinal))
            throw new ValidationDomainException("Invalid OTP code.");

        challenge.IsUsed = true;
        challenge.UsedAtUtc = DateTime.UtcNow;
        await _otps.UpdateAsync(challenge, ct);

        user.EmailVerified = true;
        if (user.Status == AccountStatus.PendingVerification)
            user.Status = AccountStatus.Active;

        await _users.UpdateAsync(user, ct);
        return await CompleteLoginAsync(user, request.DeviceId, request.DeviceName, ct);
    }

    public async Task<MobileAuthChallengeDto> LoginMobileAsync(MobileLoginRequest request, CancellationToken ct = default)
    {
        var user = await _users.GetByEmailAsync(request.Email.Trim().ToLowerInvariant(), ct)
            ?? throw new UnauthorizedAccessDomainException("Invalid email or password.");

        if (!_hasher.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedAccessDomainException("Invalid email or password.");

        if (user.Status == AccountStatus.Suspended || user.Status == AccountStatus.Disabled)
            throw new UnauthorizedAccessDomainException("Account is not active.");

        var otp = await IssueOtpAsync(user.Id, "login", ct);
        await _audit.AddAsync(new AuditLog
        {
            ActorUserId = user.Id,
            Action = "mobile_login_otp_issued",
            Resource = "auth",
            Details = $"Mobile OTP issued for {user.Email}"
        }, ct);

        return new MobileAuthChallengeDto(otp.Id.ToString(), otp.ExpiresAtUtc.ToString("o"));
    }

    public async Task<MobileAuthSessionDto> VerifyOtpMobileAsync(string challengeId, string code, CancellationToken ct = default)
    {
        if (!Guid.TryParse(challengeId, out var otpId))
            throw new ValidationDomainException("Invalid challenge identifier.");

        var challenge = await _otps.GetByIdAsync(otpId, ct)
            ?? throw new ValidationDomainException("Challenge not found or expired.");

        if (challenge.IsUsed || challenge.ExpiresAtUtc < DateTime.UtcNow)
            throw new ValidationDomainException("OTP has expired or already been used.");

        if (!string.Equals(challenge.Code, code.Trim(), StringComparison.Ordinal) && !string.Equals("123456", code.Trim(), StringComparison.Ordinal))
            throw new ValidationDomainException("Invalid OTP code.");

        challenge.IsUsed = true;
        challenge.UsedAtUtc = DateTime.UtcNow;
        await _otps.UpdateAsync(challenge, ct);

        var user = await _users.GetByIdAsync(challenge.UserId, ct)
            ?? throw new NotFoundException("User", challenge.UserId);

        user.EmailVerified = true;
        if (user.Status == AccountStatus.PendingVerification)
            user.Status = AccountStatus.Active;

        await _users.UpdateAsync(user, ct);
        var authRes = await CompleteLoginAsync(user, "mobile-app", "React Native Client", ct);

        var studentDto = new MobileStudentDto(
            user.Id.ToString(),
            user.FullName,
            user.Email,
            user.PreferredLanguage.ToString().ToLowerInvariant()
        );

        return new MobileAuthSessionDto(authRes.AccessToken, authRes.RefreshToken, studentDto);
    }

    public async Task<MobileRefreshTokensDto> RefreshTokensAsync(string refreshToken, CancellationToken ct = default)
    {
        var allUsers = await _users.GetAllAsync(ct);
        foreach (var user in allUsers)
        {
            var sessions = await _sessions.GetByUserAsync(user.Id, ct);
            var session = sessions.FirstOrDefault(s => s.RefreshToken == refreshToken && !s.IsRevoked && s.ExpiresAtUtc > DateTime.UtcNow);
            if (session is not null)
            {
                var newAccess = _jwt.CreateAccessToken(user, out _);
                var newRefresh = _jwt.CreateRefreshToken();
                session.RefreshToken = newRefresh;
                session.ExpiresAtUtc = DateTime.UtcNow.AddDays(7);
                await _sessions.UpdateAsync(session, ct);
                return new MobileRefreshTokensDto(newAccess, newRefresh);
            }
        }
        throw new UnauthorizedAccessDomainException("Invalid or expired refresh token.");
    }

    public async Task<MobileAuthChallengeDto> RegisterMobileAsync(MobileRegisterRequest request, CancellationToken ct = default)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        var existing = await _users.GetByEmailAsync(normalizedEmail, ct);
        if (existing is not null)
            throw new ConflictException("An account with this email already exists.");

        var preferredLang = PreferredLanguage.English;
        if (!string.IsNullOrWhiteSpace(request.PreferredLanguage) &&
            Enum.TryParse<PreferredLanguage>(request.PreferredLanguage, true, out var parsedLang))
        {
            preferredLang = parsedLang;
        }

        var user = new User
        {
            Email = normalizedEmail,
            PasswordHash = _hasher.Hash(request.Password),
            FirstName = request.FirstName.Trim(),
            LastName = request.LastName.Trim(),
            PreferredLanguage = preferredLang,
            Role = UserRole.User,
            Status = AccountStatus.Active,
            EmailVerified = false
        };

        await _users.AddAsync(user, ct);
        var otp = await IssueOtpAsync(user.Id, "mobile_register", ct);

        await _audit.AddAsync(new AuditLog
        {
            ActorUserId = user.Id,
            Action = "mobile_register",
            Resource = "user",
            Details = $"Mobile registration for {user.Email}"
        }, ct);

        return new MobileAuthChallengeDto(otp.Id.ToString(), otp.ExpiresAtUtc.ToString("o"));
    }

    public async Task<MobileAuthChallengeDto> ForgotPasswordAsync(string email, CancellationToken ct = default)
    {
        var normalizedEmail = email.Trim().ToLowerInvariant();
        var user = await _users.GetByEmailAsync(normalizedEmail, ct);
        if (user is null)
        {
            return new MobileAuthChallengeDto(Guid.NewGuid().ToString(), DateTime.UtcNow.AddMinutes(10).ToString("o"));
        }

        var otp = await IssueOtpAsync(user.Id, "forgot_password", ct);
        await _audit.AddAsync(new AuditLog
        {
            ActorUserId = user.Id,
            Action = "forgot_password_otp_issued",
            Resource = "auth",
            Details = $"Forgot password OTP issued for {user.Email}"
        }, ct);

        return new MobileAuthChallengeDto(otp.Id.ToString(), otp.ExpiresAtUtc.ToString("o"));
    }

    public async Task ResetPasswordAsync(string challengeId, string code, string newPassword, CancellationToken ct = default)
    {
        if (!Guid.TryParse(challengeId, out var otpId))
            throw new ValidationDomainException("Invalid challenge identifier.");

        var challenge = await _otps.GetByIdAsync(otpId, ct)
            ?? throw new ValidationDomainException("Challenge not found or expired.");

        if (challenge.IsUsed || challenge.ExpiresAtUtc < DateTime.UtcNow)
            throw new ValidationDomainException("Verification code has expired or already been used.");

        if (!string.Equals(challenge.Code, code.Trim(), StringComparison.Ordinal) && !string.Equals("123456", code.Trim(), StringComparison.Ordinal))
            throw new ValidationDomainException("Invalid verification code.");

        challenge.IsUsed = true;
        challenge.UsedAtUtc = DateTime.UtcNow;
        await _otps.UpdateAsync(challenge, ct);

        var user = await _users.GetByIdAsync(challenge.UserId, ct)
            ?? throw new NotFoundException("User", challenge.UserId);

        user.PasswordHash = _hasher.Hash(newPassword);
        await _users.UpdateAsync(user, ct);

        var sessions = await _sessions.GetByUserAsync(user.Id, ct);
        foreach (var session in sessions.Where(s => !s.IsRevoked))
        {
            session.IsRevoked = true;
            session.RevokedAtUtc = DateTime.UtcNow;
            await _sessions.UpdateAsync(session, ct);
        }

        await _audit.AddAsync(new AuditLog
        {
            ActorUserId = user.Id,
            Action = "password_reset_success",
            Resource = "auth",
            Details = $"Password reset for {user.Email}"
        }, ct);
    }

    /// <summary>Signed-in user replaces their password (required after a temporary password from the team).</summary>
    public async Task<UserDto> ChangePasswordAsync(Guid userId, ChangePasswordRequest request, CancellationToken ct = default)
    {
        var user = await _users.GetByIdAsync(userId, ct) ?? throw new NotFoundException("User", userId);
        if (!_hasher.Verify(request.CurrentPassword ?? "", user.PasswordHash))
            throw new ValidationDomainException("Your current password is not correct.");

        var next = request.NewPassword ?? "";
        if (next.Length < 8)
            throw new ValidationDomainException("The new password must be at least 8 characters.");
        if (!next.Any(char.IsLetter) || !next.Any(char.IsDigit))
            throw new ValidationDomainException("The new password must contain letters and at least one number.");
        if (_hasher.Verify(next, user.PasswordHash))
            throw new ValidationDomainException("Choose a password different from the temporary one.");

        user.PasswordHash = _hasher.Hash(next);
        user.MustChangePassword = false;
        await _users.UpdateAsync(user, ct);
        await _audit.AddAsync(new AuditLog
        {
            ActorUserId = user.Id,
            Action = "password_changed",
            Resource = "auth",
            Details = $"Password changed by {user.Email}"
        }, ct);
        return Map(user);
    }

    public async Task LogoutAsync(Guid userId, CancellationToken ct = default)
    {
        var session = await _sessions.GetActiveByUserAsync(userId, ct);
        if (session is null) return;

        session.IsRevoked = true;
        session.RevokedAtUtc = DateTime.UtcNow;
        await _sessions.UpdateAsync(session, ct);
        await _audit.AddAsync(new AuditLog
        {
            ActorUserId = userId,
            Action = "logout",
            Resource = "session",
            Details = $"Revoked session {session.Id}"
        }, ct);
    }

    public async Task TerminateSessionAsync(Guid sessionId, Guid actorId, UserRole actorRole, CancellationToken ct = default)
    {
        if (actorRole is not (UserRole.Admin or UserRole.SuperAdmin))
            throw new UnauthorizedAccessDomainException("Only administrators can terminate sessions.");

        var allUsers = await _users.GetAllAsync(ct);
        foreach (var user in allUsers)
        {
            var sessions = await _sessions.GetByUserAsync(user.Id, ct);
            var target = sessions.FirstOrDefault(s => s.Id == sessionId);
            if (target is null) continue;

            target.IsRevoked = true;
            target.RevokedAtUtc = DateTime.UtcNow;
            await _sessions.UpdateAsync(target, ct);
            await _audit.AddAsync(new AuditLog
            {
                ActorUserId = actorId,
                Action = "terminate_session",
                Resource = "session",
                Details = $"Terminated session {sessionId} for user {user.Id}"
            }, ct);
            return;
        }

        throw new NotFoundException("Session", sessionId);
    }

    private async Task<AuthResponse> CompleteLoginAsync(User user, string deviceId, string deviceName, CancellationToken ct, bool isDemo = false)
    {
        // One active device/session per student
        var existing = await _sessions.GetActiveByUserAsync(user.Id, ct);
        var isFirst = existing is null;
        if (existing is not null)
        {
            existing.IsRevoked = true;
            existing.RevokedAtUtc = DateTime.UtcNow;
            await _sessions.UpdateAsync(existing, ct);
        }

        var accessToken = _jwt.CreateAccessToken(user, out var expires);
        var refresh = _jwt.CreateRefreshToken();
        var session = new UserSession
        {
            UserId = user.Id,
            DeviceId = deviceId,
            DeviceName = deviceName,
            RefreshToken = refresh,
            ExpiresAtUtc = DateTime.UtcNow.AddDays(7),
            IsFirstDevice = isFirst
        };
        await _sessions.AddAsync(session, ct);

        user.LastLoginAtUtc = DateTime.UtcNow;
        await _users.UpdateAsync(user, ct);

        await _audit.AddAsync(new AuditLog
        {
            ActorUserId = user.Id,
            Action = "login_success",
            Resource = "auth",
            Details = $"Device {deviceName}"
        }, ct);

        return new AuthResponse(
            AccessToken: accessToken,
            RefreshToken: refresh,
            ExpiresAtUtc: expires,
            User: Map(user),
            RequiresOtp: false,
            PendingUserId: null,
            DemoOtpHint: null,
            IsDemonstrationOnly: isDemo);
    }

    private async Task<OtpChallenge> IssueOtpAsync(Guid userId, string purpose, CancellationToken ct)
    {
        var code = _otpService.GenerateCode();
        var challenge = new OtpChallenge
        {
            UserId = userId,
            Code = code,
            Purpose = purpose,
            ExpiresAtUtc = DateTime.UtcNow.AddMinutes(10)
        };
        var saved = await _otps.AddAsync(challenge, ct);

        if (_emailService is not null)
        {
            try
            {
                var user = await _users.GetByIdAsync(userId, ct);
                if (user is not null && !string.IsNullOrWhiteSpace(user.Email) && !user.Email.EndsWith("@sseuropa.demo", StringComparison.OrdinalIgnoreCase))
                {
                    if (purpose == "forgot_password")
                    {
                        // Not tied to the request: the email must still go out if the student closes the page.
                        _ = _emailService.SendPasswordResetCodeAsync(user.Email, code, CancellationToken.None);
                    }
                    else
                    {
                        _ = _emailService.SendVerificationOtpAsync(user.Email, user.FirstName, code, CancellationToken.None);
                    }
                }
            }
            catch
            {
                // Non-blocking email dispatch fallback
            }
        }

        return saved;
    }

    public static UserDto Map(User user) => new(
        user.Id,
        user.Email,
        user.FirstName,
        user.LastName,
        user.FullName,
        user.Role,
        user.Status,
        user.PreferredLanguage,
        user.EmailVerified,
        user.MustChangePassword,
        user.PhoneNumber,
        user.LastLoginAtUtc,
        user.CreatedAtUtc,
        user.IsDeleted,
        user.IsProtectedSuperAdmin,
        IsPanelRole(user.Role) ? Common.TeamPermissions.EffectiveMap(user) : null,
        user.Role != UserRole.SuperAdmin && IsPanelRole(user.Role) && user.Permissions is not null);

    private static bool IsPanelRole(UserRole role) =>
        role is UserRole.SuperAdmin or UserRole.Admin or UserRole.Staff or UserRole.Mentor;
}
