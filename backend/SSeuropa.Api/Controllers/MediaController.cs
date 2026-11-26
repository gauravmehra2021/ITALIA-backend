using SSeuropa.Api.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SSeuropa.Application.Common;
using SSeuropa.Application.DTOs;
using SSeuropa.Api.Storage;

namespace SSeuropa.Api.Controllers;

[ApiController]
[Route("api/media")]
public class MediaController : ControllerBase
{
    private readonly IWebHostEnvironment _env;
    private readonly IConfiguration _config;

    private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        // Videos
        ".mp4", ".webm", ".mov", ".mkv",
        // Audios
        ".mp3", ".wav", ".m4a", ".aac", ".ogg",
        // Documents
        ".pdf", ".docx", ".pptx", ".zip",
        // Images
        ".jpg", ".jpeg", ".png", ".webp", ".gif", ".svg"
    };

    public MediaController(IWebHostEnvironment env, IConfiguration config)
    {
        _env = env;
        _config = config;
    }

    [HttpPost("upload")]
    [Authorize(Roles = "SuperAdmin,Admin,Staff,Mentor")]
    [TeamModule(TeamPermissions.Courses, RequireFull = true)]
    [RequestSizeLimit(UploadStorage.MaxUploadBytes)] // 500 MB max for video lectures
    [RequestFormLimits(MultipartBodyLengthLimit = UploadStorage.MaxUploadBytes)]
    public async Task<ActionResult<ApiResponse<FileUploadResponse>>> Upload(
        [FromForm] IFormFile? file,
        [FromForm] string? category,
        CancellationToken ct)
    {
        if (file is null || file.Length == 0)
        {
            return BadRequest(ApiResponse<FileUploadResponse>.Fail("No file was uploaded."));
        }

        var ext = Path.GetExtension(file.FileName);
        if (string.IsNullOrEmpty(ext) || !AllowedExtensions.Contains(ext))
        {
            return BadRequest(ApiResponse<FileUploadResponse>.Fail(
                $"File extension '{ext}' is not supported. Allowed: {string.Join(", ", AllowedExtensions)}"));
        }

        var sanitizedCategory = string.IsNullOrWhiteSpace(category) ? "general" : category.Trim().ToLowerInvariant();
        sanitizedCategory = new string(sanitizedCategory.Where(c => char.IsLetterOrDigit(c) || c == '-').ToArray());

        if (string.IsNullOrEmpty(sanitizedCategory)) sanitizedCategory = "general";
        var uploadsRoot = Path.Combine(UploadStorage.Root(_config, _env), sanitizedCategory);
        Directory.CreateDirectory(uploadsRoot);

        var cleanFileName = Path.GetFileNameWithoutExtension(file.FileName);
        cleanFileName = new string(cleanFileName.Where(c => char.IsLetterOrDigit(c) || c == '_' || c == '-').ToArray());
        if (string.IsNullOrEmpty(cleanFileName)) cleanFileName = "file";

        var storedFileName = $"{Guid.NewGuid():N}_{cleanFileName}{ext.ToLowerInvariant()}";
        var fullPath = Path.Combine(uploadsRoot, storedFileName);

        await using (var stream = new FileStream(fullPath, FileMode.Create))
        {
            await file.CopyToAsync(stream, ct);
        }

        // Public link: Storage:PublicBaseUrl on a live server, otherwise the (proxy-aware) request address.
        var relativeUrl = $"/uploads/{sanitizedCategory}/{storedFileName}";
        var absoluteUrl = UploadStorage.PublicUrl(_config, HttpContext.Request, relativeUrl);

        var response = new FileUploadResponse(
            Url: absoluteUrl,
            FileName: file.FileName,
            FileSize: file.Length,
            ContentType: file.ContentType
        );

        return Ok(ApiResponse<FileUploadResponse>.Ok(response, "File uploaded successfully. Ready for playback and download."));
    }
}

