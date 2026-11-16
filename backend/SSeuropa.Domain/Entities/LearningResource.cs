using SSeuropa.Domain.Common;

namespace SSeuropa.Domain.Entities;

public class LearningResource : EntityBase
{
    public Guid CourseId { get; set; }
    public Guid? ModuleId { get; set; }
    public Guid? ChapterId { get; set; }
    public Guid? LessonId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string OriginalFileName { get; set; } = string.Empty;
    public string FileType { get; set; } = string.Empty; // Video, Pdf, Audio, Image, Document
    public string MimeType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string BucketName { get; set; } = string.Empty;
    public string ObjectKey { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
    public Guid? UploadedBy { get; set; }
    public string Status { get; set; } = "Active"; // Active, Archived, Processing
    public int? DurationSeconds { get; set; }
    public string? ThumbnailObjectKey { get; set; }
}
