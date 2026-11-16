using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SSeuropa.Application.Common;
using SSeuropa.Application.DTOs;
using SSeuropa.Application.Services;

namespace SSeuropa.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/assignments")]
public class AssignmentsController : ControllerBase
{
    private readonly AssignmentService _service;

    public AssignmentsController(AssignmentService service)
    {
        _service = service;
    }

    [HttpGet("course/{courseId:guid}")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<AssignmentDto>>>> GetByCourse(Guid courseId, CancellationToken ct)
    {
        var items = await _service.GetByCourseAsync(courseId, ct);
        return Ok(ApiResponse<IReadOnlyList<AssignmentDto>>.Ok(items));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<AssignmentDto>>> GetById(Guid id, CancellationToken ct)
    {
        var item = await _service.GetByIdAsync(id, ct);
        return Ok(ApiResponse<AssignmentDto>.Ok(item));
    }

    [HttpPost]
    [Authorize(Roles = "Admin,SuperAdmin,Mentor")]
    public async Task<ActionResult<ApiResponse<AssignmentDto>>> Create([FromBody] CreateAssignmentRequest request, CancellationToken ct)
    {
        var item = await _service.CreateAsync(request, ct);
        return Ok(ApiResponse<AssignmentDto>.Ok(item, "Assignment created successfully."));
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin,SuperAdmin,Mentor")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(Guid id, CancellationToken ct)
    {
        await _service.DeleteAsync(id, ct);
        return Ok(ApiResponse<object>.Ok(new { }, "Assignment deleted."));
    }

    [HttpPost("{id:guid}/submit")]
    public async Task<ActionResult<ApiResponse<AssignmentSubmissionDto>>> Submit(
        Guid id, [FromBody] SubmitAssignmentRequest request, CancellationToken ct)
    {
        var submission = await _service.SubmitAsync(id, request, ct);
        return Ok(ApiResponse<AssignmentSubmissionDto>.Ok(submission, "Assignment submitted successfully."));
    }

    [HttpGet("{id:guid}/submissions")]
    [Authorize(Roles = "Admin,SuperAdmin,Mentor")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<AssignmentSubmissionDto>>>> GetSubmissions(Guid id, CancellationToken ct)
    {
        var items = await _service.GetSubmissionsForAssignmentAsync(id, ct);
        return Ok(ApiResponse<IReadOnlyList<AssignmentSubmissionDto>>.Ok(items));
    }

    [HttpGet("my-submissions")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<AssignmentSubmissionDto>>>> GetMySubmissions(CancellationToken ct)
    {
        var items = await _service.GetMySubmissionsAsync(ct);
        return Ok(ApiResponse<IReadOnlyList<AssignmentSubmissionDto>>.Ok(items));
    }

    [HttpPost("submissions/{submissionId:guid}/grade")]
    [Authorize(Roles = "Admin,SuperAdmin,Mentor")]
    public async Task<ActionResult<ApiResponse<AssignmentSubmissionDto>>> Grade(
        Guid submissionId, [FromBody] GradeAssignmentRequest request, CancellationToken ct)
    {
        var graded = await _service.GradeSubmissionAsync(submissionId, request, ct);
        return Ok(ApiResponse<AssignmentSubmissionDto>.Ok(graded, "Submission graded and student notified."));
    }
}

