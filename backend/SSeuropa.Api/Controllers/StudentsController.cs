using SSeuropa.Api.Security;
using SSeuropa.Application.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SSeuropa.Application.DTOs;
using SSeuropa.Application.Services;

namespace SSeuropa.Api.Controllers;

/// <summary>Student accounts in the admin panel. Teachers can read; Admin and Staff can change.</summary>
[ApiController]
[Authorize(Roles = "SuperAdmin,Admin,Staff,Mentor")]
[TeamModule(TeamPermissions.Students)]
[Route("api/students")]
public class StudentsController(StudentService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<StudentDto>>>> Get(CancellationToken ct)
        => Ok(ApiResponse<IReadOnlyList<StudentDto>>.Ok(await service.GetStudentsAsync(ct)));

    [HttpPost]
    [Authorize(Roles = "SuperAdmin,Admin,Staff,Mentor")]
    public async Task<ActionResult<ApiResponse<StudentPasswordResultDto>>> Create([FromBody] CreateStudentRequest request, CancellationToken ct)
        => Ok(ApiResponse<StudentPasswordResultDto>.Ok(await service.CreateStudentAsync(request, ct), "Student created and login details emailed."));

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "SuperAdmin,Admin,Staff,Mentor")]
    public async Task<ActionResult<ApiResponse<StudentDto>>> Update(Guid id, [FromBody] UpdateStudentRequest request, CancellationToken ct)
        => Ok(ApiResponse<StudentDto>.Ok(await service.UpdateStudentAsync(id, request, ct), "Student updated."));

    [HttpPost("move")]
    [Authorize(Roles = "SuperAdmin,Admin,Staff,Mentor")]
    public async Task<ActionResult<ApiResponse<MoveStudentsResultDto>>> Move([FromBody] MoveStudentsRequest request, CancellationToken ct)
        => Ok(ApiResponse<MoveStudentsResultDto>.Ok(await service.MoveStudentsAsync(request, ct), "Students moved."));

    [HttpPut("{id:guid}/status")]
    [Authorize(Roles = "SuperAdmin,Admin,Staff,Mentor")]
    public async Task<ActionResult<ApiResponse<StudentDto>>> SetStatus(Guid id, [FromBody] SetStudentStatusRequest request, CancellationToken ct)
        => Ok(ApiResponse<StudentDto>.Ok(await service.SetStudentStatusAsync(id, request.IsActive, ct), "Student status updated."));

    [HttpPost("{id:guid}/reset-password")]
    [Authorize(Roles = "SuperAdmin,Admin,Staff,Mentor")]
    public async Task<ActionResult<ApiResponse<StudentPasswordResultDto>>> ResetPassword(Guid id, CancellationToken ct)
        => Ok(ApiResponse<StudentPasswordResultDto>.Ok(await service.ResetStudentPasswordAsync(id, ct), "New temporary password emailed."));

    /// <summary>
    /// Paid student → place in a batch and email the one-time password (if the team made the account).
    /// View access is enough for students waiting for their login, so Teachers can send it; the service checks the rest.
    /// </summary>
    [HttpPost("{id:guid}/give-access")]
    [TeamModule(TeamPermissions.Students, AllowView = true)]
    public async Task<ActionResult<ApiResponse<StudentAccessResultDto>>> GiveAccess(Guid id, [FromBody] GiveAccessRequest request, CancellationToken ct)
    {
        var result = await service.GiveAccessAsync(id, request, ct);
        return Ok(ApiResponse<StudentAccessResultDto>.Ok(result, result.TemporaryPassword is null ? "Access given." : "Access given and login details emailed."));
    }
}

/// <summary>Student batches (groups) and their assigned courses. Only Admins change them.</summary>
[ApiController]
[Authorize(Roles = "SuperAdmin,Admin,Staff,Mentor")]
[TeamModule(TeamPermissions.Batches)]
[Route("api/batches")]
public class BatchesController(StudentService service) : ControllerBase
{
    [HttpGet]
    [TeamModule(TeamPermissions.Batches, TeamPermissions.Students)]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<BatchDto>>>> Get(CancellationToken ct)
        => Ok(ApiResponse<IReadOnlyList<BatchDto>>.Ok(await service.GetBatchesAsync(ct)));

    [HttpGet("teachers")]
    [TeamModule(TeamPermissions.Batches, TeamPermissions.Students)]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<TeacherOptionDto>>>> Teachers(CancellationToken ct)
        => Ok(ApiResponse<IReadOnlyList<TeacherOptionDto>>.Ok(await service.GetTeacherOptionsAsync(ct)));

    [HttpPost]
    [Authorize(Roles = "SuperAdmin,Admin,Staff,Mentor")]
    public async Task<ActionResult<ApiResponse<BatchDto>>> Create([FromBody] SaveBatchRequest request, CancellationToken ct)
        => Ok(ApiResponse<BatchDto>.Ok(await service.CreateBatchAsync(request, ct), "Batch created."));

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "SuperAdmin,Admin,Staff,Mentor")]
    public async Task<ActionResult<ApiResponse<BatchDto>>> Update(Guid id, [FromBody] SaveBatchRequest request, CancellationToken ct)
        => Ok(ApiResponse<BatchDto>.Ok(await service.UpdateBatchAsync(id, request, ct), "Batch updated."));

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "SuperAdmin,Admin,Staff,Mentor")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(Guid id, CancellationToken ct)
    {
        await service.DeleteBatchAsync(id, ct);
        return Ok(ApiResponse<object>.Ok(new { }, "Batch deleted."));
    }
}
