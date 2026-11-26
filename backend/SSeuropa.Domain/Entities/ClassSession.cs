using SSeuropa.Domain.Common;
using SSeuropa.Domain.Enums;

namespace SSeuropa.Domain.Entities;

public class ClassSession : EntityBase
{
    /// <summary>Optional for batch classes (Guid.Empty = no particular course).</summary>
    public Guid CourseId { get; set; }
    /// <summary>
    /// The batch this class is for: only its students see the class and get its alerts.
    /// Null only for classes scheduled before classes were per batch (those go to the course's enrolled students).
    /// </summary>
    public Guid? BatchId { get; set; }
    public Guid MentorId { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime ScheduledAtUtc { get; set; }
    public int DurationMinutes { get; set; } = 60;
    public ClassStatus Status { get; set; } = ClassStatus.Scheduled;
    public string? MeetingUrl { get; set; }
    public string? MeetingProvider { get; set; } = "GoogleMeet";
}
