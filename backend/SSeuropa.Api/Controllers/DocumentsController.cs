using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SSeuropa.Api.Storage;
using SSeuropa.Application.Common;
using SSeuropa.Application.Interfaces;
using SSeuropa.Application.Services;
using SSeuropa.Domain.Enums;

namespace SSeuropa.Api.Controllers;

/// <summary>
/// Syllabus PDFs are viewed through short-lived links only (see <see cref="ProtectedDocuments"/>).
/// </summary>
[ApiController]
[Route("api/documents")]
public class DocumentsController(ProtectedDocuments documents, IWebHostEnvironment env) : ControllerBase
{
    public sealed record LinkRequest(string Url);
    public sealed record LinkResponse(string Url, DateTime ExpiresAtUtc);

    /// <summary>A link to a syllabus PDF that works for 10 minutes, for someone who may see its course.</summary>
    [HttpPost("link")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<LinkResponse>>> Link(
        [FromBody] LinkRequest request, [FromServices] ICurrentUserService current, [FromServices] CourseAccessService access, CancellationToken ct)
    {
        var path = ProtectedDocuments.Normalize(request.Url);
        if (path is null || documents.FileFor(path, env) is null)
            return NotFound(ApiResponse<LinkResponse>.Fail("Document not found."));

        var team = current.IsInRole(UserRole.Admin, UserRole.SuperAdmin, UserRole.Staff, UserRole.Mentor);
        if (!team)
        {
            var allowed = false;
            foreach (var courseId in await documents.CoursesUsingAsync(path, ct))
                if (await access.CanSeeAsync(current.UserId!.Value, courseId, ct)) { allowed = true; break; }
            if (!allowed) return StatusCode(StatusCodes.Status403Forbidden, ApiResponse<LinkResponse>.Fail("This document belongs to a course you can't open."));
        }

        return Ok(ApiResponse<LinkResponse>.Ok(new LinkResponse(documents.SignedLink(path), DateTime.UtcNow.Add(ProtectedDocuments.LinkLifetime))));
    }

    /// <summary>The PDF behind a signed link: shown in the page, never cached.</summary>
    [HttpGet("view")]
    [AllowAnonymous]
    public IActionResult View([FromQuery] string p, [FromQuery] long e, [FromQuery] string s)
    {
        var path = ProtectedDocuments.Normalize(p);
        if (path is null || !documents.IsValid(path, e, s)) return StatusCode(StatusCodes.Status403Forbidden);
        var file = documents.FileFor(path, env);
        if (file is null) return NotFound();

        Response.Headers.CacheControl = "no-store, private";
        Response.Headers.ContentDisposition = "inline";
        Response.Headers["X-Content-Type-Options"] = "nosniff";
        Response.Headers["Access-Control-Allow-Origin"] = "*";
        return PhysicalFile(file, "application/pdf", enableRangeProcessing: true);
    }
}
