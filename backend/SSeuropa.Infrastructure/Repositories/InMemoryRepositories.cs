using SSeuropa.Application.Interfaces;
using SSeuropa.Domain.Entities;
using SSeuropa.Infrastructure.Persistence;

namespace SSeuropa.Infrastructure.Repositories;

public class InMemoryUserRepository(InMemoryStore store) : IUserRepository
{
    public Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        Task.FromResult(store.Users.FirstOrDefault(u => u.Id == id));

    public Task<User?> GetByEmailAsync(string email, CancellationToken ct = default) =>
        Task.FromResult(store.Users.FirstOrDefault(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase)));

    public Task<IReadOnlyList<User>> GetAllAsync(CancellationToken ct = default) =>
        Task.FromResult<IReadOnlyList<User>>(store.Users.ToList());

    public Task<User> AddAsync(User user, CancellationToken ct = default)
    {
        store.Users.Add(user);
        return Task.FromResult(user);
    }

    public Task UpdateAsync(User user, CancellationToken ct = default)
    {
        user.UpdatedAtUtc = DateTime.UtcNow;
        return Task.CompletedTask;
    }
}

public class InMemorySessionRepository(InMemoryStore store) : ISessionRepository
{
    public Task<UserSession?> GetByRefreshTokenAsync(string refreshToken, CancellationToken ct = default) =>
        Task.FromResult(store.Sessions.FirstOrDefault(s => s.RefreshToken == refreshToken && !s.IsRevoked));

    public Task<UserSession?> GetActiveByUserAsync(Guid userId, CancellationToken ct = default) =>
        Task.FromResult(store.Sessions.FirstOrDefault(s => s.UserId == userId && !s.IsRevoked && s.ExpiresAtUtc > DateTime.UtcNow));

    public Task<IReadOnlyList<UserSession>> GetByUserAsync(Guid userId, CancellationToken ct = default) =>
        Task.FromResult<IReadOnlyList<UserSession>>(store.Sessions.Where(s => s.UserId == userId).ToList());

    public Task<UserSession> AddAsync(UserSession session, CancellationToken ct = default)
    {
        store.Sessions.Add(session);
        return Task.FromResult(session);
    }

    public Task UpdateAsync(UserSession session, CancellationToken ct = default) => Task.CompletedTask;
}

public class InMemoryOtpRepository(InMemoryStore store) : IOtpRepository
{
    public Task<OtpChallenge> AddAsync(OtpChallenge challenge, CancellationToken ct = default)
    {
        store.Otps.Add(challenge);
        return Task.FromResult(challenge);
    }

    public Task<OtpChallenge?> GetActiveAsync(Guid userId, string purpose, CancellationToken ct = default) =>
        Task.FromResult(store.Otps
            .Where(o => o.UserId == userId && o.Purpose == purpose && !o.IsUsed && o.ExpiresAtUtc > DateTime.UtcNow)
            .OrderByDescending(o => o.CreatedAtUtc)
            .FirstOrDefault());

    public Task<OtpChallenge?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        Task.FromResult(store.Otps.FirstOrDefault(o => o.Id == id));

    public Task UpdateAsync(OtpChallenge challenge, CancellationToken ct = default) => Task.CompletedTask;
}

public class InMemoryCourseRepository(InMemoryStore store) : ICourseRepository
{
    public Task<Course?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        Task.FromResult(store.Courses.FirstOrDefault(c => c.Id == id));

    public Task<IReadOnlyList<Course>> GetAllAsync(CancellationToken ct = default) =>
        Task.FromResult<IReadOnlyList<Course>>(store.Courses.ToList());

    public Task<Course> AddAsync(Course course, CancellationToken ct = default)
    {
        store.Courses.Add(course);
        return Task.FromResult(course);
    }

    public Task UpdateAsync(Course course, CancellationToken ct = default) => Task.CompletedTask;

    public Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        store.Courses.RemoveAll(c => c.Id == id);
        return Task.CompletedTask;
    }
}

public class InMemoryClassRepository(InMemoryStore store) : IClassRepository
{
    public Task<ClassSession?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        Task.FromResult(store.Classes.FirstOrDefault(c => c.Id == id));

    public Task<IReadOnlyList<ClassSession>> GetAllAsync(CancellationToken ct = default) =>
        Task.FromResult<IReadOnlyList<ClassSession>>(store.Classes.ToList());

    public Task<IReadOnlyList<ClassSession>> GetByCourseAsync(Guid courseId, CancellationToken ct = default) =>
        Task.FromResult<IReadOnlyList<ClassSession>>(store.Classes.Where(c => c.CourseId == courseId).ToList());

