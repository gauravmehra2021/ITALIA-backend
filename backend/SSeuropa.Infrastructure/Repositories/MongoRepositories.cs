using System.Text.RegularExpressions;
using MongoDB.Bson;
using MongoDB.Driver;
using SSeuropa.Application.Interfaces;
using SSeuropa.Domain.Common;
using SSeuropa.Domain.Entities;
using SSeuropa.Infrastructure.Persistence;

namespace SSeuropa.Infrastructure.Repositories;

internal static class MongoRepoHelpers
{
    public static async Task ReplaceAsync<T>(IMongoCollection<T> collection, T entity, CancellationToken ct)
        where T : EntityBase
    {
        entity.UpdatedAtUtc = DateTime.UtcNow;
        await collection.ReplaceOneAsync(x => x.Id == entity.Id, entity, cancellationToken: ct);
    }
}

public class MongoUserRepository(MongoDbContext db) : IUserRepository
{
    public async Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await db.Users.Find(u => u.Id == id).FirstOrDefaultAsync(ct);

    public async Task<User?> GetByEmailAsync(string email, CancellationToken ct = default)
    {
        var normalized = email.Trim().ToLowerInvariant();
        var exact = await db.Users.Find(u => u.Email == normalized).FirstOrDefaultAsync(ct);
        if (exact is not null) return exact;

        var regex = new BsonRegularExpression($"^{Regex.Escape(email.Trim())}$", "i");
        return await db.Users.Find(Builders<User>.Filter.Regex(u => u.Email, regex)).FirstOrDefaultAsync(ct);
    }

    public async Task<IReadOnlyList<User>> GetAllAsync(CancellationToken ct = default) =>
        await db.Users.Find(FilterDefinition<User>.Empty).ToListAsync(ct);

    public async Task<User> AddAsync(User user, CancellationToken ct = default)
    {
        user.Email = user.Email.Trim().ToLowerInvariant();
        await db.Users.InsertOneAsync(user, cancellationToken: ct);
        return user;
    }

    public Task UpdateAsync(User user, CancellationToken ct = default)
    {
        user.Email = user.Email.Trim().ToLowerInvariant();
        return MongoRepoHelpers.ReplaceAsync(db.Users, user, ct);
    }
}

public class MongoSessionRepository(MongoDbContext db) : ISessionRepository
{
    public async Task<UserSession?> GetByRefreshTokenAsync(string refreshToken, CancellationToken ct = default) =>
        await db.Sessions.Find(s => s.RefreshToken == refreshToken && !s.IsRevoked).FirstOrDefaultAsync(ct);

    public async Task<UserSession?> GetActiveByUserAsync(Guid userId, CancellationToken ct = default)
    {
        var now = DateTime.UtcNow;
        return await db.Sessions
            .Find(s => s.UserId == userId && !s.IsRevoked && s.ExpiresAtUtc > now)
            .FirstOrDefaultAsync(ct);
    }

    public async Task<IReadOnlyList<UserSession>> GetByUserAsync(Guid userId, CancellationToken ct = default) =>
        await db.Sessions.Find(s => s.UserId == userId).ToListAsync(ct);

    public async Task<UserSession> AddAsync(UserSession session, CancellationToken ct = default)
    {
        await db.Sessions.InsertOneAsync(session, cancellationToken: ct);
        return session;
    }

    public Task UpdateAsync(UserSession session, CancellationToken ct = default) =>
        MongoRepoHelpers.ReplaceAsync(db.Sessions, session, ct);
}

public class MongoOtpRepository(MongoDbContext db) : IOtpRepository
{
    public async Task<OtpChallenge> AddAsync(OtpChallenge challenge, CancellationToken ct = default)
    {
        await db.Otps.InsertOneAsync(challenge, cancellationToken: ct);
        return challenge;
    }

    public async Task<OtpChallenge?> GetActiveAsync(Guid userId, string purpose, CancellationToken ct = default)
    {
        var now = DateTime.UtcNow;
        return await db.Otps
            .Find(o => o.UserId == userId && o.Purpose == purpose && !o.IsUsed && o.ExpiresAtUtc > now)
            .SortByDescending(o => o.CreatedAtUtc)
            .FirstOrDefaultAsync(ct);
    }

