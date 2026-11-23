using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SSeuropa.Application.Common;
using SSeuropa.Application.DTOs;
using SSeuropa.Application.Services;
using SSeuropa.Domain.Enums;

namespace SSeuropa.Api.Controllers;

/// <summary>Course builder for Teachers, Admins and the Super Admin.</summary>
[ApiController]
[Authorize(Roles = "Mentor,Admin,SuperAdmin")]
[Route("api/curriculum")]
public class CurriculumController(CurriculumService service) : ControllerBase
{
    [HttpGet("courses/{courseId:guid}")]
    public async Task<ActionResult<ApiResponse<CourseOutlineDto>>> Outline(Guid courseId, CancellationToken ct)
        => Ok(ApiResponse<CourseOutlineDto>.Ok(await service.GetStaffOutlineAsync(courseId, ct)));

    /// <summary>Built-in illustrations (wwwroot/content/img) that teachers can pick for words and units.</summary>
    [HttpGet("illustrations")]
    public ActionResult<ApiResponse<List<IllustrationDto>>> Illustrations([FromServices] IWebHostEnvironment env)
    {
        var dir = Path.Combine(env.WebRootPath ?? Path.Combine(env.ContentRootPath, "wwwroot"), "content", "img");
        var items = Directory.Exists(dir)
            ? Directory.GetFiles(dir, "*.png")
                .Select(Path.GetFileNameWithoutExtension)
                .OrderBy(n => n)
                .Select(n => new IllustrationDto(n!.Replace('-', ' '), $"/content/img/{n}.png"))
                .ToList()
            : [];
        return Ok(ApiResponse<List<IllustrationDto>>.Ok(items));
    }

    [HttpGet("courses/{courseId:guid}/students")]
    public async Task<ActionResult<ApiResponse<List<StudentCourseProgressDto>>>> Students(Guid courseId, CancellationToken ct)
        => Ok(ApiResponse<List<StudentCourseProgressDto>>.Ok(await service.GetStudentProgressAsync(courseId, ct)));

    [HttpPost("courses/{courseId:guid}/publish-all")]
    public async Task<ActionResult<ApiResponse<object>>> PublishAll(Guid courseId, CancellationToken ct)
    {
        await service.SetCurriculumStatusAsync(courseId, LessonStatus.Published, ct);
        return Ok(ApiResponse<object>.Ok(new { }, "All units and lessons are published."));
    }

    [HttpPost("courses/{courseId:guid}/units")]
    public async Task<ActionResult<ApiResponse<UnitDto>>> CreateUnit(Guid courseId, [FromBody] SaveUnitRequest request, CancellationToken ct)
        => Ok(ApiResponse<UnitDto>.Ok(await service.CreateUnitAsync(courseId, request, ct), "Unit created."));

    [HttpPut("units/{unitId:guid}")]
    public async Task<ActionResult<ApiResponse<UnitDto>>> UpdateUnit(Guid unitId, [FromBody] SaveUnitRequest request, CancellationToken ct)
        => Ok(ApiResponse<UnitDto>.Ok(await service.UpdateUnitAsync(unitId, request, ct), "Unit saved."));

    [HttpPost("units/{unitId:guid}/move")]
    public async Task<ActionResult<ApiResponse<object>>> MoveUnit(Guid unitId, [FromBody] MoveRequest request, CancellationToken ct)
    {
        await service.MoveUnitAsync(unitId, request.Direction, ct);
        return Ok(ApiResponse<object>.Ok(new { }));
    }

