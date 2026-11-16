using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SSeuropa.Application.Common;
using SSeuropa.Application.DTOs;
using SSeuropa.Application.Interfaces;
using SSeuropa.Domain.Entities;

namespace SSeuropa.Api.Controllers;

[ApiController]
[Route("api/uploads")]
public class UploadsController(
    IAwsS3Service s3Service,
    ILearningResourceRepository resourceRepo,
    ICourseRepository courseRepo,
    IMaterialRepository materialRepo,
    ICurrentUserService currentUser,
    IWebHostEnvironment env,
    ILogger<UploadsController> logger) : ControllerBase
{
    private static readonly HashSet<string> AllowedMimeTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        // Video
        "video/mp4", "video/webm", "video/quicktime", "video/x-matroska",
        // Audio
        "audio/mpeg", "audio/mp3", "audio/wav", "audio/x-m4a", "audio/aac", "audio/ogg",
        // Documents
        "application/pdf", "application/msword",
        "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
        "application/zip", "application/x-zip-compressed",
        // Images
        "image/jpeg", "image/png", "image/webp", "image/gif", "image/svg+xml"
    };

    [HttpPost("s3/presigned-url")]
    [Authorize(Roles = "Mentor,Admin,SuperAdmin")]
    public async Task<ActionResult<ApiResponse<PresignedUploadResponseDto>>> GetPresignedUrl(
        [FromBody] PresignedUploadRequestDto request,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.FileName))
        {
            return BadRequest(ApiResponse<PresignedUploadResponseDto>.Fail("File name is required."));
        }

        // Validate MIME type
        if (!AllowedMimeTypes.Contains(request.MimeType))
        {
            return BadRequest(ApiResponse<PresignedUploadResponseDto>.Fail(
                $"MIME type '{request.MimeType}' is not permitted for learning content."));
        }

        // Validate File Size (e.g. 500 MB max for videos, 50MB for others)
        var maxLimit = request.Category == "videos" ? 500L * 1024 * 1024 : 50L * 1024 * 1024;
        if (request.FileSize > maxLimit)
        {
            return BadRequest(ApiResponse<PresignedUploadResponseDto>.Fail(
                $"File exceeds maximum permitted size of {maxLimit / (1024 * 1024)} MB."));
        }

        var course = await courseRepo.GetByIdAsync(request.CourseId, ct);
        var courseSlug = !string.IsNullOrWhiteSpace(course?.Slug) ? course.Slug : $"course-{request.CourseId:N}";
        var cleanFileName = SanitizeFileName(request.FileName);

        var subPath = request.Category switch
        {
            "covers" => $"courses/{courseSlug}/covers",
            "videos" => $"courses/{courseSlug}/lessons/video",
            "audios" => $"courses/{courseSlug}/lessons/audio",
            "documents" => $"courses/{courseSlug}/lessons/documents",
            _ => $"courses/{courseSlug}/resources"
        };

        var objectKey = $"{subPath}/{Guid.NewGuid():N}_{cleanFileName}";
        var ttl = TimeSpan.FromHours(1);

        var response = await s3Service.GeneratePresignedUploadUrlAsync(
            objectKey,
            request.MimeType,
            request.FileSize,
            ttl,
            ct);

        logger.LogInformation("Generated S3 presigned upload URL for {ObjectKey} by user {UserId}", objectKey, currentUser.UserId);
        return Ok(ApiResponse<PresignedUploadResponseDto>.Ok(response, "S3 upload authorization granted."));
    }

    [HttpPost("s3/multipart/initiate")]
    [Authorize(Roles = "Mentor,Admin,SuperAdmin")]
    public async Task<ActionResult<ApiResponse<InitiateMultipartResponseDto>>> InitiateMultipart(
        [FromBody] InitiateMultipartRequestDto request,
        CancellationToken ct)
    {
        var course = await courseRepo.GetByIdAsync(request.CourseId, ct);
        var courseSlug = !string.IsNullOrWhiteSpace(course?.Slug) ? course.Slug : $"course-{request.CourseId:N}";
        var cleanFileName = SanitizeFileName(request.FileName);
        var objectKey = $"courses/{courseSlug}/videos/{Guid.NewGuid():N}_{cleanFileName}";

        var response = await s3Service.InitiateMultipartUploadAsync(objectKey, request.MimeType, request.FileSize, ct);
        return Ok(ApiResponse<InitiateMultipartResponseDto>.Ok(response, "Multipart upload initiated."));
    }

    [HttpPost("s3/multipart/presigned-part")]
    [Authorize(Roles = "Mentor,Admin,SuperAdmin")]
    public async Task<ActionResult<ApiResponse<string>>> GetPresignedPartUrl(
        [FromQuery] string objectKey,
        [FromQuery] string uploadId,
        [FromQuery] int partNumber,
        CancellationToken ct)
    {
        var url = await s3Service.GeneratePresignedPartUploadUrlAsync(objectKey, uploadId, partNumber, TimeSpan.FromHours(1), ct);
        return Ok(ApiResponse<string>.Ok(url));
    }

    [HttpPost("s3/multipart/complete")]
    [Authorize(Roles = "Mentor,Admin,SuperAdmin")]
    public async Task<ActionResult<ApiResponse<CompleteMultipartResponseDto>>> CompleteMultipart(
        [FromBody] CompleteMultipartRequestDto request,
        CancellationToken ct)
    {
        var result = await s3Service.CompleteMultipartUploadAsync(request.ObjectKey, request.UploadId, request.Parts, ct);
        return Ok(ApiResponse<CompleteMultipartResponseDto>.Ok(result, "Multipart upload completed."));
    }

    [HttpPost("s3/confirm")]
    [Authorize(Roles = "Mentor,Admin,SuperAdmin")]
    public async Task<ActionResult<ApiResponse<ConfirmUploadResponseDto>>> ConfirmUpload(
        [FromBody] ConfirmUploadRequestDto request,
        CancellationToken ct)
    {
        // 1. Verify object exists if S3 is active
        var exists = await s3Service.DoesObjectExistAsync(request.ObjectKey, ct);
        if (!exists)
        {
            logger.LogWarning("ConfirmUpload failed: Object {ObjectKey} was not found on S3.", request.ObjectKey);
            return BadRequest(ApiResponse<ConfirmUploadResponseDto>.Fail("Uploaded object was not found in S3 storage. Please retry upload."));
        }

        // 2. Persist metadata in LearningResource collection
        var resource = new LearningResource
        {
            CourseId = request.CourseId,
            ModuleId = request.ModuleId,
            ChapterId = request.ChapterId,
            LessonId = request.LessonId,
            FileName = request.FileName,
            OriginalFileName = request.OriginalFileName,
            FileType = request.FileType,
            MimeType = request.MimeType,
            FileSize = request.FileSize,
            BucketName = s3Service.BucketName,
            ObjectKey = request.ObjectKey,
            Region = s3Service.Region,
            UploadedBy = currentUser.UserId,
            DurationSeconds = request.DurationSeconds,
            Status = "Active"
        };

        await resourceRepo.AddAsync(resource, ct);

        // 3. Generate initial playback/view access URL
        var accessUrl = await s3Service.GeneratePresignedDownloadUrlAsync(request.ObjectKey, TimeSpan.FromHours(4), ct);

        // 4. If associated with an existing Material (lesson), link it automatically
        if (request.LessonId.HasValue)
        {
            var material = await materialRepo.GetByIdAsync(request.LessonId.Value, ct);
            if (material is not null)
            {
                material.S3ObjectKey = request.ObjectKey;
                material.MimeType = request.MimeType;
                material.FileSize = request.FileSize;

                if (request.FileType.Equals("video", StringComparison.OrdinalIgnoreCase))
                    material.VideoUrl = accessUrl;
                else if (request.FileType.Equals("audio", StringComparison.OrdinalIgnoreCase))
                    material.AudioUrl = accessUrl;
                else if (request.FileType.Equals("pdf", StringComparison.OrdinalIgnoreCase) || request.FileType.Equals("document", StringComparison.OrdinalIgnoreCase))
                    material.PdfUrl = accessUrl;

                await materialRepo.UpdateAsync(material, ct);
            }
        }

        var response = new ConfirmUploadResponseDto(
            ResourceId: resource.Id,
            ObjectKey: request.ObjectKey,
            AccessUrl: accessUrl,
            Message: "Learning resource metadata persisted and ready for student streaming."
        );

        logger.LogInformation("Confirmed S3 upload for resource {ResourceId} (ObjectKey: {ObjectKey})", resource.Id, request.ObjectKey);
        return Ok(ApiResponse<ConfirmUploadResponseDto>.Ok(response, "Resource verified and saved."));
    }

    [HttpGet("s3/access-url")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<S3AccessUrlResponseDto>>> GetAccessUrl(
        [FromQuery] string objectKey,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(objectKey))
        {
            return BadRequest(ApiResponse<S3AccessUrlResponseDto>.Fail("ObjectKey is required."));
        }

        var expiresAt = DateTime.UtcNow.AddHours(2);
        var accessUrl = await s3Service.GeneratePresignedDownloadUrlAsync(objectKey, TimeSpan.FromHours(2), ct);

        return Ok(ApiResponse<S3AccessUrlResponseDto>.Ok(new S3AccessUrlResponseDto(
            ObjectKey: objectKey,
            AccessUrl: accessUrl,
            ExpiresAtUtc: expiresAt
        )));
    }

    [HttpPut("local-direct")]
    [AllowAnonymous]
    [RequestSizeLimit(500 * 1024 * 1024)]
    public async Task<IActionResult> LocalDirectUpload([FromQuery] string key, CancellationToken ct)
    {
        var sanitizedKey = key.TrimStart('/').Replace('\\', '/');
        var fullPath = Path.Combine(env.ContentRootPath, "uploads", sanitizedKey);
        var dir = Path.GetDirectoryName(fullPath);
        if (dir is not null && !Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }

        await using var targetStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write);
        await Request.Body.CopyToAsync(targetStream, ct);

        return Ok(new { success = true, key = sanitizedKey });
    }

    private static string SanitizeFileName(string fileName)
    {
        var name = Path.GetFileNameWithoutExtension(fileName);
        var ext = Path.GetExtension(fileName).ToLowerInvariant();
        name = new string(name.Where(c => char.IsLetterOrDigit(c) || c == '_' || c == '-').ToArray());
        if (string.IsNullOrWhiteSpace(name)) name = "asset";
        return $"{name}{ext}";
    }
}

