using SSeuropa.Domain.Entities;

namespace SSeuropa.Application.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<User?> GetByEmailAsync(string email, CancellationToken ct = default);
    Task<IReadOnlyList<User>> GetAllAsync(CancellationToken ct = default);
    Task<User> AddAsync(User user, CancellationToken ct = default);
    Task UpdateAsync(User user, CancellationToken ct = default);
}

public interface ISessionRepository
{
    Task<UserSession?> GetByRefreshTokenAsync(string refreshToken, CancellationToken ct = default);
    Task<UserSession?> GetActiveByUserAsync(Guid userId, CancellationToken ct = default);
    Task<IReadOnlyList<UserSession>> GetByUserAsync(Guid userId, CancellationToken ct = default);
    Task<UserSession> AddAsync(UserSession session, CancellationToken ct = default);
    Task UpdateAsync(UserSession session, CancellationToken ct = default);
}

public interface IOtpRepository
{
    Task<OtpChallenge> AddAsync(OtpChallenge challenge, CancellationToken ct = default);
    Task<OtpChallenge?> GetActiveAsync(Guid userId, string purpose, CancellationToken ct = default);
    Task<OtpChallenge?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task UpdateAsync(OtpChallenge challenge, CancellationToken ct = default);
}

public interface ICourseRepository
{
    Task<Course?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<Course>> GetAllAsync(CancellationToken ct = default);
    Task<Course> AddAsync(Course course, CancellationToken ct = default);
    Task UpdateAsync(Course course, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}

public interface IClassRepository
{
    Task<ClassSession?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<ClassSession>> GetAllAsync(CancellationToken ct = default);
    Task<IReadOnlyList<ClassSession>> GetByCourseAsync(Guid courseId, CancellationToken ct = default);
    Task<IReadOnlyList<ClassSession>> GetByMentorAsync(Guid mentorId, CancellationToken ct = default);
    Task<ClassSession> AddAsync(ClassSession session, CancellationToken ct = default);
    Task UpdateAsync(ClassSession session, CancellationToken ct = default);
}

public interface IEnrollmentRepository
{
    Task<Enrollment?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Enrollment?> GetByUserAndCourseAsync(Guid userId, Guid courseId, CancellationToken ct = default);
    Task<IReadOnlyList<Enrollment>> GetByUserAsync(Guid userId, CancellationToken ct = default);
    Task<IReadOnlyList<Enrollment>> GetAllAsync(CancellationToken ct = default);
    Task<Enrollment> AddAsync(Enrollment enrollment, CancellationToken ct = default);
    Task UpdateAsync(Enrollment enrollment, CancellationToken ct = default);
}

public interface IMaterialRepository
{
    Task<Material?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<Material>> GetByCourseAsync(Guid courseId, CancellationToken ct = default);
    Task<Material> AddAsync(Material material, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}

public interface IAssessmentRepository
{
    Task<Assessment?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<Assessment>> GetByCourseAsync(Guid courseId, CancellationToken ct = default);
    Task<IReadOnlyList<Assessment>> GetAllAsync(CancellationToken ct = default);
    Task<Assessment> AddAsync(Assessment assessment, CancellationToken ct = default);
    Task<TestAttempt> AddAttemptAsync(TestAttempt attempt, CancellationToken ct = default);
    Task<IReadOnlyList<TestAttempt>> GetAttemptsByUserAsync(Guid userId, CancellationToken ct = default);
}

public interface IAttendanceRepository
{
    Task<AttendanceRecord?> GetAsync(Guid classSessionId, Guid userId, CancellationToken ct = default);
    Task<IReadOnlyList<AttendanceRecord>> GetByClassAsync(Guid classSessionId, CancellationToken ct = default);
    Task<AttendanceRecord> UpsertAsync(AttendanceRecord record, CancellationToken ct = default);
}

public interface IProgressRepository
{
    Task<LearningProgress?> GetAsync(Guid userId, Guid courseId, CancellationToken ct = default);
    Task<IReadOnlyList<LearningProgress>> GetByUserAsync(Guid userId, CancellationToken ct = default);
    Task<IReadOnlyList<LearningProgress>> GetAllAsync(CancellationToken ct = default);
    Task UpsertAsync(LearningProgress progress, CancellationToken ct = default);
}

public interface ISubscriptionRepository
{
    Task<Subscription?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<Subscription>> GetByUserAsync(Guid userId, CancellationToken ct = default);
    Task<IReadOnlyList<Subscription>> GetAllAsync(CancellationToken ct = default);
    Task<Subscription> AddAsync(Subscription subscription, CancellationToken ct = default);
    Task UpdateAsync(Subscription subscription, CancellationToken ct = default);
}

public interface IPaymentRepository
{
    Task<IReadOnlyList<Payment>> GetByUserAsync(Guid userId, CancellationToken ct = default);
    Task<IReadOnlyList<Payment>> GetAllAsync(CancellationToken ct = default);
    Task<Payment> AddAsync(Payment payment, CancellationToken ct = default);
    Task UpdateAsync(Payment payment, CancellationToken ct = default);
}

public interface ICertificateRepository
{
    Task<Certificate?> GetByCodeAsync(string code, CancellationToken ct = default);
    Task<IReadOnlyList<Certificate>> GetByUserAsync(Guid userId, CancellationToken ct = default);
    Task<IReadOnlyList<Certificate>> GetAllAsync(CancellationToken ct = default);
    Task<Certificate> AddAsync(Certificate certificate, CancellationToken ct = default);
}

public interface INotificationRepository
{
    Task<IReadOnlyList<Notification>> GetByUserAsync(Guid userId, CancellationToken ct = default);
    Task<Notification> AddAsync(Notification notification, CancellationToken ct = default);
    Task MarkReadAsync(Guid id, CancellationToken ct = default);
}

public interface IAnnouncementRepository
{
    Task<IReadOnlyList<Announcement>> GetPublishedAsync(CancellationToken ct = default);
    Task<Announcement> AddAsync(Announcement announcement, CancellationToken ct = default);
}

public interface ISupportTicketRepository
{
    Task<SupportTicket?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<SupportTicket>> GetByUserAsync(Guid userId, CancellationToken ct = default);
    Task<IReadOnlyList<SupportTicket>> GetAllAsync(CancellationToken ct = default);
    Task<SupportTicket> AddAsync(SupportTicket ticket, CancellationToken ct = default);
    Task UpdateAsync(SupportTicket ticket, CancellationToken ct = default);
}

public interface IAuditLogRepository
{
    Task AddAsync(AuditLog log, CancellationToken ct = default);
    Task<IReadOnlyList<AuditLog>> GetRecentAsync(int take = 50, CancellationToken ct = default);
}

public interface IAiSessionRepository
{
    Task<AiSession> AddAsync(AiSession session, CancellationToken ct = default);
    Task<IReadOnlyList<AiSession>> GetByUserAsync(Guid userId, CancellationToken ct = default);
}

public interface IEnquiryRepository
{
    Task<Enquiry?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<Enquiry>> GetAllAsync(CancellationToken ct = default);
    Task<Enquiry> AddAsync(Enquiry enquiry, CancellationToken ct = default);
    Task UpdateAsync(Enquiry enquiry, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}