    [HttpDelete("units/{unitId:guid}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteUnit(Guid unitId, CancellationToken ct)
    {
        await service.DeleteUnitAsync(unitId, ct);
        return Ok(ApiResponse<object>.Ok(new { }, "Unit deleted."));
    }

    [HttpPost("units/{unitId:guid}/lessons")]
    public async Task<ActionResult<ApiResponse<LessonDto>>> CreateLesson(Guid unitId, [FromBody] SaveLessonRequest request, CancellationToken ct)
        => Ok(ApiResponse<LessonDto>.Ok(await service.CreateLessonAsync(unitId, request, ct), "Lesson created."));

    [HttpGet("lessons/{lessonId:guid}")]
    public async Task<ActionResult<ApiResponse<LessonDto>>> GetLesson(Guid lessonId, CancellationToken ct)
        => Ok(ApiResponse<LessonDto>.Ok(await service.GetLessonAsync(lessonId, ct)));

    [HttpPut("lessons/{lessonId:guid}")]
    public async Task<ActionResult<ApiResponse<LessonDto>>> UpdateLesson(Guid lessonId, [FromBody] SaveLessonRequest request, CancellationToken ct)
        => Ok(ApiResponse<LessonDto>.Ok(await service.UpdateLessonAsync(lessonId, request, ct), "Lesson saved."));

    [HttpPost("lessons/{lessonId:guid}/move")]
    public async Task<ActionResult<ApiResponse<object>>> MoveLesson(Guid lessonId, [FromBody] MoveRequest request, CancellationToken ct)
    {
        await service.MoveLessonAsync(lessonId, request.Direction, ct);
        return Ok(ApiResponse<object>.Ok(new { }));
    }

    [HttpDelete("lessons/{lessonId:guid}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteLesson(Guid lessonId, CancellationToken ct)
    {
        await service.DeleteLessonAsync(lessonId, ct);
        return Ok(ApiResponse<object>.Ok(new { }, "Lesson deleted."));
    }
}

/// <summary>Student learning API, shared by the student web portal (and later the mobile app).</summary>
[ApiController]
[Authorize]
[Route("api/learn")]
public class LearnController(CurriculumService service, CourseService courses, TranslationService translations, Application.Interfaces.ICurrentUserService current) : ControllerBase
{
    /// <summary>
    /// Translations (Punjabi, Hindi or Urdu) of the English content texts the student has on screen.
    /// Returns { text: translation } only for texts that have a translation.
    /// </summary>
    [HttpPost("translations")]
    public async Task<ActionResult<ApiResponse<Dictionary<string, string>>>> Translations([FromBody] TranslateTextsRequest request, CancellationToken ct)
        => Ok(ApiResponse<Dictionary<string, string>>.Ok(await translations.TranslateAsync(request, ct)));

    [HttpGet("me")]
    public async Task<ActionResult<ApiResponse<LearnerSummaryDto>>> Me(CancellationToken ct)
        => Ok(ApiResponse<LearnerSummaryDto>.Ok(await service.GetLearnerSummaryAsync(ct)));

    /// <summary>Saves the portal language (English, Punjabi, Hindi or Urdu) on the student's profile.</summary>
    [HttpPut("me/language")]
    public async Task<ActionResult<ApiResponse<string>>> SetLanguage([FromBody] SetLanguageRequest request, CancellationToken ct)
        => Ok(ApiResponse<string>.Ok((await service.SetPreferredLanguageAsync(request, ct)).ToString()));

    [HttpGet("courses")]
    public async Task<ActionResult<ApiResponse<List<CourseCardDto>>>> Catalogue(CancellationToken ct)
        => Ok(ApiResponse<List<CourseCardDto>>.Ok(await service.GetCatalogueAsync(ct)));

    [HttpGet("courses/{courseId:guid}")]
    public async Task<ActionResult<ApiResponse<CourseOutlineDto>>> Outline(Guid courseId, CancellationToken ct)
        => Ok(ApiResponse<CourseOutlineDto>.Ok(await service.GetStudentOutlineAsync(courseId, ct)));

    [HttpPost("courses/{courseId:guid}/enroll")]
    public async Task<ActionResult<ApiResponse<CourseOutlineDto>>> Enroll(Guid courseId, CancellationToken ct)
    {
        var userId = current.UserId ?? throw new Domain.Exceptions.UnauthorizedAccessDomainException();
        try
        {
            await courses.EnrollAsync(courseId, userId, ct);
        }
        catch (Domain.Exceptions.ConflictException)
        {
            // Already enrolled: fine, just return the outline.
        }
        return Ok(ApiResponse<CourseOutlineDto>.Ok(await service.GetStudentOutlineAsync(courseId, ct), "You're enrolled. Buon studio!"));
    }

    [HttpGet("lessons/{lessonId:guid}")]
    public async Task<ActionResult<ApiResponse<LessonDto>>> Lesson(Guid lessonId, CancellationToken ct)
        => Ok(ApiResponse<LessonDto>.Ok(await service.GetLessonAsync(lessonId, ct)));

    [HttpPost("lessons/{lessonId:guid}/complete")]
    public async Task<ActionResult<ApiResponse<LessonResultDto>>> Complete(Guid lessonId, [FromBody] CompleteLessonRequest request, CancellationToken ct)
        => Ok(ApiResponse<LessonResultDto>.Ok(await service.CompleteLessonAsync(lessonId, request, ct)));
}
