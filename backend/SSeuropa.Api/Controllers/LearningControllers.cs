using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SSeuropa.Application.Common;
using SSeuropa.Application.DTOs;
using SSeuropa.Application.Services;

namespace SSeuropa.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/classes")]
public class ClassesController(ClassService classService, IValidator<CreateClassRequest> validator) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<ClassSessionDto>>>> Get(CancellationToken ct)
        => Ok(ApiResponse<IReadOnlyList<ClassSessionDto>>.Ok(await classService.GetAllAsync(ct)));

    [HttpPost]
    [Authorize(Roles = "Mentor,Admin,SuperAdmin")]
    public async Task<ActionResult<ApiResponse<ClassSessionDto>>> Create(CreateClassRequest request, CancellationToken ct)
    {
        await validator.ValidateAndThrowAsync(request, ct);
        return Ok(ApiResponse<ClassSessionDto>.Ok(await classService.CreateAsync(request, ct), "Class scheduled."));
    }

    [HttpGet("{id:guid}/meeting")]
    public async Task<ActionResult<ApiResponse<ClassSessionDto>>> Meeting(Guid id, CancellationToken ct)
        => Ok(ApiResponse<ClassSessionDto>.Ok(await classService.GetMeetingAccessAsync(id, ct)));

    [HttpPost("{id:guid}/notify-start")]
    [Authorize(Roles = "Mentor,Admin,SuperAdmin")]
    public async Task<ActionResult<ApiResponse<SendNotificationResultDto>>> NotifyStart(Guid id, [FromServices] CommunicationService comms, CancellationToken ct)
    {
        var result = await comms.NotifyClassStartingAsync(id, ct);
        return Ok(ApiResponse<SendNotificationResultDto>.Ok(result, $"Class start alert dispatched to {result.SentCount} student(s)."));
    }
}

[ApiController]
[Authorize]
[Route("api/materials")]
public class MaterialsController(MaterialService materialService) : ControllerBase
{
    [HttpGet("course/{courseId:guid}")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<MaterialDto>>>> ByCourse(Guid courseId, CancellationToken ct)
        => Ok(ApiResponse<IReadOnlyList<MaterialDto>>.Ok(await materialService.GetByCourseAsync(courseId, ct)));

    [HttpPost]
    [Authorize(Roles = "Mentor,Admin,SuperAdmin")]
    public async Task<ActionResult<ApiResponse<MaterialDto>>> Create(CreateMaterialRequest request, CancellationToken ct)
        => Ok(ApiResponse<MaterialDto>.Ok(await materialService.CreateAsync(request, ct), "Material added."));

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Mentor,Admin,SuperAdmin")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(Guid id, CancellationToken ct)
    {
        await materialService.DeleteAsync(id, ct);
        return Ok(ApiResponse<object>.Ok(new { }, "Material deleted."));
    }
}

[ApiController]
[Authorize]
[Route("api/assessments")]
public class AssessmentsController(AssessmentService assessmentService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<AssessmentDto>>>> GetAll(CancellationToken ct)
        => Ok(ApiResponse<IReadOnlyList<AssessmentDto>>.Ok(await assessmentService.GetAllAsync(ct)));

    [HttpGet("course/{courseId:guid}")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<AssessmentDto>>>> ByCourse(Guid courseId, CancellationToken ct)
        => Ok(ApiResponse<IReadOnlyList<AssessmentDto>>.Ok(await assessmentService.GetByCourseAsync(courseId, ct)));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<AssessmentDto>>> Get(Guid id, CancellationToken ct)
        => Ok(ApiResponse<AssessmentDto>.Ok(await assessmentService.GetByIdAsync(id, ct)));

    [HttpPost("{id:guid}/attempts")]
    public async Task<ActionResult<ApiResponse<TestAttemptDto>>> Submit(Guid id, SubmitAttemptRequest request, CancellationToken ct)
        => Ok(ApiResponse<TestAttemptDto>.Ok(await assessmentService.SubmitAsync(id, request, ct), "Attempt submitted."));

    [HttpGet("attempts/mine")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<TestAttemptDto>>>> MyAttempts(CancellationToken ct)
        => Ok(ApiResponse<IReadOnlyList<TestAttemptDto>>.Ok(await assessmentService.GetMyAttemptsAsync(ct)));
}

[ApiController]
[Authorize]
[Route("api/progress")]
public class ProgressController(ProgressService progressService) : ControllerBase
{
    [HttpGet("mine")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<ProgressDto>>>> Mine(CancellationToken ct)
        => Ok(ApiResponse<IReadOnlyList<ProgressDto>>.Ok(await progressService.GetMyProgressAsync(ct)));

    [HttpPost("attendance")]
    public async Task<ActionResult<ApiResponse<AttendanceDto>>> Mark(MarkAttendanceRequest request, CancellationToken ct)
        => Ok(ApiResponse<AttendanceDto>.Ok(await progressService.MarkAttendanceAsync(request, ct)));

    [HttpGet("attendance/{classSessionId:guid}")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<AttendanceDto>>>> Attendance(Guid classSessionId, CancellationToken ct)
        => Ok(ApiResponse<IReadOnlyList<AttendanceDto>>.Ok(await progressService.GetAttendanceAsync(classSessionId, ct)));
}