    public Task<IReadOnlyList<ClassSession>> GetByMentorAsync(Guid mentorId, CancellationToken ct = default) =>
        Task.FromResult<IReadOnlyList<ClassSession>>(store.Classes.Where(c => c.MentorId == mentorId).ToList());

    public Task<ClassSession> AddAsync(ClassSession session, CancellationToken ct = default)
    {
        store.Classes.Add(session);
        return Task.FromResult(session);
    }

    public Task UpdateAsync(ClassSession session, CancellationToken ct = default) => Task.CompletedTask;
}

public class InMemoryEnrollmentRepository(InMemoryStore store) : IEnrollmentRepository
{
    public Task<Enrollment?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        Task.FromResult(store.Enrollments.FirstOrDefault(e => e.Id == id));

    public Task<Enrollment?> GetByUserAndCourseAsync(Guid userId, Guid courseId, CancellationToken ct = default) =>
        Task.FromResult(store.Enrollments.FirstOrDefault(e => e.UserId == userId && e.CourseId == courseId));

    public Task<IReadOnlyList<Enrollment>> GetByUserAsync(Guid userId, CancellationToken ct = default) =>
        Task.FromResult<IReadOnlyList<Enrollment>>(store.Enrollments.Where(e => e.UserId == userId).ToList());

    public Task<IReadOnlyList<Enrollment>> GetAllAsync(CancellationToken ct = default) =>
        Task.FromResult<IReadOnlyList<Enrollment>>(store.Enrollments.ToList());

    public Task<Enrollment> AddAsync(Enrollment enrollment, CancellationToken ct = default)
    {
        store.Enrollments.Add(enrollment);
        return Task.FromResult(enrollment);
    }

    public Task UpdateAsync(Enrollment enrollment, CancellationToken ct = default) => Task.CompletedTask;
}

public class InMemoryMaterialRepository(InMemoryStore store) : IMaterialRepository
{
    public Task<Material?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        Task.FromResult(store.Materials.FirstOrDefault(m => m.Id == id));

    public Task<IReadOnlyList<Material>> GetByCourseAsync(Guid courseId, CancellationToken ct = default) =>
        Task.FromResult<IReadOnlyList<Material>>(store.Materials.Where(m => m.CourseId == courseId).ToList());

    public Task<Material> AddAsync(Material material, CancellationToken ct = default)
    {
        store.Materials.Add(material);
        return Task.FromResult(material);
    }

    public Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        store.Materials.RemoveAll(m => m.Id == id);
        return Task.CompletedTask;
    }
}

public class InMemoryAssessmentRepository(InMemoryStore store) : IAssessmentRepository
{
    public Task<Assessment?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        Task.FromResult(store.Assessments.FirstOrDefault(a => a.Id == id));

    public Task<IReadOnlyList<Assessment>> GetByCourseAsync(Guid courseId, CancellationToken ct = default) =>
        Task.FromResult<IReadOnlyList<Assessment>>(store.Assessments.Where(a => a.CourseId == courseId).ToList());

    public Task<IReadOnlyList<Assessment>> GetAllAsync(CancellationToken ct = default) =>
        Task.FromResult<IReadOnlyList<Assessment>>(store.Assessments.ToList());

    public Task<Assessment> AddAsync(Assessment assessment, CancellationToken ct = default)
    {
        store.Assessments.Add(assessment);
        return Task.FromResult(assessment);
    }

    public Task<TestAttempt> AddAttemptAsync(TestAttempt attempt, CancellationToken ct = default)
    {
        store.Attempts.Add(attempt);
        return Task.FromResult(attempt);
    }

    public Task<IReadOnlyList<TestAttempt>> GetAttemptsByUserAsync(Guid userId, CancellationToken ct = default) =>
        Task.FromResult<IReadOnlyList<TestAttempt>>(store.Attempts.Where(a => a.UserId == userId).ToList());
}

public class InMemoryAttendanceRepository(InMemoryStore store) : IAttendanceRepository
{
    public Task<AttendanceRecord?> GetAsync(Guid classSessionId, Guid userId, CancellationToken ct = default) =>
        Task.FromResult(store.Attendance.FirstOrDefault(a => a.ClassSessionId == classSessionId && a.UserId == userId));

    public Task<IReadOnlyList<AttendanceRecord>> GetByClassAsync(Guid classSessionId, CancellationToken ct = default) =>
        Task.FromResult<IReadOnlyList<AttendanceRecord>>(store.Attendance.Where(a => a.ClassSessionId == classSessionId).ToList());

