using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SSeuropa.Application.Common;
using SSeuropa.Application.DTOs;
using SSeuropa.Application.Interfaces;
using SSeuropa.Application.Services;

namespace SSeuropa.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class CoursesController(
    CourseService courseService,
    IValidator<CreateCourseRequest> createValidator,
    ICurrentUserService currentUser) : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<PagedResult<CourseDto>>>> Get(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null,
        [FromQuery] string? sortBy = null,
        [FromQuery] bool sortDescending = false,
        CancellationToken ct = default)
    {
        var result = await courseService.GetCoursesAsync(new PagedQuery
        {
            Page = page,
            PageSize = pageSize,
            Search = search,
            SortBy = sortBy,
            SortDescending = sortDescending
        }, ct);
        return Ok(ApiResponse<PagedResult<CourseDto>>.Ok(result));
    }

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<CourseDto>>> GetById(Guid id, CancellationToken ct)
    {
        var course = await courseService.GetByIdAsync(id, ct);
        return Ok(ApiResponse<CourseDto>.Ok(course));
    }

    [HttpPost]
    [Authorize(Roles = "Mentor,Admin,SuperAdmin")]
    public async Task<ActionResult<ApiResponse<CourseDto>>> Create(CreateCourseRequest request, CancellationToken ct)
    {
        await createValidator.ValidateAndThrowAsync(request, ct);
        var course = await courseService.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetById), new { id = course.Id }, ApiResponse<CourseDto>.Ok(course, "Course created."));
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Mentor,Admin,SuperAdmin")]
    public async Task<ActionResult<ApiResponse<CourseDto>>> Update(Guid id, UpdateCourseRequest request, CancellationToken ct)
    {
        var course = await courseService.UpdateAsync(id, request, ct);
        return Ok(ApiResponse<CourseDto>.Ok(course, "Course updated."));
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(Guid id, CancellationToken ct)
    {
        await courseService.DeleteAsync(id, ct);
        return Ok(ApiResponse<object>.Ok(new { }, "Course deleted."));
    }

    [HttpPost("{id:guid}/enroll")]
    public async Task<ActionResult<ApiResponse<EnrollmentDto>>> Enroll(Guid id, CancellationToken ct)
    {
        if (currentUser.UserId is null) return Unauthorized();
        var enrollment = await courseService.EnrollAsync(id, currentUser.UserId.Value, ct);
        return Ok(ApiResponse<EnrollmentDto>.Ok(enrollment, "Enrolled."));
    }
}
