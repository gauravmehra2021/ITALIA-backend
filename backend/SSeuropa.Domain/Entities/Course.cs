using SSeuropa.Domain.Common;
using SSeuropa.Domain.Enums;

namespace SSeuropa.Domain.Entities;

public class Course : EntityBase
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public CourseLevel Level { get; set; } = CourseLevel.Beginner;
    public CourseStatus Status { get; set; } = CourseStatus.Draft;
    public int Capacity { get; set; } = 30;
    public int EnrolledCount { get; set; }
    public Guid? MentorId { get; set; }
    public string Language { get; set; } = "Italian";
    public decimal Price { get; set; }
    public string Currency { get; set; } = "EUR";
    public string? ThumbnailUrl { get; set; }
    public int DurationWeeks { get; set; } = 8;
}