    public async Task<OtpChallenge?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await db.Otps.Find(o => o.Id == id).FirstOrDefaultAsync(ct);

    public Task UpdateAsync(OtpChallenge challenge, CancellationToken ct = default) =>
        MongoRepoHelpers.ReplaceAsync(db.Otps, challenge, ct);
}

public class MongoCourseRepository(MongoDbContext db) : ICourseRepository
{
    public async Task<Course?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await db.Courses.Find(c => c.Id == id).FirstOrDefaultAsync(ct);

    public async Task<IReadOnlyList<Course>> GetAllAsync(CancellationToken ct = default) =>
        await db.Courses.Find(FilterDefinition<Course>.Empty).ToListAsync(ct);

    public async Task<Course> AddAsync(Course course, CancellationToken ct = default)
    {
        await db.Courses.InsertOneAsync(course, cancellationToken: ct);
        return course;
    }

    public Task UpdateAsync(Course course, CancellationToken ct = default) =>
        MongoRepoHelpers.ReplaceAsync(db.Courses, course, ct);

    public Task DeleteAsync(Guid id, CancellationToken ct = default) =>
        db.Courses.DeleteOneAsync(c => c.Id == id, ct);
}

public class MongoClassRepository(MongoDbContext db) : IClassRepository
{
    public async Task<ClassSession?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await db.Classes.Find(c => c.Id == id).FirstOrDefaultAsync(ct);

    public async Task<IReadOnlyList<ClassSession>> GetAllAsync(CancellationToken ct = default) =>
        await db.Classes.Find(FilterDefinition<ClassSession>.Empty).ToListAsync(ct);

    public async Task<IReadOnlyList<ClassSession>> GetByCourseAsync(Guid courseId, CancellationToken ct = default) =>
        await db.Classes.Find(c => c.CourseId == courseId).ToListAsync(ct);

    public async Task<IReadOnlyList<ClassSession>> GetByMentorAsync(Guid mentorId, CancellationToken ct = default) =>
        await db.Classes.Find(c => c.MentorId == mentorId).ToListAsync(ct);

    public async Task<ClassSession> AddAsync(ClassSession session, CancellationToken ct = default)
    {
        await db.Classes.InsertOneAsync(session, cancellationToken: ct);
        return session;
    }

    public Task UpdateAsync(ClassSession session, CancellationToken ct = default) =>
        MongoRepoHelpers.ReplaceAsync(db.Classes, session, ct);
}

public class MongoEnrollmentRepository(MongoDbContext db) : IEnrollmentRepository
{
    public async Task<Enrollment?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await db.Enrollments.Find(e => e.Id == id).FirstOrDefaultAsync(ct);

    public async Task<Enrollment?> GetByUserAndCourseAsync(Guid userId, Guid courseId, CancellationToken ct = default) =>
        await db.Enrollments.Find(e => e.UserId == userId && e.CourseId == courseId).FirstOrDefaultAsync(ct);

    public async Task<IReadOnlyList<Enrollment>> GetByUserAsync(Guid userId, CancellationToken ct = default) =>
        await db.Enrollments.Find(e => e.UserId == userId).ToListAsync(ct);

    public async Task<IReadOnlyList<Enrollment>> GetAllAsync(CancellationToken ct = default) =>
        await db.Enrollments.Find(FilterDefinition<Enrollment>.Empty).ToListAsync(ct);

    public async Task<Enrollment> AddAsync(Enrollment enrollment, CancellationToken ct = default)
    {
        await db.Enrollments.InsertOneAsync(enrollment, cancellationToken: ct);
        return enrollment;
    }

    public Task UpdateAsync(Enrollment enrollment, CancellationToken ct = default) =>
        MongoRepoHelpers.ReplaceAsync(db.Enrollments, enrollment, ct);
}

public class MongoMaterialRepository(MongoDbContext db) : IMaterialRepository
{
    public async Task<Material?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await db.Materials.Find(m => m.Id == id).FirstOrDefaultAsync(ct);

