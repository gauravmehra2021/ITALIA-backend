using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SSeuropa.Application.Common;
using SSeuropa.Application.DTOs;
using SSeuropa.Application.Services;

namespace SSeuropa.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/subscriptions")]
public class SubscriptionsController(SubscriptionService service, IValidator<CreateSubscriptionRequest> validator) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<SubscriptionDto>>>> Get(CancellationToken ct)
        => Ok(ApiResponse<IReadOnlyList<SubscriptionDto>>.Ok(await service.GetMineAsync(ct)));

    [HttpPost]
    public async Task<ActionResult<ApiResponse<SubscriptionDto>>> Create(CreateSubscriptionRequest request, CancellationToken ct)
    {
        await validator.ValidateAndThrowAsync(request, ct);
        return Ok(ApiResponse<SubscriptionDto>.Ok(await service.CreateAsync(request, ct), "Mock PayPal subscription activated."));
    }
}

[ApiController]
[Authorize]
[Route("api/payments")]
public class PaymentsController(SubscriptionService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<PaymentDto>>>> Get(CancellationToken ct)
        => Ok(ApiResponse<IReadOnlyList<PaymentDto>>.Ok(await service.GetPaymentsAsync(ct)));
}

[ApiController]
[Authorize]
[Route("api/certificates")]
public class CertificatesController(CertificateService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<CertificateDto>>>> Get(CancellationToken ct)
        => Ok(ApiResponse<IReadOnlyList<CertificateDto>>.Ok(await service.GetMineAsync(ct)));

    [HttpPost("issue")]
    public async Task<ActionResult<ApiResponse<CertificateDto>>> Issue([FromQuery] Guid userId, [FromQuery] Guid courseId, CancellationToken ct)
        => Ok(ApiResponse<CertificateDto>.Ok(await service.IssueAsync(userId, courseId, ct), "Certificate issued."));

    [HttpGet("verify/{code}")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<CertificateDto>>> Verify(string code, CancellationToken ct)
        => Ok(ApiResponse<CertificateDto>.Ok(await service.VerifyAsync(code, ct)));
}

[ApiController]
[Authorize]
[Route("api/ai")]
public class AiController(AiService service, IValidator<AiRequest> validator) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<ApiResponse<AiResponseDto>>> Ask(AiRequest request, CancellationToken ct)
    {
        await validator.ValidateAndThrowAsync(request, ct);
        return Ok(ApiResponse<AiResponseDto>.Ok(await service.AskAsync(request, ct), "Mock AI response."));
    }

    [HttpGet("history")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<AiResponseDto>>>> History(CancellationToken ct)
        => Ok(ApiResponse<IReadOnlyList<AiResponseDto>>.Ok(await service.GetHistoryAsync(ct)));
}

[ApiController]
[Authorize]
[Route("api/notifications")]
public class NotificationsController(CommunicationService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<NotificationDto>>>> Get(CancellationToken ct)
        => Ok(ApiResponse<IReadOnlyList<NotificationDto>>.Ok(await service.GetNotificationsAsync(ct)));

    [HttpPost("{id:guid}/read")]
    public async Task<ActionResult<ApiResponse<object>>> Read(Guid id, CancellationToken ct)
    {
        await service.MarkNotificationReadAsync(id, ct);
        return Ok(ApiResponse<object>.Ok(new { }));
    }

    [HttpPost("send")]
    public async Task<ActionResult<ApiResponse<SendNotificationResultDto>>> Send(SendNotificationRequest request, CancellationToken ct)
    {
        var result = await service.SendNotificationAsync(request, ct);
        return Ok(ApiResponse<SendNotificationResultDto>.Ok(result, $"Notification dispatched to {result.SentCount} recipients ({result.TargetDescription})."));
    }
}

[ApiController]
[Route("api/announcements")]
public class AnnouncementsController(CommunicationService service) : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<AnnouncementDto>>>> Get(CancellationToken ct)
        => Ok(ApiResponse<IReadOnlyList<AnnouncementDto>>.Ok(await service.GetAnnouncementsAsync(ct)));
}

[ApiController]
[Authorize]
[Route("api/support")]
public class SupportController(CommunicationService service, IValidator<CreateSupportTicketRequest> validator) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<SupportTicketDto>>>> Get(CancellationToken ct)
        => Ok(ApiResponse<IReadOnlyList<SupportTicketDto>>.Ok(await service.GetTicketsAsync(ct)));

    [HttpPost]
    public async Task<ActionResult<ApiResponse<SupportTicketDto>>> Create(CreateSupportTicketRequest request, CancellationToken ct)
    {
        await validator.ValidateAndThrowAsync(request, ct);
        return Ok(ApiResponse<SupportTicketDto>.Ok(await service.CreateTicketAsync(request, ct), "Ticket submitted."));
    }
}

[ApiController]
[Authorize]
[Route("api/dashboard")]
public class DashboardController(DashboardService service) : ControllerBase
{
    [HttpGet("student")]
    public async Task<ActionResult<ApiResponse<DashboardStatsDto>>> Student(CancellationToken ct)
        => Ok(ApiResponse<DashboardStatsDto>.Ok(await service.GetStudentAsync(ct)));

    [HttpGet("admin")]
    public async Task<ActionResult<ApiResponse<AdminDashboardDto>>> Admin(CancellationToken ct)
        => Ok(ApiResponse<AdminDashboardDto>.Ok(await service.GetAdminAsync(ct)));

    [HttpGet("mentor")]
    public async Task<ActionResult<ApiResponse<MentorDashboardDto>>> Mentor(CancellationToken ct)
        => Ok(ApiResponse<MentorDashboardDto>.Ok(await service.GetMentorAsync(ct)));
}

[ApiController]
[Authorize(Roles = "Admin,SuperAdmin")]
[Route("api/users")]
public class UsersController(DashboardService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<UserDto>>>> Get(CancellationToken ct)
        => Ok(ApiResponse<IReadOnlyList<UserDto>>.Ok(await service.GetUsersAsync(ct)));
}

[ApiController]
[Authorize(Roles = "SuperAdmin")]
[Route("api/audit")]
public class AuditController(CommunicationService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<AuditLogDto>>>> Get(CancellationToken ct)
        => Ok(ApiResponse<IReadOnlyList<AuditLogDto>>.Ok(await service.GetAuditLogsAsync(ct)));
}
