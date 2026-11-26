using SSeuropa.Domain.Entities;
using SSeuropa.Domain.Enums;

namespace SSeuropa.Application.Services;

/// <summary>
/// Who a live class is for. One rule, used by the student portal, the mobile app, class alerts and dashboards:
///   • A batch class is for the students in that batch (a student may be in several batches).
///   • A class scheduled before classes were per batch (no batch) is for the students actively
///     enrolled in its course, as before.
/// </summary>
public static class ClassAudience
{
    public static bool IsFor(ClassSession session, User student, IReadOnlySet<Guid> activeCourseIds)
    {
        if (student.IsDeleted) return false;
        return session.BatchId is { } batchId
            ? student.IsInBatch(batchId)
            : session.CourseId != Guid.Empty && activeCourseIds.Contains(session.CourseId);
    }

    /// <summary>The students who should see the class and receive its alerts.</summary>
    public static List<User> Students(ClassSession session, IEnumerable<User> allUsers, IEnumerable<Enrollment> allEnrollments)
    {
        var students = allUsers.Where(u => u.Role == UserRole.User && !u.IsDeleted);
        if (session.BatchId is { } batchId)
            return students.Where(u => u.IsInBatch(batchId)).ToList();

        var enrolled = allEnrollments
            .Where(e => e.CourseId == session.CourseId && e.Status == EnrollmentStatus.Active)
            .Select(e => e.UserId)
            .ToHashSet();
        return students.Where(u => enrolled.Contains(u.Id)).ToList();
    }

    /// <summary>The classes a student should see.</summary>
    public static IEnumerable<ClassSession> ForStudent(IEnumerable<ClassSession> classes, User student, IEnumerable<Enrollment> studentEnrollments)
    {
        var active = studentEnrollments.Where(e => e.Status == EnrollmentStatus.Active).Select(e => e.CourseId).ToHashSet();
        return classes.Where(c => IsFor(c, student, active));
    }
}