    public async Task<IReadOnlyList<Material>> GetByCourseAsync(Guid courseId, CancellationToken ct = default) =>
        await db.Materials.Find(m => m.CourseId == courseId).ToListAsync(ct);

    public async Task<Material> AddAsync(Material material, CancellationToken ct = default)
    {
        await db.Materials.InsertOneAsync(material, cancellationToken: ct);
        return material;
    }

    public Task DeleteAsync(Guid id, CancellationToken ct = default) =>
        db.Materials.DeleteOneAsync(m => m.Id == id, ct);
}

public class MongoAssessmentRepository(MongoDbContext db) : IAssessmentRepository
{
    public async Task<Assessment?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await db.Assessments.Find(a => a.Id == id).FirstOrDefaultAsync(ct);

    public async Task<IReadOnlyList<Assessment>> GetByCourseAsync(Guid courseId, CancellationToken ct = default) =>
        await db.Assessments.Find(a => a.CourseId == courseId).ToListAsync(ct);

    public async Task<IReadOnlyList<Assessment>> GetAllAsync(CancellationToken ct = default) =>
        await db.Assessments.Find(FilterDefinition<Assessment>.Empty).ToListAsync(ct);

    public async Task<Assessment> AddAsync(Assessment assessment, CancellationToken ct = default)
    {
        await db.Assessments.InsertOneAsync(assessment, cancellationToken: ct);
        return assessment;
    }

    public async Task<TestAttempt> AddAttemptAsync(TestAttempt attempt, CancellationToken ct = default)
    {
        await db.Attempts.InsertOneAsync(attempt, cancellationToken: ct);
        return attempt;
    }

    public async Task<IReadOnlyList<TestAttempt>> GetAttemptsByUserAsync(Guid userId, CancellationToken ct = default) =>
        await db.Attempts.Find(a => a.UserId == userId).ToListAsync(ct);
}

public class MongoAttendanceRepository(MongoDbContext db) : IAttendanceRepository
{
    public async Task<AttendanceRecord?> GetAsync(Guid classSessionId, Guid userId, CancellationToken ct = default) =>
        await db.Attendance.Find(a => a.ClassSessionId == classSessionId && a.UserId == userId).FirstOrDefaultAsync(ct);

    public async Task<IReadOnlyList<AttendanceRecord>> GetByClassAsync(Guid classSessionId, CancellationToken ct = default) =>
        await db.Attendance.Find(a => a.ClassSessionId == classSessionId).ToListAsync(ct);

    public async Task<AttendanceRecord> UpsertAsync(AttendanceRecord record, CancellationToken ct = default)
    {
        var existing = await GetAsync(record.ClassSessionId, record.UserId, ct);
        if (existing is null)
        {
            await db.Attendance.InsertOneAsync(record, cancellationToken: ct);
            return record;
        }

        existing.Present = record.Present;
        existing.MarkedAtUtc = record.MarkedAtUtc;
        await MongoRepoHelpers.ReplaceAsync(db.Attendance, existing, ct);
        return existing;
    }
}

public class MongoProgressRepository(MongoDbContext db) : IProgressRepository
{
    public async Task<LearningProgress?> GetAsync(Guid userId, Guid courseId, CancellationToken ct = default) =>
        await db.Progress.Find(p => p.UserId == userId && p.CourseId == courseId).FirstOrDefaultAsync(ct);

    public async Task<IReadOnlyList<LearningProgress>> GetByUserAsync(Guid userId, CancellationToken ct = default) =>
        await db.Progress.Find(p => p.UserId == userId).ToListAsync(ct);

    public async Task<IReadOnlyList<LearningProgress>> GetAllAsync(CancellationToken ct = default) =>
        await db.Progress.Find(FilterDefinition<LearningProgress>.Empty).ToListAsync(ct);

