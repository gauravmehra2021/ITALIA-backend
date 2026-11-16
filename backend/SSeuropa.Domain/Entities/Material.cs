using SSeuropa.Domain.Common;
using SSeuropa.Domain.Enums;

namespace SSeuropa.Domain.Entities;

public class Material : EntityBase
{
    public Guid CourseId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public MaterialType Type { get; set; } = MaterialType.Pdf;
    public string StorageKey { get; set; } = string.Empty;
    public bool IsProtected { get; set; } = true;
    public int SortOrder { get; set; }
    public string ChapterTitle { get; set; } = "Chapter 1: Getting Started";
    public int ChapterNumber { get; set; } = 1;
    public string? VideoUrl { get; set; }
    public string? AudioUrl { get; set; }
    public string? PdfUrl { get; set; }
    public string? ContentText { get; set; }
    public string? PracticalTask { get; set; }
    public int DurationMinutes { get; set; } = 15;
}
