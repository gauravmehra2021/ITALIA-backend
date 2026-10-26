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
}
