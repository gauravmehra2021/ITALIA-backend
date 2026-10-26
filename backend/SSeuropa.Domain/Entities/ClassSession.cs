using SSeuropa.Domain.Common;
using SSeuropa.Domain.Enums;

namespace SSeuropa.Domain.Entities;

public class ClassSession : EntityBase
{
    public Guid CourseId { get; set; }
    public Guid MentorId { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime ScheduledAtUtc { get; set; }
    public int DurationMinutes { get; set; } = 60;
    public ClassStatus Status { get; set; } = ClassStatus.Scheduled;
    public string? MeetingUrl { get; set; }
    public string? MeetingProvider { get; set; } = "GoogleMeet";
}
