using SSeuropa.Api.Security;
using SSeuropa.Application.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SSeuropa.Application.Services;

namespace SSeuropa.Api.Controllers;

/// <summary>Teachers and admins: Punjabi / Hindi / Urdu translations of the lesson content.</summary>
[ApiController]
[Authorize(Roles = "SuperAdmin,Admin,Staff,Mentor")]
[TeamModule(TeamPermissions.Translations)]
[Route("api/translations")]
public class TranslationsController(TranslationService service) : ControllerBase
{
    /// <summary>
    /// The English texts of one course (or all courses) with their translations.
    /// missing = punjabi | hindi | urdu | any shows only the texts still to translate.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<TranslationPageDto>>> List(
        [FromQuery] Guid? courseId, [FromQuery] string? missing, [FromQuery] string? search,
        [FromQuery] int page = 1, [FromQuery] int pageSize = 50, CancellationToken ct = default)
        => Ok(ApiResponse<TranslationPageDto>.Ok(await service.ListAsync(new TranslationQuery(courseId, missing, search, page, pageSize), ct)));

    /// <summary>Saves the Punjabi, Hindi and Urdu translation of one English text (empty = remove).</summary>
    [HttpPut]
    public async Task<ActionResult<ApiResponse<TranslationItemDto>>> Save([FromBody] SaveTranslationRequest request, CancellationToken ct)
        => Ok(ApiResponse<TranslationItemDto>.Ok(await service.SaveAsync(request, ct), "Translation saved."));
}
