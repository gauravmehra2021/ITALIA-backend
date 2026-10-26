using SSeuropa.Domain.Common;
using SSeuropa.Domain.Enums;

namespace SSeuropa.Domain.Entities;

public class Enquiry : EntityBase
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string InterestedCourse { get; set; } = string.Empty;
    public string PreferredLanguage { get; set; } = "English";
    public string Message { get; set; } = string.Empty;
    public EnquiryStatus Status { get; set; } = EnquiryStatus.New;
    public string? StaffNotes { get; set; }
    public Guid? AssignedStaffId { get; set; }
    public string? AssignedStaffName { get; set; }
    public Guid? ConvertedUserId { get; set; }
}
