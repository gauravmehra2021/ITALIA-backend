using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SSeuropa.Application.Common;
using SSeuropa.Application.DTOs;
using SSeuropa.Application.Interfaces;
using SSeuropa.Application.Services;

namespace SSeuropa.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(
    AuthService authService,
    IValidator<RegisterRequest> registerValidator,
    IValidator<LoginRequest> loginValidator,
    ICurrentUserService currentUser) : ControllerBase
{
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<UserDto>>> Register(RegisterRequest request, CancellationToken ct)
    {
        await registerValidator.ValidateAndThrowAsync(request, ct);
        var user = await authService.RegisterAsync(request, ct);
        return Ok(ApiResponse<UserDto>.Ok(user, "Registration successful. Verify OTP to activate."));
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<AuthResponse>>> Login(LoginRequest request, CancellationToken ct)
    {
        await loginValidator.ValidateAndThrowAsync(request, ct);
        var result = await authService.LoginAsync(request, ct);
        return Ok(ApiResponse<AuthResponse>.Ok(result, "OTP required. Demonstration OTP returned in demoOtpHint."));
    }

    /// <summary>
    /// Demonstration-only login that skips OTP. Not for production.
    /// </summary>
    [HttpPost("demo-login")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<AuthResponse>>> DemoLogin(DemoLoginRequest request, CancellationToken ct)
    {
        var result = await authService.DemoLoginAsync(request, ct);
        return Ok(ApiResponse<AuthResponse>.Ok(result, "DEMONSTRATION ONLY — insecure mock authentication."));
    }

    [HttpPost("verify-otp")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<AuthResponse>>> VerifyOtp(VerifyOtpRequest request, CancellationToken ct)
    {
        var result = await authService.VerifyOtpAsync(request, ct);
        return Ok(ApiResponse<AuthResponse>.Ok(result, "OTP verified."));
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<object>>> Logout(CancellationToken ct)
    {
        if (currentUser.UserId is null) return Unauthorized();
        await authService.LogoutAsync(currentUser.UserId.Value, ct);
        return Ok(ApiResponse<object>.Ok(new { }, "Logged out."));
    }
}
