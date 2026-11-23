using SSeuropa.Domain.Common;

namespace SSeuropa.Domain.Entities;

/// <summary>
/// A group of students who learn together (shown as "Batch" in the admin panel, so it is not
/// confused with live class sessions). The courses assigned to a batch are the only courses its
/// students can open, apart from courses marked Course.OpenToAll.
/// </summary>
public class Batch : EntityBase
{
    public string Name { get; set; } = string.Empty;
    /// <summary>Level of the group, e.g. "A0", "A1", "A2".</summary>
    public string Level { get; set; } = "A0";
    public string Description { get; set; } = string.Empty;
    /// <summary>Teacher in charge (role Mentor). Teachers see only the batches they lead.</summary>
    public Guid? MentorId { get; set; }
    public List<Guid> CourseIds { get; set; } = [];
    public DateTime? StartDateUtc { get; set; }
}
