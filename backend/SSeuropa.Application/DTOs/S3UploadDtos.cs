namespace SSeuropa.Application.DTOs;

public record PresignedUploadRequestDto(
    Guid CourseId,
    string FileName,
    string MimeType,
    long FileSize,
    string Category = "general",
    Guid? ModuleId = null,
    Guid? ChapterId = null,
    Guid? LessonId = null);

public record PresignedUploadResponseDto(
    string UploadUrl,
    string ObjectKey,
    string BucketName,
    string Region,
    DateTime ExpiresAtUtc,
    bool IsDirectUpload = true,
    string? UploadId = null);

public record ConfirmUploadRequestDto(
    Guid CourseId,
    string FileName,
    string OriginalFileName,
    string ObjectKey,
    string MimeType,
    long FileSize,
    string FileType,
    Guid? ModuleId = null,
    Guid? ChapterId = null,
    Guid? LessonId = null,
    int? DurationSeconds = null);

public record ConfirmUploadResponseDto(
    Guid ResourceId,
    string ObjectKey,
    string AccessUrl,
    string Message);

public record S3AccessUrlResponseDto(
    string ObjectKey,
    string AccessUrl,
    DateTime ExpiresAtUtc);

public record InitiateMultipartRequestDto(
    Guid CourseId,
    string FileName,
    string MimeType,
    long FileSize,
    string Category = "videos",
    Guid? ModuleId = null,
    Guid? ChapterId = null,
    Guid? LessonId = null);

public record InitiateMultipartResponseDto(
    string UploadId,
    string ObjectKey,
    long PartSize,
    int TotalParts);

public record MultipartPartDto(int PartNumber, string ETag);

public record CompleteMultipartRequestDto(
    string UploadId,
    string ObjectKey,
    IReadOnlyList<MultipartPartDto> Parts);

public record CompleteMultipartResponseDto(
    string ObjectKey,
    string Location,
    string Message);