    public async Task UpsertAsync(LearningProgress progress, CancellationToken ct = default)
    {
        var existing = await GetAsync(progress.UserId, progress.CourseId, ct);
        if (existing is null)
        {
            await db.Progress.InsertOneAsync(progress, cancellationToken: ct);
            return;
        }

        existing.CompletionPercent = progress.CompletionPercent;
        existing.StreakDays = progress.StreakDays;
        existing.GrammarScore = progress.GrammarScore;
        existing.VocabularyScore = progress.VocabularyScore;
        existing.SpeakingScore = progress.SpeakingScore;
        existing.LastActivityAtUtc = progress.LastActivityAtUtc;
        await MongoRepoHelpers.ReplaceAsync(db.Progress, existing, ct);
    }
}

public class MongoSubscriptionRepository(MongoDbContext db) : ISubscriptionRepository
{
    public async Task<Subscription?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await db.Subscriptions.Find(s => s.Id == id).FirstOrDefaultAsync(ct);

    public async Task<IReadOnlyList<Subscription>> GetByUserAsync(Guid userId, CancellationToken ct = default) =>
        await db.Subscriptions.Find(s => s.UserId == userId).ToListAsync(ct);

    public async Task<IReadOnlyList<Subscription>> GetAllAsync(CancellationToken ct = default) =>
        await db.Subscriptions.Find(FilterDefinition<Subscription>.Empty).ToListAsync(ct);

    public async Task<Subscription> AddAsync(Subscription subscription, CancellationToken ct = default)
    {
        await db.Subscriptions.InsertOneAsync(subscription, cancellationToken: ct);
        return subscription;
    }

    public Task UpdateAsync(Subscription subscription, CancellationToken ct = default) =>
        MongoRepoHelpers.ReplaceAsync(db.Subscriptions, subscription, ct);
}

public class MongoPaymentRepository(MongoDbContext db) : IPaymentRepository
{
    public async Task<IReadOnlyList<Payment>> GetByUserAsync(Guid userId, CancellationToken ct = default) =>
        await db.Payments.Find(p => p.UserId == userId).ToListAsync(ct);

    public async Task<IReadOnlyList<Payment>> GetAllAsync(CancellationToken ct = default) =>
        await db.Payments.Find(FilterDefinition<Payment>.Empty).ToListAsync(ct);

    public async Task<Payment> AddAsync(Payment payment, CancellationToken ct = default)
    {
        await db.Payments.InsertOneAsync(payment, cancellationToken: ct);
        return payment;
    }

    public Task UpdateAsync(Payment payment, CancellationToken ct = default) =>
        MongoRepoHelpers.ReplaceAsync(db.Payments, payment, ct);
}

public class MongoCertificateRepository(MongoDbContext db) : ICertificateRepository
{
    public async Task<Certificate?> GetByCodeAsync(string code, CancellationToken ct = default)
    {
        var regex = new BsonRegularExpression($"^{Regex.Escape(code.Trim())}$", "i");
        return await db.Certificates.Find(Builders<Certificate>.Filter.Regex(c => c.CertificateCode, regex)).FirstOrDefaultAsync(ct);
    }

    public async Task<IReadOnlyList<Certificate>> GetByUserAsync(Guid userId, CancellationToken ct = default) =>
        await db.Certificates.Find(c => c.UserId == userId).ToListAsync(ct);

    public async Task<IReadOnlyList<Certificate>> GetAllAsync(CancellationToken ct = default) =>
        await db.Certificates.Find(FilterDefinition<Certificate>.Empty).ToListAsync(ct);

    public async Task<Certificate> AddAsync(Certificate certificate, CancellationToken ct = default)
    {
        await db.Certificates.InsertOneAsync(certificate, cancellationToken: ct);
        return certificate;
    }
}

public class MongoNotificationRepository(MongoDbContext db) : INotificationRepository
{
    public async Task<IReadOnlyList<Notification>> GetByUserAsync(Guid userId, CancellationToken ct = default) =>
        await db.Notifications
            .Find(n => n.UserId == userId || n.UserId == null)
            .SortByDescending(n => n.CreatedAtUtc)
            .ToListAsync(ct);

    public async Task<Notification> AddAsync(Notification notification, CancellationToken ct = default)
    {
        await db.Notifications.InsertOneAsync(notification, cancellationToken: ct);
        return notification;
    }

