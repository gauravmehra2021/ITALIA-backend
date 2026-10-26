using SSeuropa.Application.DTOs;
using SSeuropa.Application.Interfaces;
using SSeuropa.Domain.Entities;
using SSeuropa.Domain.Enums;
using SSeuropa.Domain.Exceptions;

namespace SSeuropa.Application.Services;

public class EnquiryService
{
    private readonly IEnquiryRepository _enquiries;
    private readonly IUserRepository _users;
    private readonly ICurrentUserService _current;
    private readonly IAuditLogRepository _audit;

    public EnquiryService(
        IEnquiryRepository enquiries,
        IUserRepository users,
        ICurrentUserService current,
        IAuditLogRepository audit)
    {
        _enquiries = enquiries;
        _users = users;
        _current = current;
        _audit = audit;
    }

    public async Task<EnquiryDto> SubmitAsync(CreateEnquiryRequest request, CancellationToken ct = default)
    {
        var enquiry = new Enquiry
        {
            FullName = request.FullName.Trim(),
            Email = request.Email.Trim().ToLowerInvariant(),
            Phone = request.Phone.Trim(),
            InterestedCourse = request.InterestedCourse.Trim(),
            PreferredLanguage = string.IsNullOrWhiteSpace(request.PreferredLanguage) ? "English" : request.PreferredLanguage.Trim(),
            Message = request.Message.Trim(),
            Status = EnquiryStatus.New
        };

        await _enquiries.AddAsync(enquiry, ct);

        if (_current.UserId.HasValue)
        {
            await _audit.AddAsync(new AuditLog
            {
                ActorUserId = _current.UserId.Value,
                Action = "submit_enquiry",
                Resource = "enquiry",
                Details = $"Enquiry submitted by {enquiry.FullName} ({enquiry.Email})"
            }, ct);
        }

        return Map(enquiry);
    }

    public async Task<IReadOnlyList<EnquiryDto>> GetAllAsync(CancellationToken ct = default)
    {
        EnsureStaffOrAdmin();

        var list = await _enquiries.GetAllAsync(ct);
        return list.OrderByDescending(e => e.CreatedAtUtc).Select(Map).ToList();
    }

    public async Task<EnquiryDto> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        EnsureStaffOrAdmin();

        var item = await _enquiries.GetByIdAsync(id, ct) ?? throw new NotFoundException("Enquiry", id);
        return Map(item);
    }

    public async Task<EnquiryDto> UpdateStatusAsync(Guid id, UpdateEnquiryStatusRequest request, CancellationToken ct = default)
    {
        EnsureStaffOrAdmin();

        var item = await _enquiries.GetByIdAsync(id, ct) ?? throw new NotFoundException("Enquiry", id);

        item.Status = request.Status;
        if (request.StaffNotes is not null)
        {
            item.StaffNotes = request.StaffNotes.Trim();
        }

        if (request.AssignedStaffId.HasValue)
        {
            item.AssignedStaffId = request.AssignedStaffId.Value;
            if (!string.IsNullOrWhiteSpace(request.AssignedStaffName))
            {
                item.AssignedStaffName = request.AssignedStaffName.Trim();
            }
            else
            {
                var assignedUser = await _users.GetByIdAsync(request.AssignedStaffId.Value, ct);
                item.AssignedStaffName = assignedUser?.FullName;
            }
        }
        else if (item.AssignedStaffId is null && _current.UserId.HasValue)
        {
            // Auto-assign to current staff member if not already assigned
            item.AssignedStaffId = _current.UserId.Value;
            var currentUser = await _users.GetByIdAsync(_current.UserId.Value, ct);
            item.AssignedStaffName = currentUser?.FullName;
        }

        item.UpdatedAtUtc = DateTime.UtcNow;
        await _enquiries.UpdateAsync(item, ct);

        if (_current.UserId.HasValue)
        {
            await _audit.AddAsync(new AuditLog
            {
                ActorUserId = _current.UserId.Value,
                Action = "update_enquiry_status",
                Resource = "enquiry",
                Details = $"Enquiry for {item.FullName} updated to status '{item.Status}' by staff"
            }, ct);
        }

        return Map(item);
    }

    private void EnsureStaffOrAdmin()
    {
        if (_current.UserId is null || !_current.IsInRole(UserRole.Staff, UserRole.Admin, UserRole.SuperAdmin))
        {
            throw new UnauthorizedAccessDomainException();
        }
    }

    private static EnquiryDto Map(Enquiry e) =>
        new(
            e.Id,
            e.FullName,
            e.Email,
            e.Phone,
            e.InterestedCourse,
            e.PreferredLanguage,
            e.Message,
            e.Status,
            e.StaffNotes,
            e.AssignedStaffId,
            e.AssignedStaffName,
            e.ConvertedUserId,
            e.CreatedAtUtc,
            e.UpdatedAtUtc
        );
}
