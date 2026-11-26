using SSeuropa.Application.DTOs;
using SSeuropa.Domain.Entities;
using SSeuropa.Domain.Enums;

namespace SSeuropa.Application.Interfaces;

public interface IJwtTokenService
{
    string CreateAccessToken(User user, out DateTime expiresAtUtc);
    string CreateRefreshToken();
}

public interface IPasswordHasher
{
    string Hash(string password);
    bool Verify(string password, string hash);
}

public interface IOtpService
{
    string GenerateCode();
}

public interface IStorageService
{
    Task<string> GetProtectedAccessUrlAsync(string storageKey, TimeSpan ttl, CancellationToken ct = default);
}

public interface IPaymentGateway
{
    Task<(string Reference, bool Success)> CreateSubscriptionAsync(Guid userId, Guid courseId, string planName, decimal amount, CancellationToken ct = default);
    Task<bool> VerifyPaymentAsync(string reference, CancellationToken ct = default);
}

public interface IAiLearningService
{
    Task<string> GenerateResponseAsync(string mode, string prompt, string? sourceLanguage, string? targetLanguage, CancellationToken ct = default);
}

public interface ICurrentUserService
{
    Guid? UserId { get; }
    UserRole? Role { get; }
    bool IsAuthenticated { get; }
    bool IsInRole(params UserRole[] roles);

    /// <summary>The signed-in team member's access to an admin module (set per request by the API's access check). Super Admin: Full.</summary>
    Common.AccessLevel ModuleAccess(string module) => Role == UserRole.SuperAdmin ? Common.AccessLevel.Full : Common.AccessLevel.None;
}

public interface IEmailService
{
    Task SendStaffInvitationAsync(string toEmail, string recipientName, string role, string temporaryPassword, CancellationToken ct = default);
    Task SendPasswordResetCodeAsync(string toEmail, string code, CancellationToken ct = default);
    Task SendVerificationOtpAsync(string toEmail, string studentName, string code, CancellationToken ct = default);
    Task SendAssignmentFeedbackAsync(string toEmail, string studentName, string assignmentTitle, int marks, string? feedback, CancellationToken ct = default);
    /// <summary>Login details for a student account created by the team, or a password reset done by the team.</summary>
    Task SendStudentWelcomeAsync(string toEmail, string studentName, string temporaryPassword, string? batchName, bool isReset, CancellationToken ct = default);
}


/// <summary>Short-lived cache of team accounts used to check access on every request. Clear it when an account changes.</summary>
public interface ITeamAccessCache
{
    void Invalidate(Guid userId);
}
