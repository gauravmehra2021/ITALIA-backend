using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SSeuropa.Application.Common;
using SSeuropa.Application.DTOs;
using SSeuropa.Application.Services;

namespace SSeuropa.Api.Controllers;

[ApiController]
[Route("api/enquiries")]
public class EnquiriesController(
    EnquiryService service,
    IValidator<CreateEnquiryRequest> createValidator,
    IValidator<UpdateEnquiryStatusRequest> statusValidator) : ControllerBase
{
    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<EnquiryDto>>> Create(
        [FromBody] CreateEnquiryRequest request,
        CancellationToken ct)
    {
        await createValidator.ValidateAndThrowAsync(request, ct);
        var result = await service.SubmitAsync(request, ct);
        return Ok(ApiResponse<EnquiryDto>.Ok(result, "Thank you for contacting SS Europa! Our admissions team will review your enquiry and get back to you shortly."));
    }

    [HttpGet]
    [Authorize(Roles = "Staff,Admin,SuperAdmin")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<EnquiryDto>>>> GetAll(CancellationToken ct)
    {
        var result = await service.GetAllAsync(ct);
        return Ok(ApiResponse<IReadOnlyList<EnquiryDto>>.Ok(result));
    }

    [HttpGet("{id:guid}")]
    [Authorize(Roles = "Staff,Admin,SuperAdmin")]
    public async Task<ActionResult<ApiResponse<EnquiryDto>>> GetById(Guid id, CancellationToken ct)
    {
        var result = await service.GetByIdAsync(id, ct);
        return Ok(ApiResponse<EnquiryDto>.Ok(result));
    }

    [HttpPut("{id:guid}/status")]
    [Authorize(Roles = "Staff,Admin,SuperAdmin")]
    public async Task<ActionResult<ApiResponse<EnquiryDto>>> UpdateStatus(
        Guid id,
        [FromBody] UpdateEnquiryStatusRequest request,
        CancellationToken ct)
    {
        await statusValidator.ValidateAndThrowAsync(request, ct);
        var result = await service.UpdateStatusAsync(id, request, ct);
        return Ok(ApiResponse<EnquiryDto>.Ok(result, "Enquiry status updated successfully."));
    }
}
