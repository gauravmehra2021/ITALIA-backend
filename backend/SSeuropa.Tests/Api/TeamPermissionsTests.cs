using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace SSeuropa.Tests.Api;

/// <summary>
/// Team module: the Super Admin edits team members, sets per-module permissions (None / View / Full,
/// capped by the role), resets passwords — and the API enforces it on every request.
/// Each test creates its own team member so tests don't affect each other.
/// </summary>
public class TeamPermissionsTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    public TeamPermissionsTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.WithWebHostBuilder(b => b.UseSetting("Database:Provider", "InMemory")).CreateClient();
    }

    [Fact]
    public async Task NewMember_WithoutCustomPermissions_GetsRoleDefaults()
    {
        var super = await LoginAsync("superadmin@sseuropa.demo");
        var member = await CreateMemberAsync(super, "Mentor");

        member.HasCustomPermissions.Should().BeFalse();
        member.Permissions!["courses"].Should().Be("Full");
        member.Permissions["students"].Should().Be("View");
        member.Permissions["enquiries"].Should().Be("None");
    }

    [Fact]
    public async Task TeacherWithTranslationsOff_IsBlockedFromTranslations_ButKeepsCourses()
    {
        var super = await LoginAsync("superadmin@sseuropa.demo");
        var member = await CreateMemberAsync(super, "Mentor");

        var save = await SendAsync(HttpMethod.Put, $"/api/users/{member.Id}/permissions", super,
            new { permissions = new Dictionary<string, string> { ["translations"] = "None" } });
        save.StatusCode.Should().Be(HttpStatusCode.OK);
        var saved = await ReadAsync<UserRecord>(save);
        saved.HasCustomPermissions.Should().BeTrue();
        saved.Permissions!["translations"].Should().Be("None");
        saved.Permissions["courses"].Should().Be("Full");

        var token = await LoginAsync(member.Email);
        (await SendAsync(HttpMethod.Get, "/api/translations", token)).StatusCode.Should().Be(HttpStatusCode.Forbidden);
        (await SendAsync(HttpMethod.Get, "/api/curriculum/illustrations", token)).StatusCode.Should().NotBe(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task ViewOnlyStaff_CanListStudents_ButCannotAddOne()
    {
        var super = await LoginAsync("superadmin@sseuropa.demo");
        var member = await CreateMemberAsync(super, "Staff", new Dictionary<string, string> { ["students"] = "View" });
        var token = await LoginAsync(member.Email);

        (await SendAsync(HttpMethod.Get, "/api/students", token)).StatusCode.Should().Be(HttpStatusCode.OK);

        var add = await SendAsync(HttpMethod.Post, "/api/students", token,
            new { firstName = "Blocked", lastName = "Student", email = $"blocked-{Guid.NewGuid():N}@example.com" });
        add.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        (await add.Content.ReadAsStringAsync()).Should().Contain("view-only");
    }

    [Fact]
    public async Task PermissionChange_AppliesWithoutSigningInAgain()
    {
        var super = await LoginAsync("superadmin@sseuropa.demo");
        var member = await CreateMemberAsync(super, "Staff");
        var token = await LoginAsync(member.Email);
        (await SendAsync(HttpMethod.Get, "/api/enquiries", token)).StatusCode.Should().Be(HttpStatusCode.OK);

        await SendAsync(HttpMethod.Put, $"/api/users/{member.Id}/permissions", super,
            new { permissions = new Dictionary<string, string> { ["enquiries"] = "None" } });

        (await SendAsync(HttpMethod.Get, "/api/enquiries", token)).StatusCode.Should().Be(HttpStatusCode.Forbidden);

        var me = await ReadAsync<UserRecord>(await SendAsync(HttpMethod.Get, "/api/auth/me", token));
        me.Permissions!["enquiries"].Should().Be("None");
    }

    [Fact]
    public async Task AnyRole_CanBeGivenAnyArea()
    {
        var super = await LoginAsync("superadmin@sseuropa.demo");

        // Staff normally has no Courses access: give it Full and they can work in the course builder.
        var staff = await CreateMemberAsync(super, "Staff");
        var blocked = await LoginAsync(staff.Email);
        (await SendAsync(HttpMethod.Get, "/api/curriculum/illustrations", blocked)).StatusCode.Should().Be(HttpStatusCode.Forbidden);
        var give = await SendAsync(HttpMethod.Put, $"/api/users/{staff.Id}/permissions", super,
            new { permissions = new Dictionary<string, string> { ["courses"] = "Full", ["enquiries"] = "None" } });
        give.StatusCode.Should().Be(HttpStatusCode.OK);
        (await SendAsync(HttpMethod.Get, "/api/curriculum/illustrations", blocked)).StatusCode.Should().Be(HttpStatusCode.OK);
        (await SendAsync(HttpMethod.Get, "/api/enquiries", blocked)).StatusCode.Should().Be(HttpStatusCode.Forbidden);

        // A Teacher with full Students and Batches access can add a student and create a batch.
        var teacher = await CreateMemberAsync(super, "Mentor",
            new Dictionary<string, string> { ["students"] = "Full", ["batches"] = "Full", ["enquiries"] = "View" });
        var token = await LoginAsync(teacher.Email);
        var add = await SendAsync(HttpMethod.Post, "/api/students", token,
            new { firstName = "Added", lastName = "ByTeacher", email = $"by-teacher-{Guid.NewGuid():N}@example.com" });
        add.StatusCode.Should().Be(HttpStatusCode.OK, await add.Content.ReadAsStringAsync());
        var batch = await SendAsync(HttpMethod.Post, "/api/batches", token, new { name = $"Teacher batch {Guid.NewGuid():N}".Substring(0, 30), level = "A1" });
        batch.StatusCode.Should().Be(HttpStatusCode.OK, await batch.Content.ReadAsStringAsync());
        (await SendAsync(HttpMethod.Get, "/api/enquiries", token)).StatusCode.Should().Be(HttpStatusCode.OK);

        var unknown = await SendAsync(HttpMethod.Put, $"/api/users/{staff.Id}/permissions", super,
            new { permissions = new Dictionary<string, string> { ["billing"] = "Full" } });
        unknown.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task SuspendingAMember_BlocksTheirOpenSessionAtOnce()
    {
        var super = await LoginAsync("superadmin@sseuropa.demo");
        var member = await CreateMemberAsync(super, "Admin");
        var token = await LoginAsync(member.Email);
        (await SendAsync(HttpMethod.Get, "/api/students", token)).StatusCode.Should().Be(HttpStatusCode.OK);

        (await SendAsync(HttpMethod.Put, $"/api/users/{member.Id}/status", super, new { isActive = false })).StatusCode.Should().Be(HttpStatusCode.OK);

        (await SendAsync(HttpMethod.Get, "/api/students", token)).StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task RoleChange_ResetsCustomPermissions_AndSignsTheMemberOut()
    {
        var super = await LoginAsync("superadmin@sseuropa.demo");
        var member = await CreateMemberAsync(super, "Mentor", new Dictionary<string, string> { ["classes"] = "None" });
        var token = await LoginAsync(member.Email);

        var res = await SendAsync(HttpMethod.Put, $"/api/users/{member.Id}/role", super, new { role = "Admin" });
        var updated = await ReadAsync<UserRecord>(res);
        updated.Role.Should().Be("Admin");
        updated.HasCustomPermissions.Should().BeFalse();
        updated.Permissions!["classes"].Should().Be("Full");

        (await SendAsync(HttpMethod.Get, "/api/students", token)).StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task SuperAdmin_EditsDetails_AndEmailMustBeUnique()
    {
        var super = await LoginAsync("superadmin@sseuropa.demo");
        var member = await CreateMemberAsync(super, "Staff");
        var newEmail = $"renamed-{Guid.NewGuid():N}@example.com";

        var ok = await SendAsync(HttpMethod.Put, $"/api/users/{member.Id}", super,
            new { firstName = "Giulia", lastName = "Bianchi", email = newEmail, phoneNumber = "+39 333 123 4567" });
        ok.StatusCode.Should().Be(HttpStatusCode.OK);
        var saved = await ReadAsync<UserRecord>(ok);
        saved.FullName.Should().Be("Giulia Bianchi");
        saved.Email.Should().Be(newEmail);
        saved.PhoneNumber.Should().Be("+39 333 123 4567");

        var clash = await SendAsync(HttpMethod.Put, $"/api/users/{member.Id}", super,
            new { firstName = "Giulia", lastName = "Bianchi", email = "admin@sseuropa.demo", phoneNumber = (string?)null });
        clash.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task PasswordReset_GivesANewPassword_AndEndsOldSessions()
    {
        var super = await LoginAsync("superadmin@sseuropa.demo");
        var member = await CreateMemberAsync(super, "Staff");
        var oldToken = await LoginAsync(member.Email);
        await Task.Delay(1100); // tokens are stamped in whole seconds

        var res = await SendAsync(HttpMethod.Post, $"/api/users/{member.Id}/reset-password", super, new { });
        res.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await ReadAsync<PasswordResult>(res);
        result.TemporaryPassword.Length.Should().BeGreaterThanOrEqualTo(8);

        (await SendAsync(HttpMethod.Get, "/api/enquiries", oldToken)).StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        var login = await _client.PostAsJsonAsync("/api/auth/login", new { email = member.Email, password = result.TemporaryPassword, deviceId = "pw-test", deviceName = "Test" });
        login.StatusCode.Should().Be(HttpStatusCode.OK);
        var fresh = (await login.Content.ReadFromJsonAsync<Envelope<AuthData>>(JsonOptions))!.Data!.AccessToken;
        (await SendAsync(HttpMethod.Get, "/api/enquiries", fresh)).StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task OnlySuperAdmin_ManagesPermissions()
    {
        var super = await LoginAsync("superadmin@sseuropa.demo");
        var member = await CreateMemberAsync(super, "Staff");
        var admin = await LoginAsync("admin@sseuropa.demo");

        (await SendAsync(HttpMethod.Put, $"/api/users/{member.Id}/permissions", admin, new { permissions = (object?)null })).StatusCode.Should().Be(HttpStatusCode.Forbidden);
        (await SendAsync(HttpMethod.Post, $"/api/users/{member.Id}/reset-password", admin, new { })).StatusCode.Should().Be(HttpStatusCode.Forbidden);
        (await SendAsync(HttpMethod.Get, "/api/users/team", admin)).StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task TeamList_HasOnlyTeamAccounts()
    {
        var super = await LoginAsync("superadmin@sseuropa.demo");
        var team = await ReadAsync<List<UserRecord>>(await SendAsync(HttpMethod.Get, "/api/users/team", super));
        team.Should().NotBeEmpty();
        team.Should().OnlyContain(u => u.Role != "User");
        team[0].Role.Should().Be("SuperAdmin");
    }

    // ── helpers ──

    private async Task<UserRecord> CreateMemberAsync(string token, string role, Dictionary<string, string>? permissions = null)
    {
        var res = await SendAsync(HttpMethod.Post, "/api/users", token, new
        {
            email = $"{role.ToLowerInvariant()}-{Guid.NewGuid():N}@example.com",
            password = "Team@12345",
            firstName = "Test",
            lastName = role,
            role,
            permissions
        });
        res.StatusCode.Should().Be(HttpStatusCode.OK, await res.Content.ReadAsStringAsync());
        return await ReadAsync<UserRecord>(res);
    }

    private async Task<string> LoginAsync(string email)
    {
        var res = await _client.PostAsJsonAsync("/api/auth/demo-login", new { email, deviceId = $"dev-{email}", deviceName = "Integration Test" });
        res.StatusCode.Should().Be(HttpStatusCode.OK);
        return (await res.Content.ReadFromJsonAsync<Envelope<AuthData>>(JsonOptions))!.Data!.AccessToken;
    }

    private async Task<HttpResponseMessage> SendAsync(HttpMethod method, string path, string token, object? body = null)
    {
        using var req = new HttpRequestMessage(method, path);
        req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        if (body is not null) req.Content = JsonContent.Create(body);
        return await _client.SendAsync(req);
    }

    private static async Task<T> ReadAsync<T>(HttpResponseMessage res)
    {
        var payload = await res.Content.ReadFromJsonAsync<Envelope<T>>(JsonOptions);
        return payload!.Data!;
    }

    private sealed record Envelope<T>(bool Success, string? Message, T? Data);
    private sealed record AuthData(string AccessToken);
    private sealed record PasswordResult(string TemporaryPassword);
    private sealed record UserRecord(Guid Id, string Email, string FullName, string Role, string? PhoneNumber, Dictionary<string, string>? Permissions, bool HasCustomPermissions);
}
