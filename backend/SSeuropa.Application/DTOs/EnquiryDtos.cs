using SSeuropa.Domain.Enums;

namespace SSeuropa.Application.DTOs;

public record EnquiryDto(
    Guid Id,
    string FullName,
    string Email,
    string Phone,
    string InterestedCourse,
    string PreferredLanguage,
    string Message,
    EnquiryStatus Status,
    string? StaffNotes,
    Guid? AssignedStaffId,
    string? AssignedStaffName,
    Guid? ConvertedUserId,
    DateTime CreatedAtUtc,
    DateTime? UpdatedAtUtc);

public record CreateEnquiryRequest(
    string FullName,
    string Email,
    string Phone,
    string InterestedCourse,
    string? PreferredLanguage,
    string Message);

public record UpdateEnquiryStatusRequest(
    EnquiryStatus Status,
    string? StaffNotes,
    Guid? AssignedStaffId,
    string? AssignedStaffName);
