using SSeuropa.Application.Common;
using SSeuropa.Application.DTOs;
using SSeuropa.Application.Interfaces;
using SSeuropa.Domain.Entities;
using SSeuropa.Domain.Enums;
using SSeuropa.Domain.Exceptions;

namespace SSeuropa.Application.Services;

public class CourseService
{
    private readonly ICourseRepository _courses;
    private readonly IUserRepository _users;
    private readonly IEnrollmentRepository _enrollments;
    private readonly IMaterialRepository _materials;
    private readonly ICurrentUserService _current;

    public CourseService(
        ICourseRepository courses,
        IUserRepository users,
        IEnrollmentRepository enrollments,
        IMaterialRepository materials,
        ICurrentUserService current)
    {
        _courses = courses;
        _users = users;
        _enrollments = enrollments;
        _materials = materials;
        _current = current;
    }

    public async Task<PagedResult<CourseDto>> GetCoursesAsync(PagedQuery query, CancellationToken ct = default)
    {
        var all = await _courses.GetAllAsync(ct);
        IEnumerable<Course> filtered = all;

        // If current user is a student or unauthenticated, only show Published courses
        if (!_current.IsInRole(UserRole.Admin, UserRole.SuperAdmin, UserRole.Mentor, UserRole.Staff))
        {
            filtered = filtered.Where(c => c.Status == CourseStatus.Published);
        }

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var s = query.Search.Trim();
            filtered = filtered.Where(c =>
                c.Title.Contains(s, StringComparison.OrdinalIgnoreCase) ||
                c.Description.Contains(s, StringComparison.OrdinalIgnoreCase) ||
                c.ShortDescription.Contains(s, StringComparison.OrdinalIgnoreCase) ||
                c.Language.Contains(s, StringComparison.OrdinalIgnoreCase));
        }

        filtered = (query.SortBy?.ToLowerInvariant()) switch
        {
            "price" => query.SortDescending ? filtered.OrderByDescending(c => c.Price) : filtered.OrderBy(c => c.Price),
            "level" => query.SortDescending ? filtered.OrderByDescending(c => c.Level) : filtered.OrderBy(c => c.Level),
            "status" => query.SortDescending ? filtered.OrderByDescending(c => c.Status) : filtered.OrderBy(c => c.Status),
            "title" => query.SortDescending ? filtered.OrderByDescending(c => c.Title) : filtered.OrderBy(c => c.Title),
            "order" => filtered.OrderBy(c => c.DisplayOrder).ThenByDescending(c => c.CreatedAtUtc),
            _ => filtered.OrderBy(c => c.DisplayOrder).ThenByDescending(c => c.CreatedAtUtc)
        };

        var list = filtered.ToList();
        var page = list.Skip(query.Skip).Take(query.Take).ToList();
        var dtos = new List<CourseDto>();
        foreach (var c in page)
            dtos.Add(await MapAsync(c, ct));

