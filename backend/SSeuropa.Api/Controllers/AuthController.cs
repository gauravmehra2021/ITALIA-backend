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
        return Ok(ApiResponse<AuthResponse>.Ok(result, result.RequiresOtp ? "OTP required." : "Login successful."));
    }

    /// <summary>
    /// Demonstration-only login that skips the password. Available in Development only:
    /// in any other environment it returns 404 so nobody can sign in with just an email.
    /// </summary>
    [HttpPost("demo-login")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<AuthResponse>>> DemoLogin(
        DemoLoginRequest request,
        [FromServices] IWebHostEnvironment env,
        CancellationToken ct)
    {
        if (!env.IsDevelopment())
            return NotFound();

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

    [HttpPost("forgot-password")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<MobileAuthChallengeDto>>> ForgotPassword([FromBody] ForgotPasswordRequest request, CancellationToken ct)
    {
        var challenge = await authService.ForgotPasswordAsync(request.Email, ct);
        return Ok(ApiResponse<MobileAuthChallengeDto>.Ok(challenge, "Verification code sent if account exists."));
    }

    [HttpPost("reset-password")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<object>>> ResetPassword([FromBody] ResetPasswordRequest request, CancellationToken ct)
    {
        await authService.ResetPasswordAsync(request.ChallengeId, request.Code, request.NewPassword, ct);
        return Ok(ApiResponse<object>.Ok(new { }, "Password has been successfully reset."));
    }

    [HttpPost("change-password")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<UserDto>>> ChangePassword([FromBody] ChangePasswordRequest request, CancellationToken ct)
    {
        if (currentUser.UserId is null) return Unauthorized();
        var user = await authService.ChangePasswordAsync(currentUser.UserId.Value, request, ct);
        return Ok(ApiResponse<UserDto>.Ok(user, "Password changed."));
    }

    /// <summary>The signed-in account as it is now (role, status, permissions). The admin panel refreshes from this.</summary>
    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<UserDto>>> Me([FromServices] IUserRepository users, CancellationToken ct)
    {
        if (currentUser.UserId is null) return Unauthorized();
        var user = await users.GetByIdAsync(currentUser.UserId.Value, ct);
        if (user is null || user.IsDeleted) return Unauthorized();
        return Ok(ApiResponse<UserDto>.Ok(AuthService.Map(user)));
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