    public Task<AttendanceRecord> UpsertAsync(AttendanceRecord record, CancellationToken ct = default)
    {
        var existing = store.Attendance.FirstOrDefault(a => a.ClassSessionId == record.ClassSessionId && a.UserId == record.UserId);
        if (existing is null)
        {
            store.Attendance.Add(record);
            return Task.FromResult(record);
        }

        existing.Present = record.Present;
        existing.MarkedAtUtc = record.MarkedAtUtc;
        existing.UpdatedAtUtc = DateTime.UtcNow;
        return Task.FromResult(existing);
    }
}

public class InMemoryProgressRepository(InMemoryStore store) : IProgressRepository
{
    public Task<LearningProgress?> GetAsync(Guid userId, Guid courseId, CancellationToken ct = default) =>
        Task.FromResult(store.Progress.FirstOrDefault(p => p.UserId == userId && p.CourseId == courseId));

    public Task<IReadOnlyList<LearningProgress>> GetByUserAsync(Guid userId, CancellationToken ct = default) =>
        Task.FromResult<IReadOnlyList<LearningProgress>>(store.Progress.Where(p => p.UserId == userId).ToList());

    public Task<IReadOnlyList<LearningProgress>> GetAllAsync(CancellationToken ct = default) =>
        Task.FromResult<IReadOnlyList<LearningProgress>>(store.Progress.ToList());

    public Task UpsertAsync(LearningProgress progress, CancellationToken ct = default)
    {
        var existing = store.Progress.FirstOrDefault(p => p.UserId == progress.UserId && p.CourseId == progress.CourseId);
        if (existing is null) store.Progress.Add(progress);
        else
        {
            existing.CompletionPercent = progress.CompletionPercent;
            existing.StreakDays = progress.StreakDays;
            existing.GrammarScore = progress.GrammarScore;
            existing.VocabularyScore = progress.VocabularyScore;
            existing.SpeakingScore = progress.SpeakingScore;
            existing.LastActivityAtUtc = progress.LastActivityAtUtc;
            existing.UpdatedAtUtc = DateTime.UtcNow;
        }
        return Task.CompletedTask;
    }
}

public class InMemorySubscriptionRepository(InMemoryStore store) : ISubscriptionRepository
{
    public Task<Subscription?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        Task.FromResult(store.Subscriptions.FirstOrDefault(s => s.Id == id));

    public Task<IReadOnlyList<Subscription>> GetByUserAsync(Guid userId, CancellationToken ct = default) =>
        Task.FromResult<IReadOnlyList<Subscription>>(store.Subscriptions.Where(s => s.UserId == userId).ToList());

    public Task<IReadOnlyList<Subscription>> GetAllAsync(CancellationToken ct = default) =>
        Task.FromResult<IReadOnlyList<Subscription>>(store.Subscriptions.ToList());

    public Task<Subscription> AddAsync(Subscription subscription, CancellationToken ct = default)
    {
        store.Subscriptions.Add(subscription);
        return Task.FromResult(subscription);
    }

    public Task UpdateAsync(Subscription subscription, CancellationToken ct = default) => Task.CompletedTask;
}

public class InMemoryPaymentRepository(InMemoryStore store) : IPaymentRepository
{
    public Task<IReadOnlyList<Payment>> GetByUserAsync(Guid userId, CancellationToken ct = default) =>
        Task.FromResult<IReadOnlyList<Payment>>(store.Payments.Where(p => p.UserId == userId).ToList());

    public Task<IReadOnlyList<Payment>> GetAllAsync(CancellationToken ct = default) =>
        Task.FromResult<IReadOnlyList<Payment>>(store.Payments.ToList());

    public Task<Payment> AddAsync(Payment payment, CancellationToken ct = default)
    {
        store.Payments.Add(payment);
        return Task.FromResult(payment);
    }

    public Task UpdateAsync(Payment payment, CancellationToken ct = default) => Task.CompletedTask;
}

public class InMemoryCertificateRepository(InMemoryStore store) : ICertificateRepository
{
    public Task<Certificate?> GetByCodeAsync(string code, CancellationToken ct = default) =>
        Task.FromResult(store.Certificates.FirstOrDefault(c => c.CertificateCode.Equals(code, StringComparison.OrdinalIgnoreCase)));

    public Task<IReadOnlyList<Certificate>> GetByUserAsync(Guid userId, CancellationToken ct = default) =>
        Task.FromResult<IReadOnlyList<Certificate>>(store.Certificates.Where(c => c.UserId == userId).ToList());

    public Task<IReadOnlyList<Certificate>> GetAllAsync(CancellationToken ct = default) =>
        Task.FromResult<IReadOnlyList<Certificate>>(store.Certificates.ToList());