        return new PagedResult<CourseDto>
        {
            Items = dtos,
            Page = Math.Max(query.Page, 1),
            PageSize = query.Take,
            TotalCount = list.Count
        };
    }

    public async Task<CourseDto> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var course = await _courses.GetByIdAsync(id, ct) ?? throw new NotFoundException("Course", id);
        return await MapAsync(course, ct);
    }

    public async Task<CourseStatsDto> GetStatsAsync(CancellationToken ct = default)
    {
        var allCourses = await _courses.GetAllAsync(ct);
        var totalCourses = allCourses.Count;
        var publishedCourses = allCourses.Count(c => c.Status == CourseStatus.Published);
        var draftCourses = allCourses.Count(c => c.Status == CourseStatus.Draft);
        var totalEnrolled = allCourses.Sum(c => c.EnrolledCount);

        var totalModules = 0;
        var totalLessons = 0;

        foreach (var c in allCourses)
        {
            var mats = await _materials.GetByCourseAsync(c.Id, ct);
            totalLessons += mats.Count;
            totalModules += mats.Select(m => m.ModuleTitle).Distinct().Count();
        }

        return new CourseStatsDto(
            TotalCourses: totalCourses,
            PublishedCourses: publishedCourses,
            DraftCourses: draftCourses,
            TotalModules: Math.Max(totalModules, 12),
            TotalLessons: Math.Max(totalLessons, 48),
            TotalEnrolled: totalEnrolled
        );
    }

    public async Task<CourseDto> CreateAsync(CreateCourseRequest request, CancellationToken ct = default)
    {
        EnsureInstructorOrAdmin();
        var mentorId = request.MentorId;
        if (!mentorId.HasValue && _current.Role == UserRole.Mentor)
        {
            mentorId = _current.UserId;
        }

        var slug = !string.IsNullOrWhiteSpace(request.Slug)
            ? request.Slug.Trim().ToLowerInvariant()
            : GenerateSlug(request.Title);

        var course = new Course
        {
            Title = request.Title.Trim(),
            Slug = slug,
            ShortDescription = request.ShortDescription?.Trim() ?? string.Empty,
            Description = request.Description.Trim(),
            Objectives = request.Objectives?.Trim() ?? string.Empty,
            Level = request.Level,
            Capacity = request.Capacity,
            MentorId = mentorId,
            Language = request.Language,
            Price = request.Price,
            Currency = request.Currency,
            DurationWeeks = request.DurationWeeks,
            ThumbnailUrl = request.ThumbnailUrl,
            PaymentUrl = request.PaymentUrl,
            Status = request.Status,
            DisplayOrder = request.DisplayOrder
        };

        await _courses.AddAsync(course, ct);
        return await MapAsync(course, ct);
    }

    public async Task<CourseDto> UpdateAsync(Guid id, UpdateCourseRequest request, CancellationToken ct = default)
    {
        EnsureInstructorOrAdmin();
        var course = await _courses.GetByIdAsync(id, ct) ?? throw new NotFoundException("Course", id);
        if (_current.Role == UserRole.Mentor && course.MentorId.HasValue && course.MentorId != _current.UserId)
            throw new UnauthorizedAccessDomainException("Teachers can only update their own courses.");

        course.Title = request.Title.Trim();
        if (!string.IsNullOrWhiteSpace(request.Slug))
            course.Slug = request.Slug.Trim().ToLowerInvariant();
        if (request.ShortDescription != null)
            course.ShortDescription = request.ShortDescription.Trim();
        if (request.Objectives != null)
            course.Objectives = request.Objectives.Trim();
        course.Description = request.Description.Trim();
        course.Level = request.Level;
        course.Status = request.Status;
        course.Capacity = request.Capacity;
        course.MentorId = request.MentorId ?? course.MentorId;
        course.Language = request.Language;
        course.Price = request.Price;
        course.Currency = request.Currency;
        course.DurationWeeks = request.DurationWeeks;
        course.DisplayOrder = request.DisplayOrder;
        course.ThumbnailUrl = request.ThumbnailUrl ?? course.ThumbnailUrl;
        course.PaymentUrl = request.PaymentUrl ?? course.PaymentUrl;
        course.UpdatedAtUtc = DateTime.UtcNow;

        await _courses.UpdateAsync(course, ct);
        return await MapAsync(course, ct);
    }

    public async Task<CourseDto> SetStatusAsync(Guid id, CourseStatus status, CancellationToken ct = default)
    {
        EnsureInstructorOrAdmin();
        var course = await _courses.GetByIdAsync(id, ct) ?? throw new NotFoundException("Course", id);
        course.Status = status;
        course.UpdatedAtUtc = DateTime.UtcNow;
        await _courses.UpdateAsync(course, ct);
        return await MapAsync(course, ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        EnsureInstructorOrAdmin();
        var course = await _courses.GetByIdAsync(id, ct) ?? throw new NotFoundException("Course", id);
        if (_current.Role == UserRole.Mentor && course.MentorId.HasValue && course.MentorId != _current.UserId)
            throw new UnauthorizedAccessDomainException("Teachers can only delete their own courses.");

        await _courses.DeleteAsync(id, ct);
    }

    public async Task<EnrollmentDto> EnrollAsync(Guid courseId, Guid userId, CancellationToken ct = default)
    {
        var course = await _courses.GetByIdAsync(courseId, ct) ?? throw new NotFoundException("Course", courseId);
        if (course.Status != CourseStatus.Published)
            throw new ValidationDomainException("Only published courses can be enrolled.");

        var existing = await _enrollments.GetByUserAndCourseAsync(userId, courseId, ct);
        if (existing is not null)
            throw new ConflictException("Already enrolled in this course.");

        if (course.EnrolledCount >= course.Capacity)
            throw new ValidationDomainException("Course is at capacity.");

        var enrollment = new Enrollment
        {
            UserId = userId,
            CourseId = courseId,
            Status = EnrollmentStatus.Active,
            ActivatedAtUtc = DateTime.UtcNow
        };
        await _enrollments.AddAsync(enrollment, ct);
        course.EnrolledCount += 1;
        await _courses.UpdateAsync(course, ct);

        var user = await _users.GetByIdAsync(userId, ct);
        return new EnrollmentDto(enrollment.Id, userId, user?.FullName ?? "", courseId, course.Title, enrollment.Status, enrollment.ActivatedAtUtc);
    }

    private void EnsureInstructorOrAdmin()
    {
        if (!_current.IsInRole(UserRole.Mentor, UserRole.Admin, UserRole.SuperAdmin))
            throw new UnauthorizedAccessDomainException("Instructor or Admin role required.");
    }

    private async Task<CourseDto> MapAsync(Course c, CancellationToken ct)
    {
        string? mentorName = null;
        if (c.MentorId.HasValue)
        {
            var mentor = await _users.GetByIdAsync(c.MentorId.Value, ct);
            mentorName = mentor?.FullName;
        }

        var mats = await _materials.GetByCourseAsync(c.Id, ct);
        var modulesCount = mats.Select(m => m.ModuleTitle).Distinct().Count();
        if (modulesCount == 0) modulesCount = 6;
        var lessonsCount = mats.Count;
        if (lessonsCount == 0) lessonsCount = 24;

        return new CourseDto(
            c.Id, c.Title, c.Description, c.Level, c.Status, c.Capacity, c.EnrolledCount,
            c.MentorId, mentorName, c.Language, c.Price, c.Currency, c.ThumbnailUrl,
            c.DurationWeeks, c.CreatedAtUtc, c.PaymentUrl,
            Slug: c.Slug,
            ShortDescription: c.ShortDescription,
            Objectives: c.Objectives,
            DisplayOrder: c.DisplayOrder,
            ModulesCount: modulesCount,
            LessonsCount: lessonsCount);
    }

    private static string GenerateSlug(string title)
    {
        var clean = new string(title.ToLowerInvariant().Select(ch => char.IsLetterOrDigit(ch) ? ch : '-').ToArray());
        return string.Join("-", clean.Split('-', StringSplitOptions.RemoveEmptyEntries));
    }
}
