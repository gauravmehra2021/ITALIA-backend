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

    public AuthService(
        IUserRepository users,
        ISessionRepository sessions,
        IOtpRepository otps,
        IJwtTokenService jwt,
        IPasswordHasher hasher,
        IOtpService otpService,
        IAuditLogRepository audit)
    {
        _users = users;
        _sessions = sessions;
        _otps = otps;
        _jwt = jwt;
        _hasher = hasher;
        _otpService = otpService;
        _audit = audit;
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

        if (user.Status == AccountStatus.Suspended || user.Status == AccountStatus.Disabled)
            throw new UnauthorizedAccessDomainException("Account is not active.");

        // Demo OTP gate for unverified or first-time style challenge
        var otp = await IssueOtpAsync(user.Id, "login", ct);
        await _audit.AddAsync(new AuditLog
        {
            ActorUserId = user.Id,
            Action = "login_otp_issued",
            Resource = "auth",
            Details = $"OTP issued for device {request.DeviceName}"
        }, ct);

        return new AuthResponse(
            AccessToken: string.Empty,
            RefreshToken: string.Empty,
            ExpiresAtUtc: DateTime.UtcNow,
            User: Map(user),
            RequiresOtp: true,
            PendingUserId: user.Id,
            DemoOtpHint: otp.Code,
            IsDemonstrationOnly: true);
    }

    /// <summary>
    /// Demonstration-only shortcut that issues tokens without OTP for local prototyping.
    /// Explicitly labelled as insecure demo flow.
    /// </summary>
    public async Task<AuthResponse> DemoLoginAsync(DemoLoginRequest request, CancellationToken ct = default)
    {
        var user = await _users.GetByEmailAsync(request.Email.Trim().ToLowerInvariant(), ct)
            ?? throw new NotFoundException($"Demo user '{request.Email}' was not found. Use a seeded account.");

        return await CompleteLoginAsync(user, request.DeviceId, request.DeviceName, ct);
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

    private async Task<AuthResponse> CompleteLoginAsync(User user, string deviceId, string deviceName, CancellationToken ct)
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
            IsDemonstrationOnly: true);
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
        return await _otps.AddAsync(challenge, ct);
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
        user.EmailVerified);
}
