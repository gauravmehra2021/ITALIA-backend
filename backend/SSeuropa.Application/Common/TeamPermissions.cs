using SSeuropa.Domain.Entities;
using SSeuropa.Domain.Enums;

namespace SSeuropa.Application.Common;

/// <summary>How far a team member may go inside one admin-panel module.</summary>
public enum AccessLevel
{
    None = 0,
    View = 1,
    Full = 2
}

/// <summary>
/// Per-member module permissions for the admin panel.
///
/// The role only gives the starting point (the defaults below). The Super Admin can set any module
/// to No access, View or Full for any Admin, Staff or Teacher — e.g. a Teacher with full Students access,
/// or Staff who may build courses. A member without custom permissions gets the role defaults, which are
/// exactly the access each role had before per-member permissions existed.
///
/// The Super Admin always has everything; students are never affected.
/// </summary>
public static class TeamPermissions
{
    public const string Enquiries = "enquiries";
    public const string Students = "students";
    public const string Batches = "batches";
    public const string Courses = "courses";
    public const string Translations = "translations";
    public const string Classes = "classes";
    public const string Notifications = "notifications";

    public static readonly string[] Modules = [Enquiries, Students, Batches, Courses, Translations, Classes, Notifications];

    public static readonly UserRole[] TeamRoles = [UserRole.Admin, UserRole.Staff, UserRole.Mentor];

    private static readonly Dictionary<UserRole, Dictionary<string, AccessLevel>> Defaults = new()
    {
        [UserRole.Admin] = new()
        {
            [Enquiries] = AccessLevel.Full,
            [Students] = AccessLevel.Full,
            [Batches] = AccessLevel.Full,
            [Courses] = AccessLevel.Full,
            [Translations] = AccessLevel.Full,
            [Classes] = AccessLevel.Full,
            [Notifications] = AccessLevel.Full,
        },
        [UserRole.Staff] = new()
        {
            [Enquiries] = AccessLevel.Full,
            [Students] = AccessLevel.Full,
            [Batches] = AccessLevel.View,
            [Courses] = AccessLevel.None,
            [Translations] = AccessLevel.None,
            [Classes] = AccessLevel.None,
            [Notifications] = AccessLevel.None,
        },
        [UserRole.Mentor] = new()
        {
            [Enquiries] = AccessLevel.None,
            [Students] = AccessLevel.View,
            [Batches] = AccessLevel.View,
            [Courses] = AccessLevel.Full,
            [Translations] = AccessLevel.Full,
            [Classes] = AccessLevel.Full,
            [Notifications] = AccessLevel.Full,
        },
    };

    public static bool IsModule(string module) => Modules.Contains(module);

    /// <summary>What a role gets in a module when nobody customised it.</summary>
    public static AccessLevel Default(UserRole role, string module)
    {
        if (role == UserRole.SuperAdmin) return AccessLevel.Full;
        return Defaults.TryGetValue(role, out var map) && map.TryGetValue(module, out var level) ? level : AccessLevel.None;
    }

    /// <summary>What this person can actually do in a module (their custom setting, otherwise the role default).</summary>
    public static AccessLevel Effective(User user, string module)
    {
        if (user.Role == UserRole.SuperAdmin) return AccessLevel.Full;
        if (!TeamRoles.Contains(user.Role)) return AccessLevel.None;
        if (user.Permissions is not null && user.Permissions.TryGetValue(module, out var raw) && Enum.TryParse<AccessLevel>(raw, true, out var custom))
            return custom;
        return Default(user.Role, module);
    }

    public static Dictionary<string, string> EffectiveMap(User user) =>
        Modules.ToDictionary(m => m, m => Effective(user, m).ToString());

    public static Dictionary<string, string> DefaultMap(UserRole role) =>
        Modules.ToDictionary(m => m, m => Default(role, m).ToString());

    /// <summary>
    /// Cleans a requested permission set: unknown modules and levels are rejected, missing modules take the
    /// role default. Returns null when the result equals the role defaults (no custom set needed).
    /// </summary>
    public static Dictionary<string, string>? Normalize(UserRole role, IDictionary<string, string>? requested)
    {
        if (requested is null) return null;
        var result = new Dictionary<string, string>();
        foreach (var (module, rawLevel) in requested)
        {
            if (!IsModule(module))
                throw new Domain.Exceptions.ValidationDomainException($"Unknown permission area '{module}'.");
            if (!Enum.TryParse<AccessLevel>(rawLevel, true, out var level) || !Enum.IsDefined(level))
                throw new Domain.Exceptions.ValidationDomainException($"Access for '{module}' must be None, View or Full.");
            result[module] = level.ToString();
        }

        foreach (var module in Modules.Where(m => !result.ContainsKey(m)))
            result[module] = Default(role, module).ToString();

        return Modules.All(m => result[m] == Default(role, m).ToString()) ? null : result;
    }

    public static string RoleLabel(UserRole role) => role switch
    {
        UserRole.SuperAdmin => "Super Admin",
        UserRole.Mentor => "Teacher",
        _ => role.ToString()
    };
}