    public Task MarkReadAsync(Guid id, CancellationToken ct = default)
    {
        var update = Builders<Notification>.Update
            .Set(n => n.IsRead, true)
            .Set(n => n.UpdatedAtUtc, DateTime.UtcNow);
        return db.Notifications.UpdateOneAsync(n => n.Id == id, update, cancellationToken: ct);
    }
}

public class MongoAnnouncementRepository(MongoDbContext db) : IAnnouncementRepository
{
    public async Task<IReadOnlyList<Announcement>> GetPublishedAsync(CancellationToken ct = default) =>
        await db.Announcements
            .Find(a => a.IsPublished)
            .SortByDescending(a => a.CreatedAtUtc)
            .ToListAsync(ct);

    public async Task<Announcement> AddAsync(Announcement announcement, CancellationToken ct = default)
    {
        await db.Announcements.InsertOneAsync(announcement, cancellationToken: ct);
        return announcement;
    }
}

public class MongoSupportTicketRepository(MongoDbContext db) : ISupportTicketRepository
{
    public async Task<SupportTicket?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await db.Tickets.Find(t => t.Id == id).FirstOrDefaultAsync(ct);

    public async Task<IReadOnlyList<SupportTicket>> GetByUserAsync(Guid userId, CancellationToken ct = default) =>
        await db.Tickets.Find(t => t.UserId == userId).ToListAsync(ct);

    public async Task<IReadOnlyList<SupportTicket>> GetAllAsync(CancellationToken ct = default) =>
        await db.Tickets.Find(FilterDefinition<SupportTicket>.Empty).ToListAsync(ct);

    public async Task<SupportTicket> AddAsync(SupportTicket ticket, CancellationToken ct = default)
    {
        await db.Tickets.InsertOneAsync(ticket, cancellationToken: ct);
        return ticket;
    }

    public Task UpdateAsync(SupportTicket ticket, CancellationToken ct = default) =>
        MongoRepoHelpers.ReplaceAsync(db.Tickets, ticket, ct);
}

public class MongoAuditLogRepository(MongoDbContext db) : IAuditLogRepository
{
    public Task AddAsync(AuditLog log, CancellationToken ct = default) =>
        db.AuditLogs.InsertOneAsync(log, cancellationToken: ct);

    public async Task<IReadOnlyList<AuditLog>> GetRecentAsync(int take = 50, CancellationToken ct = default) =>
        await db.AuditLogs
            .Find(FilterDefinition<AuditLog>.Empty)
            .SortByDescending(a => a.CreatedAtUtc)
            .Limit(take)
            .ToListAsync(ct);
}

public class MongoAiSessionRepository(MongoDbContext db) : IAiSessionRepository
{
    public async Task<AiSession> AddAsync(AiSession session, CancellationToken ct = default)
    {
        await db.AiSessions.InsertOneAsync(session, cancellationToken: ct);
        return session;
    }

    public async Task<IReadOnlyList<AiSession>> GetByUserAsync(Guid userId, CancellationToken ct = default) =>
        await db.AiSessions
            .Find(s => s.UserId == userId)
            .SortByDescending(s => s.CreatedAtUtc)
            .ToListAsync(ct);
}

public class MongoEnquiryRepository(MongoDbContext db) : IEnquiryRepository
{
    public async Task<Enquiry?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await db.Enquiries.Find(e => e.Id == id).FirstOrDefaultAsync(ct);

    public async Task<IReadOnlyList<Enquiry>> GetAllAsync(CancellationToken ct = default) =>
        await db.Enquiries
            .Find(FilterDefinition<Enquiry>.Empty)
            .SortByDescending(e => e.CreatedAtUtc)
            .ToListAsync(ct);

    public async Task<Enquiry> AddAsync(Enquiry enquiry, CancellationToken ct = default)
    {
        await db.Enquiries.InsertOneAsync(enquiry, cancellationToken: ct);
        return enquiry;
    }

    public Task UpdateAsync(Enquiry enquiry, CancellationToken ct = default) =>
        MongoRepoHelpers.ReplaceAsync(db.Enquiries, enquiry, ct);

    public Task DeleteAsync(Guid id, CancellationToken ct = default) =>
        db.Enquiries.DeleteOneAsync(e => e.Id == id, ct);
}

