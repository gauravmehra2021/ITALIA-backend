using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace SSeuropa.Tests.Api;

/// <summary>
/// Admin panel has four roles: Super Admin (exactly one), Admin, Staff and Teacher (internal: Mentor).
/// Only the Super Admin manages team accounts, and nobody can create or become another Super Admin.
/// </summary>
public class TeamRolesTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    public TeamRolesTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
            builder.UseSetting("Database:Provider", "InMemory"));
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task Admin_CannotListOrCreateTeamAccounts()
    {
        var admin = await LoginAsync("admin@sseuropa.demo");

        var list = await SendAsync(HttpMethod.Get, "/api/users", admin);
        list.StatusCode.Should().Be(HttpStatusCode.Forbidden);

        var create = await SendAsync(HttpMethod.Post, "/api/users", admin, NewMember("by-admin@example.com", "Staff"));
        create.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Theory]
    [InlineData("Admin")]
    [InlineData("Staff")]
    [InlineData("Mentor")]
    public async Task SuperAdmin_CreatesTeamMember_WithExactRole(string role)
    {
        var super = await LoginAsync("superadmin@sseuropa.demo");

        var res = await SendAsync(HttpMethod.Post, "/api/users", super, NewMember($"new-{role.ToLowerInvariant()}@example.com", role));

        res.StatusCode.Should().Be(HttpStatusCode.OK);
        var payload = await res.Content.ReadFromJsonAsync<Envelope<UserRecord>>(JsonOptions);
        payload!.Data!.Role.Should().Be(role);
    }

    [Theory]
    [InlineData("SuperAdmin")]
    [InlineData("User")]
    public async Task SuperAdmin_CannotCreateSuperAdminOrStudentFromPanel(string role)
    {
        var super = await LoginAsync("superadmin@sseuropa.demo");

        var res = await SendAsync(HttpMethod.Post, "/api/users", super, NewMember($"blocked-{role.ToLowerInvariant()}@example.com", role));

        res.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task SuperAdmin_CannotPromoteAnyoneToSuperAdmin_OrTouchOwnAccount()
    {
        var super = await LoginAsync("superadmin@sseuropa.demo");
        var users = await ListUsersAsync(super);
        var staff = users.Single(u => u.Email == "staff@sseuropa.demo");
        var self = users.Single(u => u.Email == "superadmin@sseuropa.demo");

        var promote = await SendAsync(HttpMethod.Put, $"/api/users/{staff.Id}/role", super, new { role = "SuperAdmin" });
        promote.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var suspendSelf = await SendAsync(HttpMethod.Put, $"/api/users/{self.Id}/status", super, new { isActive = false });
        suspendSelf.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var deleteSelf = await SendAsync(HttpMethod.Delete, $"/api/users/{self.Id}", super);
        deleteSelf.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task SuperAdmin_CanChangeTeamRole_ButNotStudentRole()
    {
        var super = await LoginAsync("superadmin@sseuropa.demo");
        var users = await ListUsersAsync(super);
        var mentor = users.Single(u => u.Email == "mentor@sseuropa.demo");
        var student = users.Single(u => u.Email == "priya@sseuropa.demo");

        var toStaff = await SendAsync(HttpMethod.Put, $"/api/users/{mentor.Id}/role", super, new { role = "Staff" });
        toStaff.StatusCode.Should().Be(HttpStatusCode.OK);
        var back = await SendAsync(HttpMethod.Put, $"/api/users/{mentor.Id}/role", super, new { role = "Mentor" });
        back.StatusCode.Should().Be(HttpStatusCode.OK);

        var studentToAdmin = await SendAsync(HttpMethod.Put, $"/api/users/{student.Id}/role", super, new { role = "Admin" });
        studentToAdmin.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Theory]
    [InlineData("superadmin@sseuropa.demo", "/api/dashboard/superadmin")]
    [InlineData("admin@sseuropa.demo", "/api/dashboard/admin")]
    [InlineData("staff@sseuropa.demo", "/api/dashboard/staff")]
    [InlineData("mentor@sseuropa.demo", "/api/dashboard/mentor")]
    public async Task EachRole_GetsOwnDashboard(string email, string path)
    {
        var token = await LoginAsync(email);
        var res = await SendAsync(HttpMethod.Get, path, token);
        res.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Theory]
    [InlineData("admin@sseuropa.demo", "/api/dashboard/superadmin")]
    [InlineData("staff@sseuropa.demo", "/api/dashboard/admin")]
    [InlineData("mentor@sseuropa.demo", "/api/enquiries")]
    public async Task Roles_CannotOpenOtherRolesData(string email, string path)
    {
        var token = await LoginAsync(email);
        var res = await SendAsync(HttpMethod.Get, path, token);
        res.StatusCode.Should().BeOneOf(HttpStatusCode.Forbidden, HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task DemoLogin_IsDisabled_OutsideDevelopment()
    {
        using var prod = _factory.WithWebHostBuilder(b => b.UseEnvironment("Production")).CreateClient();

        var res = await prod.PostAsJsonAsync("/api/auth/demo-login", new
        {
            email = "superadmin@sseuropa.demo",
            deviceId = "prod-probe",
            deviceName = "Integration Test"
        });

        res.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    private static object NewMember(string email, string role) => new
    {
        email,
        password = "Team@12345",
        firstName = "Test",
        lastName = "Member",
        role
    };

    private async Task<string> LoginAsync(string email)
    {
        var res = await _client.PostAsJsonAsync("/api/auth/demo-login", new
        {
            email,
            deviceId = $"dev-{email}",
            deviceName = "Integration Test"
        });
        res.StatusCode.Should().Be(HttpStatusCode.OK);
        var data = await res.Content.ReadFromJsonAsync<Envelope<AuthData>>(JsonOptions);
        return data!.Data!.AccessToken;
    }

    private async Task<List<UserRecord>> ListUsersAsync(string token)
    {
        var res = await SendAsync(HttpMethod.Get, "/api/users", token);
        res.StatusCode.Should().Be(HttpStatusCode.OK);
        var payload = await res.Content.ReadFromJsonAsync<Envelope<List<UserRecord>>>(JsonOptions);
        return payload!.Data!;
    }

    private async Task<HttpResponseMessage> SendAsync(HttpMethod method, string path, string token, object? body = null)
    {
        using var req = new HttpRequestMessage(method, path);
        req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        if (body is not null) req.Content = JsonContent.Create(body);
        return await _client.SendAsync(req);
    }

    private sealed record Envelope<T>(bool Success, string? Message, T? Data);
    private sealed record AuthData(string AccessToken);
    private sealed record UserRecord(Guid Id, string Email, string Role);
}