    public Task<Certificate> AddAsync(Certificate certificate, CancellationToken ct = default)
    {
        store.Certificates.Add(certificate);
        return Task.FromResult(certificate);
    }
}

public class InMemoryNotificationRepository(InMemoryStore store) : INotificationRepository
{
    public Task<IReadOnlyList<Notification>> GetByUserAsync(Guid userId, CancellationToken ct = default) =>
        Task.FromResult<IReadOnlyList<Notification>>(store.Notifications.Where(n => n.UserId == userId || n.UserId is null).OrderByDescending(n => n.CreatedAtUtc).ToList());

    public Task<Notification> AddAsync(Notification notification, CancellationToken ct = default)
    {
        store.Notifications.Add(notification);
        return Task.FromResult(notification);
    }

    public Task MarkReadAsync(Guid id, CancellationToken ct = default)
    {
        var n = store.Notifications.FirstOrDefault(x => x.Id == id);
        if (n is not null) n.IsRead = true;
        return Task.CompletedTask;
    }
}

public class InMemoryAnnouncementRepository(InMemoryStore store) : IAnnouncementRepository
{
    public Task<IReadOnlyList<Announcement>> GetPublishedAsync(CancellationToken ct = default) =>
        Task.FromResult<IReadOnlyList<Announcement>>(store.Announcements.Where(a => a.IsPublished).OrderByDescending(a => a.CreatedAtUtc).ToList());

    public Task<Announcement> AddAsync(Announcement announcement, CancellationToken ct = default)
    {
        store.Announcements.Add(announcement);
        return Task.FromResult(announcement);
    }
}

public class InMemorySupportTicketRepository(InMemoryStore store) : ISupportTicketRepository
{
    public Task<SupportTicket?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        Task.FromResult(store.Tickets.FirstOrDefault(t => t.Id == id));

    public Task<IReadOnlyList<SupportTicket>> GetByUserAsync(Guid userId, CancellationToken ct = default) =>
        Task.FromResult<IReadOnlyList<SupportTicket>>(store.Tickets.Where(t => t.UserId == userId).ToList());

    public Task<IReadOnlyList<SupportTicket>> GetAllAsync(CancellationToken ct = default) =>
        Task.FromResult<IReadOnlyList<SupportTicket>>(store.Tickets.ToList());

    public Task<SupportTicket> AddAsync(SupportTicket ticket, CancellationToken ct = default)
    {
        store.Tickets.Add(ticket);
        return Task.FromResult(ticket);
    }

    public Task UpdateAsync(SupportTicket ticket, CancellationToken ct = default) => Task.CompletedTask;
}

public class InMemoryAuditLogRepository(InMemoryStore store) : IAuditLogRepository
{
    public Task AddAsync(AuditLog log, CancellationToken ct = default)
    {
        store.AuditLogs.Add(log);
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<AuditLog>> GetRecentAsync(int take = 50, CancellationToken ct = default) =>
        Task.FromResult<IReadOnlyList<AuditLog>>(store.AuditLogs.OrderByDescending(a => a.CreatedAtUtc).Take(take).ToList());
}

public class InMemoryAiSessionRepository(InMemoryStore store) : IAiSessionRepository
{
    public Task<AiSession> AddAsync(AiSession session, CancellationToken ct = default)
    {
        store.AiSessions.Add(session);
        return Task.FromResult(session);
    }

    public Task<IReadOnlyList<AiSession>> GetByUserAsync(Guid userId, CancellationToken ct = default) =>
        Task.FromResult<IReadOnlyList<AiSession>>(store.AiSessions.Where(s => s.UserId == userId).OrderByDescending(s => s.CreatedAtUtc).ToList());
}

public class InMemoryEnquiryRepository(InMemoryStore store) : IEnquiryRepository
{
    public Task<Enquiry?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        Task.FromResult(store.Enquiries.FirstOrDefault(e => e.Id == id));

    public Task<IReadOnlyList<Enquiry>> GetAllAsync(CancellationToken ct = default) =>
        Task.FromResult<IReadOnlyList<Enquiry>>(store.Enquiries.ToList());

    public Task<Enquiry> AddAsync(Enquiry enquiry, CancellationToken ct = default)
    {
        store.Enquiries.Add(enquiry);
        return Task.FromResult(enquiry);
    }

    public Task UpdateAsync(Enquiry enquiry, CancellationToken ct = default)
    {
        enquiry.UpdatedAtUtc = DateTime.UtcNow;
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        store.Enquiries.RemoveAll(e => e.Id == id);
        return Task.CompletedTask;
    }
}
