using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SSeuropa.Application.Common;
using SSeuropa.Application.Interfaces;
using SSeuropa.Domain.Enums;
using SSeuropa.Infrastructure.Auth;

namespace SSeuropa.Api.Security;

/// <summary>
/// Marks an endpoint as part of an admin-panel module (see <see cref="TeamPermissions"/>).
/// Reading (GET) needs "View"; anything that changes data needs "Full", unless <see cref="RequireFull"/> is set.
/// With several modules, access to any one of them is enough. An action-level attribute replaces the controller's.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
public sealed class TeamModuleAttribute(params string[] modules) : Attribute
{
    public string[] Modules { get; } = modules;
    /// <summary>Always needs "Full", even for a GET.</summary>
    public bool RequireFull { get; init; }
    /// <summary>"View" is enough even for a change (the service then applies its own, narrower rules).</summary>
    public bool AllowView { get; init; }
}

/// <summary>
/// Runs on every request from an Admin, Staff or Teacher account:
/// 1. The account must still exist, be active and still have the role written in its sign-in token —
///    so suspending, removing or re-roling someone takes effect straight away, not when the token expires.
/// 2. If the endpoint belongs to a module, the member's permission for that module must allow it.
/// The Super Admin and students are not affected (their access is decided by the role checks alone).
/// </summary>
public sealed class TeamAccessFilter(TeamAccessCache cache, IUserRepository users) : IAsyncAuthorizationFilter
{
    private static readonly Dictionary<string, string> ModuleLabels = new()
    {
        [TeamPermissions.Enquiries] = "Admissions & Leads",
        [TeamPermissions.Students] = "Students",
        [TeamPermissions.Batches] = "Batches",
        [TeamPermissions.Courses] = "Courses & Content",
        [TeamPermissions.Translations] = "Translations",
        [TeamPermissions.Classes] = "Live Classes",
        [TeamPermissions.Notifications] = "Notifications",
    };

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var principal = context.HttpContext.User;
        if (principal.Identity?.IsAuthenticated != true) return;
        if (context.ActionDescriptor.EndpointMetadata.OfType<IAllowAnonymous>().Any()) return;

        var roleValue = principal.FindFirstValue(ClaimTypes.Role) ?? principal.FindFirstValue("role");
        if (!Enum.TryParse<UserRole>(roleValue, out var tokenRole) || !TeamPermissions.TeamRoles.Contains(tokenRole)) return;

        var idValue = principal.FindFirstValue(ClaimTypes.NameIdentifier) ?? principal.FindFirstValue(JwtRegisteredClaimNames.Sub);
        if (!Guid.TryParse(idValue, out var userId)) return;

        var user = await cache.GetAsync(userId, users, context.HttpContext.RequestAborted);
        if (user is null || user.IsDeleted || user.Status is AccountStatus.Suspended or AccountStatus.Disabled)
        {
            context.Result = Fail(StatusCodes.Status401Unauthorized, "Your account is no longer active. Please contact the Super Admin.");
            return;
        }
        if (user.Role != tokenRole)
        {
            context.Result = Fail(StatusCodes.Status401Unauthorized, "Your role has changed. Please sign in again.");
            return;
        }
        if (user.AccessRevokedAtUtc is { } revokedAt && IssuedAt(principal) is { } issuedAt && issuedAt < revokedAt)
        {
            context.Result = Fail(StatusCodes.Status401Unauthorized, "You were signed out by the Super Admin. Please sign in again.");
            return;
        }

        // Services read this through ICurrentUserService.ModuleAccess (e.g. a Teacher with full Students access sees every student).
        context.HttpContext.Items[TeamAccessItems.Key] =
            (IReadOnlyDictionary<string, AccessLevel>)TeamPermissions.Modules.ToDictionary(m => m, m => TeamPermissions.Effective(user, m));

        var module = context.ActionDescriptor.EndpointMetadata.OfType<TeamModuleAttribute>().LastOrDefault();
        if (module is null) return;

        var method = context.HttpContext.Request.Method;
        var needed = module.RequireFull || (!module.AllowView && !(HttpMethods.IsGet(method) || HttpMethods.IsHead(method))) ? AccessLevel.Full : AccessLevel.View;
        if (module.Modules.Any(m => TeamPermissions.Effective(user, m) >= needed)) return;

        var label = ModuleLabels.GetValueOrDefault(module.Modules[0], module.Modules[0]);
        var hasView = module.Modules.Any(m => TeamPermissions.Effective(user, m) >= AccessLevel.View);
        context.Result = Fail(StatusCodes.Status403Forbidden, hasView
            ? $"You have view-only access to {label}. Ask the Super Admin if you need to make changes."
            : $"You don't have access to {label}. Ask the Super Admin for access.");
    }

    /// <summary>When the sign-in token was issued ("nbf", whole seconds).</summary>
    private static DateTime? IssuedAt(ClaimsPrincipal principal)
    {
        var raw = principal.FindFirstValue(JwtRegisteredClaimNames.Iat) ?? principal.FindFirstValue(JwtRegisteredClaimNames.Nbf);
        return long.TryParse(raw, out var seconds) ? DateTimeOffset.FromUnixTimeSeconds(seconds).UtcDateTime : null;
    }

    private static ObjectResult Fail(int status, string message) =>
        new(ApiResponse<object>.Fail(message)) { StatusCode = status };
}
