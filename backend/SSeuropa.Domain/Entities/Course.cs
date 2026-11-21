using SSeuropa.Domain.Common;
using SSeuropa.Domain.Enums;

namespace SSeuropa.Domain.Entities;

public class Course : EntityBase
{
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string ShortDescription { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Objectives { get; set; } = string.Empty;
    public CourseLevel Level { get; set; } = CourseLevel.Beginner;
    public CourseStatus Status { get; set; } = CourseStatus.Draft;
    public int Capacity { get; set; } = 30;
    public int EnrolledCount { get; set; }
    public Guid? MentorId { get; set; }
    public string Language { get; set; } = "Italian";
    public decimal Price { get; set; }
    public string Currency { get; set; } = "EUR";
    public string? ThumbnailUrl { get; set; }
    public string? PaymentUrl { get; set; }
    public int DurationWeeks { get; set; } = 8;
    public int DisplayOrder { get; set; } = 0;
    /// <summary>CEFR level the course targets, e.g. "A1".</summary>
    public string CefrLevel { get; set; } = "A1";
    /// <summary>Core, Work, Study or Visit.</summary>
    public string Category { get; set; } = "Core";
    /// <summary>
    /// Built-in courses only: keys of the units already loaded from the content seed.
    /// New chapters added to the seed later are appended once; a unit an admin deleted
    /// is never loaded again.
    /// </summary>
    public List<string> SeededUnitKeys { get; set; } = [];
}
