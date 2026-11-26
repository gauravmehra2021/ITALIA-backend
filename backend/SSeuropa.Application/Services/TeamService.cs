using System.Security.Cryptography;
using System.Text.RegularExpressions;
using SSeuropa.Application.Common;
using SSeuropa.Application.DTOs;
using SSeuropa.Application.Interfaces;
using SSeuropa.Domain.Entities;
using SSeuropa.Domain.Enums;
using SSeuropa.Domain.Exceptions;

namespace SSeuropa.Application.Services;

/// <summary>
/// Team module — Super Admin only.
/// The Super Admin creates Admin, Staff and Teacher (internal: Mentor) accounts, edits their details,
/// sets per-module permissions, resets passwords, suspends, removes and restores them.
/// There is exactly one Super Admin: that role can never be assigned and the account can't be changed here.
/// Every change is written to the audit log, and changes that cut access sign the person out at once.
/// </summary>
public class TeamService(
    IUserRepository users,
    ISessionRepository sessions,
    IAuditLogRepository audit,
    IPasswordHasher hasher,
    IEmailService email,
    ICurrentUserService current,
    ITeamAccessCache accessCache)
{
    private static readonly Regex EmailPattern = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);

    public TeamPermissionCatalogDto GetCatalog()
    {
        EnsureSuperAdmin();
        return new TeamPermissionCatalogDto(
            TeamPermissions.Modules,
            TeamPermissions.TeamRoles.ToDictionary(
                r => r.ToString(),
                r => (IReadOnlyDictionary<string, string>)TeamPermissions.DefaultMap(r)));
    }

    public async Task<IReadOnlyList<UserDto>> GetAllAsync(CancellationToken ct = default)
    {
        EnsureSuperAdmin();
        return (await users.GetAllAsync(ct)).Select(AuthService.Map).ToList();
    }

    /// <summary>The team only: the Super Admin plus Admin, Staff and Teacher accounts (students are managed elsewhere).</summary>
    public async Task<IReadOnlyList<UserDto>> GetTeamAsync(CancellationToken ct = default)
    {
        EnsureSuperAdmin();
        var all = await users.GetAllAsync(ct);
        return all
            .Where(u => u.Role == UserRole.SuperAdmin || TeamPermissions.TeamRoles.Contains(u.Role))
            .OrderBy(u => u.Role == UserRole.SuperAdmin ? 0 : 1)
            .ThenBy(u => u.FullName)
            .Select(AuthService.Map)
            .ToList();
    }

    public async Task<UserDto> CreateAsync(CreateUserByAdminRequest request, CancellationToken ct = default)
    {
        EnsureSuperAdmin();
        EnsureAssignableRole(request.Role);

        var emailAddress = NormalizeEmail(request.Email);
        var firstName = Required(request.FirstName, "First name");
        var lastName = Required(request.LastName, "Last name");
        if (string.IsNullOrWhiteSpace(request.Password) || request.Password.Length < 8)
            throw new ValidationDomainException("Temporary password must be at least 8 characters.");
        if (await users.GetByEmailAsync(emailAddress, ct) is not null)
            throw new ConflictException("An account with this email already exists.");

        var user = new User
        {
            Email = emailAddress,
            PasswordHash = hasher.Hash(request.Password),
            FirstName = firstName,
            LastName = lastName,
            Role = request.Role,
            Status = AccountStatus.Active,
            EmailVerified = true,
            PreferredLanguage = request.PreferredLanguage,
            PhoneNumber = CleanPhone(request.PhoneNumber),
            Permissions = TeamPermissions.Normalize(request.Role, request.Permissions),
            IsProtectedSuperAdmin = false
        };

        await users.AddAsync(user, ct);
        await email.SendStaffInvitationAsync(user.Email, user.FullName, TeamPermissions.RoleLabel(user.Role), request.Password, ct);
        await LogAsync("team_member_created", user, $"{TeamPermissions.RoleLabel(user.Role)} account created for {user.FullName} ({user.Email}){PermissionNote(user)}", ct);
        return AuthService.Map(user);
    }

    public async Task<UserDto> UpdateDetailsAsync(Guid userId, UpdateTeamMemberRequest request, CancellationToken ct = default)
    {
        EnsureSuperAdmin();
        var user = await GetManageableAsync(userId, ct);

        var emailAddress = NormalizeEmail(request.Email);
        var firstName = Required(request.FirstName, "First name");
        var lastName = Required(request.LastName, "Last name");
        var phone = CleanPhone(request.PhoneNumber);

        if (!string.Equals(emailAddress, user.Email, StringComparison.OrdinalIgnoreCase))
        {
            var other = await users.GetByEmailAsync(emailAddress, ct);
            if (other is not null && other.Id != user.Id)
                throw new ConflictException("Another account already uses this email.");
        }

        var changes = new List<string>();
        if (user.FirstName != firstName || user.LastName != lastName) changes.Add($"name → {firstName} {lastName}");
        if (!string.Equals(user.Email, emailAddress, StringComparison.OrdinalIgnoreCase)) changes.Add($"email {user.Email} → {emailAddress}");
        if ((user.PhoneNumber ?? "") != (phone ?? "")) changes.Add("phone updated");
        if (changes.Count == 0) return AuthService.Map(user);

        user.FirstName = firstName;
        user.LastName = lastName;
        user.Email = emailAddress;
        user.PhoneNumber = phone;
        user.UpdatedAtUtc = DateTime.UtcNow;
        await users.UpdateAsync(user, ct);
        accessCache.Invalidate(user.Id);
        await LogAsync("team_member_updated", user, $"{user.FullName}: {string.Join(", ", changes)}", ct);
        return AuthService.Map(user);
    }

    public async Task<UserDto> UpdateRoleAsync(Guid userId, UserRole role, CancellationToken ct = default)
    {
        EnsureSuperAdmin();
        EnsureAssignableRole(role);
        var user = await GetManageableAsync(userId, ct);
        if (user.Role == role) return AuthService.Map(user);

        var previous = user.Role;
        user.Role = role;
        // Custom permissions were chosen for the old role; the new role starts from its own defaults.
        user.Permissions = null;
        user.UpdatedAtUtc = DateTime.UtcNow;
        await users.UpdateAsync(user, ct);
        // The role is inside the sign-in token, so the person signs in again to pick up the new role.
        await RevokeSessionsAsync(user, ct);
        accessCache.Invalidate(user.Id);
        await LogAsync("team_role_changed", user, $"{user.FullName}: {TeamPermissions.RoleLabel(previous)} → {TeamPermissions.RoleLabel(role)} (permissions reset to role defaults)", ct);
        return AuthService.Map(user);
    }

    public async Task<UserDto> UpdatePermissionsAsync(Guid userId, UpdateTeamPermissionsRequest request, CancellationToken ct = default)
    {
        EnsureSuperAdmin();
        var user = await GetManageableAsync(userId, ct);

        var requested = request.Permissions is { Count: > 0 } ? request.Permissions : null;
        user.Permissions = TeamPermissions.Normalize(user.Role, requested);
        user.UpdatedAtUtc = DateTime.UtcNow;
        await users.UpdateAsync(user, ct);
        accessCache.Invalidate(user.Id);

        var summary = string.Join(", ", TeamPermissions.Modules.Select(m => $"{m}: {TeamPermissions.Effective(user, m)}"));
        await LogAsync("team_permissions_changed", user,
            user.Permissions is null ? $"{user.FullName}: permissions reset to {TeamPermissions.RoleLabel(user.Role)} defaults" : $"{user.FullName}: {summary}", ct);
        return AuthService.Map(user);
    }

    public async Task<UserDto> UpdateStatusAsync(Guid userId, bool isActive, CancellationToken ct = default)
    {
        EnsureSuperAdmin();
        var user = await GetManageableAsync(userId, ct);
        if (user.IsDeleted)
            throw new ValidationDomainException("Restore this account before changing its status.");

        user.Status = isActive ? AccountStatus.Active : AccountStatus.Suspended;
        user.UpdatedAtUtc = DateTime.UtcNow;
        await users.UpdateAsync(user, ct);
        if (!isActive) await RevokeSessionsAsync(user, ct);
        accessCache.Invalidate(user.Id);
        await LogAsync(isActive ? "team_member_activated" : "team_member_suspended", user, user.FullName, ct);
        return AuthService.Map(user);
    }

    public async Task<TeamPasswordResultDto> ResetPasswordAsync(Guid userId, ResetTeamPasswordRequest request, CancellationToken ct = default)
    {
        EnsureSuperAdmin();
        var user = await GetManageableAsync(userId, ct);
        if (user.IsDeleted)
            throw new ValidationDomainException("Restore this account before resetting its password.");

        var password = string.IsNullOrWhiteSpace(request.Password) ? GeneratePassword() : request.Password.Trim();
        if (password.Length < 8)
            throw new ValidationDomainException("The new password must be at least 8 characters.");

        user.PasswordHash = hasher.Hash(password);
        user.UpdatedAtUtc = DateTime.UtcNow;
        await users.UpdateAsync(user, ct);
        await RevokeSessionsAsync(user, ct);
        accessCache.Invalidate(user.Id);
        await email.SendStaffInvitationAsync(user.Email, user.FullName, TeamPermissions.RoleLabel(user.Role), password, ct);
        await LogAsync("team_password_reset", user, $"{user.FullName}: password reset, signed out of all devices", ct);
        return new TeamPasswordResultDto(AuthService.Map(user), password);
    }

    public async Task SoftDeleteAsync(Guid userId, CancellationToken ct = default)
    {
        EnsureSuperAdmin();
        var user = await GetManageableAsync(userId, ct);

        user.IsDeleted = true;
        user.DeletedAtUtc = DateTime.UtcNow;
        user.Status = AccountStatus.Disabled;
        await users.UpdateAsync(user, ct);
        await RevokeSessionsAsync(user, ct);
        accessCache.Invalidate(user.Id);
        await LogAsync("team_member_removed", user, $"{user.FullName} ({user.Email})", ct);
    }

    public async Task RestoreAsync(Guid userId, CancellationToken ct = default)
    {
        EnsureSuperAdmin();
        var user = await GetManageableAsync(userId, ct);

        user.IsDeleted = false;
        user.DeletedAtUtc = null;
        user.Status = AccountStatus.Active;
        await users.UpdateAsync(user, ct);
        accessCache.Invalidate(user.Id);
        await LogAsync("team_member_restored", user, $"{user.FullName} ({user.Email})", ct);
    }

    // ── helpers ──────────────────────────────────────────────────────────────

    private void EnsureSuperAdmin()
    {
        if (!current.IsInRole(UserRole.SuperAdmin))
            throw new UnauthorizedAccessDomainException("Only the Super Admin can manage team accounts.");
    }

    private static void EnsureAssignableRole(UserRole role)
    {
        if (!TeamPermissions.TeamRoles.Contains(role))
            throw new ValidationDomainException("Role must be Admin, Staff or Teacher.");
    }

    private async Task<User> GetManageableAsync(Guid userId, CancellationToken ct)
    {
        var user = await users.GetByIdAsync(userId, ct) ?? throw new NotFoundException("User", userId);

        if (user.Role == UserRole.SuperAdmin || user.IsProtectedSuperAdmin)
            throw new ValidationDomainException("The Super Admin account can't be changed from here.");
        if (current.UserId == user.Id)
            throw new ValidationDomainException("You can't change your own account here.");
        if (!TeamPermissions.TeamRoles.Contains(user.Role))
            throw new ValidationDomainException("This is a student account. Manage students on the Students page.");

        return user;
    }

    /// <summary>Signs the person out everywhere: refresh sessions are revoked and older access tokens are refused.</summary>
    private async Task RevokeSessionsAsync(User user, CancellationToken ct)
    {
        var now = DateTime.UtcNow;
        // Whole seconds: token "nbf" times are whole seconds, so a sign-in right after this still works.
        user.AccessRevokedAtUtc = new DateTime(now.Ticks - now.Ticks % TimeSpan.TicksPerSecond, DateTimeKind.Utc);
        await users.UpdateAsync(user, ct);

        foreach (var session in await sessions.GetByUserAsync(user.Id, ct))
        {
            if (session.IsRevoked) continue;
            session.IsRevoked = true;
            session.RevokedAtUtc = DateTime.UtcNow;
            await sessions.UpdateAsync(session, ct);
        }
    }

    private Task LogAsync(string action, User target, string details, CancellationToken ct) =>
        audit.AddAsync(new AuditLog
        {
            ActorUserId = current.UserId,
            Action = action,
            Resource = $"team:{target.Id}",
            Details = details
        }, ct);

    private static string PermissionNote(User user) => user.Permissions is null ? "" : " with custom permissions";

    private static string NormalizeEmail(string? value)
    {
        var normalized = (value ?? "").Trim().ToLowerInvariant();
        if (normalized.Length == 0) throw new ValidationDomainException("Email is required.");
        if (!EmailPattern.IsMatch(normalized)) throw new ValidationDomainException("Please enter a valid email address.");
        return normalized;
    }

    private static string Required(string? value, string label)
    {
        var trimmed = (value ?? "").Trim();
        if (trimmed.Length == 0) throw new ValidationDomainException($"{label} is required.");
        if (trimmed.Length > 60) throw new ValidationDomainException($"{label} is too long.");
        return trimmed;
    }

    private static string? CleanPhone(string? value)
    {
        var trimmed = value?.Trim();
        if (string.IsNullOrEmpty(trimmed)) return null;
        if (trimmed.Length > 25 || !Regex.IsMatch(trimmed, @"^[+\d][\d\s\-()]{5,}$"))
            throw new ValidationDomainException("Please enter a valid phone number (digits, spaces, + - ( ) only).");
        return trimmed;
    }

    private static string GeneratePassword()
    {
        const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnpqrstuvwxyz23456789";
        var body = new string(Enumerable.Range(0, 10).Select(_ => chars[RandomNumberGenerator.GetInt32(chars.Length)]).ToArray());
        return $"{body}{RandomNumberGenerator.GetInt32(2, 10)}!";
    }
}
