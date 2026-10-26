using FluentAssertions;
using Microsoft.Extensions.Configuration;
using SSeuropa.Application.DTOs;
using SSeuropa.Application.Interfaces;
using SSeuropa.Application.Services;
using SSeuropa.Domain.Enums;
using SSeuropa.Infrastructure.Auth;
using SSeuropa.Infrastructure.Persistence;
using SSeuropa.Infrastructure.Repositories;

namespace SSeuropa.Tests.Application;

public class AuthAndCourseTests
{
    private readonly InMemoryStore _store;
    private readonly AuthService _auth;
    private readonly CourseService _courses;
    private readonly FakeCurrentUser _current = new();

    public AuthAndCourseTests()
    {
        var hasher = new PasswordHasher();
        _store = InMemoryStore.CreateSeeded(hasher.Hash);
        var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["Jwt:Key"] = "SSEuropa-Demo-Signing-Key-At-Least-32-Chars!",
            ["Jwt:Issuer"] = "SSeuropa",
            ["Jwt:Audience"] = "SSeuropa.Web",
            ["Jwt:AccessTokenMinutes"] = "60"
        }).Build();

        _auth = new AuthService(
            new InMemoryUserRepository(_store),
            new InMemorySessionRepository(_store),
            new InMemoryOtpRepository(_store),
            new JwtTokenService(config),
            hasher,
            new OtpService(),
            new InMemoryAuditLogRepository(_store));

        _courses = new CourseService(
            new InMemoryCourseRepository(_store),
            new InMemoryUserRepository(_store),
            new InMemoryEnrollmentRepository(_store),
            _current);
    }

    [Fact]
    public async Task DemoLogin_ReturnsToken_ForSeededStudent()
    {
        var result = await _auth.DemoLoginAsync(new DemoLoginRequest("student@sseuropa.demo"));
        result.AccessToken.Should().NotBeNullOrWhiteSpace();
        result.User.Role.Should().Be(UserRole.User);
        result.IsDemonstrationOnly.Should().BeTrue();
    }

    [Fact]
    public async Task Login_RequiresOtp_AndVerifyCompletes()
    {
        var login = await _auth.LoginAsync(new LoginRequest("student@sseuropa.demo", "Demo@12345", "dev-1", "Test"));
        login.RequiresOtp.Should().BeTrue();
        login.DemoOtpHint.Should().NotBeNullOrWhiteSpace();

        var verified = await _auth.VerifyOtpAsync(new VerifyOtpRequest(login.PendingUserId!.Value, login.DemoOtpHint!, "dev-1", "Test"));
        verified.AccessToken.Should().NotBeNullOrWhiteSpace();
        verified.RequiresOtp.Should().BeFalse();
    }

    [Fact]
    public async Task GetCourses_SupportsSearchAndPagination()
    {
        var page = await _courses.GetCoursesAsync(new SSeuropa.Application.Common.PagedQuery
        {
            Page = 1,
            PageSize = 10,
            Search = "Italian"
        });

        page.TotalCount.Should().BeGreaterThan(0);
        page.Items.Should().OnlyContain(c =>
            c.Title.Contains("Italian", StringComparison.OrdinalIgnoreCase) ||
            c.Description.Contains("Italian", StringComparison.OrdinalIgnoreCase) ||
            c.Language.Contains("Italian", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task CreateCourse_RequiresAdmin()
    {
        _current.Role = UserRole.User;
        _current.UserId = Guid.NewGuid();

        var act = async () => await _courses.CreateAsync(new CreateCourseRequest(
            "Test", "Desc", CourseLevel.Beginner, 10, null, "Italian", 10, "EUR", 4));

        await act.Should().ThrowAsync<SSeuropa.Domain.Exceptions.UnauthorizedAccessDomainException>();
    }

    private sealed class FakeCurrentUser : ICurrentUserService
    {
        public Guid? UserId { get; set; }
        public UserRole? Role { get; set; }
        public bool IsAuthenticated => UserId.HasValue;
        public bool IsInRole(params UserRole[] roles) => Role.HasValue && roles.Contains(Role.Value);
    }
}
