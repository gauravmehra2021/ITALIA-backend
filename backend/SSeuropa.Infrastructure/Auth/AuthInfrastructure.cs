using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SSeuropa.Application.Interfaces;
using SSeuropa.Domain.Entities;
using SSeuropa.Domain.Enums;

namespace SSeuropa.Infrastructure.Auth;

public class JwtTokenService(IConfiguration configuration) : IJwtTokenService
{
    public string CreateAccessToken(User user, out DateTime expiresAtUtc)
    {
        var key = configuration["Jwt:Key"] ?? "SSEuropa-Demo-Signing-Key-At-Least-32-Chars!";
        var issuer = configuration["Jwt:Issuer"] ?? "SSeuropa";
        var audience = configuration["Jwt:Audience"] ?? "SSeuropa.Web";
        var minutes = int.TryParse(configuration["Jwt:AccessTokenMinutes"], out var m) ? m : 60;

        expiresAtUtc = DateTime.UtcNow.AddMinutes(minutes);
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email),
            // Issue time: lets the API refuse tokens issued before the Super Admin signed someone out.
            new(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.FullName),
            new(ClaimTypes.Role, user.Role.ToString()),
            new("role", user.Role.ToString())
        };

        var creds = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer,
            audience,
            claims,
            expires: expiresAtUtc,
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string CreateRefreshToken() => Convert.ToBase64String(RandomNumberGenerator.GetBytes(48));
}

public class PasswordHasher : IPasswordHasher
{
    public string Hash(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(16);
        var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, 100_000, HashAlgorithmName.SHA256, 32);
        return $"pbkdf2${Convert.ToBase64String(salt)}${Convert.ToBase64String(hash)}";
    }

    public bool Verify(string password, string stored)
    {
        var parts = stored.Split('$');
        if (parts.Length != 3 || parts[0] != "pbkdf2") return false;
        var salt = Convert.FromBase64String(parts[1]);
        var expected = Convert.FromBase64String(parts[2]);
        var actual = Rfc2898DeriveBytes.Pbkdf2(password, salt, 100_000, HashAlgorithmName.SHA256, 32);
        return CryptographicOperations.FixedTimeEquals(actual, expected);
    }
}

public class OtpService : IOtpService
{
    public string GenerateCode() => RandomNumberGenerator.GetInt32(100000, 999999).ToString();
}

public class CurrentUserService(IHttpContextAccessor accessor) : ICurrentUserService
{
    public Guid? UserId
    {
        get
        {
            var value = accessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier)
                        ?? accessor.HttpContext?.User?.FindFirstValue(JwtRegisteredClaimNames.Sub);
            return Guid.TryParse(value, out var id) ? id : null;
        }
    }

    public UserRole? Role
    {
        get
        {
            var value = accessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Role)
                        ?? accessor.HttpContext?.User?.FindFirstValue("role");
            return Enum.TryParse<UserRole>(value, out var role) ? role : null;
        }
    }

    public bool IsAuthenticated => UserId.HasValue;

    public bool IsInRole(params UserRole[] roles) => Role.HasValue && roles.Contains(Role.Value);

    public SSeuropa.Application.Common.AccessLevel ModuleAccess(string module)
    {
        if (Role == UserRole.SuperAdmin) return SSeuropa.Application.Common.AccessLevel.Full;
        if (accessor.HttpContext?.Items[TeamAccessItems.Key] is IReadOnlyDictionary<string, SSeuropa.Application.Common.AccessLevel> map
            && map.TryGetValue(module, out var level))
            return level;
        return Role is { } role ? SSeuropa.Application.Common.TeamPermissions.Default(role, module) : SSeuropa.Application.Common.AccessLevel.None;
    }
}

/// <summary>
/// Keeps team accounts in memory for a few seconds so the per-request access check
/// doesn't hit the database on every call. TeamService clears an entry the moment it changes the account.
/// </summary>
public class TeamAccessCache : ITeamAccessCache
{
    private static readonly TimeSpan Lifetime = TimeSpan.FromSeconds(20);
    private readonly System.Collections.Concurrent.ConcurrentDictionary<Guid, (User? User, DateTime Expires)> _entries = new();

    public async Task<User?> GetAsync(Guid userId, IUserRepository users, CancellationToken ct)
    {
        if (_entries.TryGetValue(userId, out var hit) && hit.Expires > DateTime.UtcNow) return hit.User;
        var user = await users.GetByIdAsync(userId, ct);
        _entries[userId] = (user, DateTime.UtcNow.Add(Lifetime));
        return user;
    }

    public void Invalidate(Guid userId) => _entries.TryRemove(userId, out _);
}

/// <summary>Where the API's access check leaves the signed-in team member's module permissions for the services.</summary>
public static class TeamAccessItems
{
    public const string Key = "sseuropa:team-permissions";
}
