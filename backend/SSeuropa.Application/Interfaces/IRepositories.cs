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
    Task UpdateAsync(Material material, CancellationToken ct = default);
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
    /// <summary>Newest first, optionally limited to [fromUtc, toUtc).</summary>
    Task<IReadOnlyList<AuditLog>> GetRangeAsync(DateTime? fromUtc, DateTime? toUtc, int take, CancellationToken ct = default);
    /// <summary>Permanently removes entries in [fromUtc, toUtc) (both null = everything). Returns how many went.</summary>
    Task<long> DeleteRangeAsync(DateTime? fromUtc, DateTime? toUtc, CancellationToken ct = default);
    Task<long> DeleteManyAsync(IReadOnlyCollection<Guid> ids, CancellationToken ct = default);
}

public interface IAiSessionRepository
{
    Task<AiSession> AddAsync(AiSession session, CancellationToken ct = default);
    Task<IReadOnlyList<AiSession>> GetByUserAsync(Guid userId, CancellationToken ct = default);
}

/// <summary>Translation memory for lesson content (see ContentTranslation).</summary>
public interface ITranslationRepository
{
    Task<IReadOnlyList<ContentTranslation>> GetAllAsync(CancellationToken ct = default);
    Task<IReadOnlyList<ContentTranslation>> GetBySourcesAsync(IReadOnlyCollection<string> sources, CancellationToken ct = default);
    Task<ContentTranslation?> GetBySourceAsync(string source, CancellationToken ct = default);
    Task AddRangeAsync(IReadOnlyCollection<ContentTranslation> items, CancellationToken ct = default);
    Task UpdateAsync(ContentTranslation item, CancellationToken ct = default);
    /// <summary>Saves many changed rows in a few round-trips (used by the seeder).</summary>
    Task UpdateRangeAsync(IReadOnlyCollection<ContentTranslation> items, CancellationToken ct = default);
}

public interface IEnquiryRepository
{
    Task<Enquiry?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<Enquiry>> GetAllAsync(CancellationToken ct = default);
    Task<Enquiry> AddAsync(Enquiry enquiry, CancellationToken ct = default);
    Task UpdateAsync(Enquiry enquiry, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}

public interface IAssignmentRepository
{
    Task<Assignment?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<Assignment>> GetByCourseAsync(Guid courseId, CancellationToken ct = default);
    Task<Assignment> AddAsync(Assignment assignment, CancellationToken ct = default);
    Task UpdateAsync(Assignment assignment, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);

    Task<AssignmentSubmission?> GetSubmissionByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<AssignmentSubmission>> GetSubmissionsByAssignmentAsync(Guid assignmentId, CancellationToken ct = default);
    Task<IReadOnlyList<AssignmentSubmission>> GetSubmissionsByStudentAsync(Guid studentId, CancellationToken ct = default);
    Task<AssignmentSubmission?> GetStudentSubmissionForAssignmentAsync(Guid assignmentId, Guid studentId, CancellationToken ct = default);
    Task<AssignmentSubmission> AddSubmissionAsync(AssignmentSubmission submission, CancellationToken ct = default);
    Task UpdateSubmissionAsync(AssignmentSubmission submission, CancellationToken ct = default);
}

public interface ILearningResourceRepository
{
    Task<LearningResource?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<LearningResource?> GetByObjectKeyAsync(string objectKey, CancellationToken ct = default);
    Task<IReadOnlyList<LearningResource>> GetByCourseAsync(Guid courseId, CancellationToken ct = default);
    Task<LearningResource> AddAsync(LearningResource resource, CancellationToken ct = default);
    Task UpdateAsync(LearningResource resource, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}



/// <summary>Units, lessons and per-student lesson progress for the course curriculum.</summary>
public interface ICurriculumRepository
{
    Task<IReadOnlyList<CourseUnit>> GetUnitsAsync(Guid courseId, CancellationToken ct = default);
    Task<CourseUnit?> GetUnitAsync(Guid id, CancellationToken ct = default);
    Task AddUnitAsync(CourseUnit unit, CancellationToken ct = default);
    Task UpdateUnitAsync(CourseUnit unit, CancellationToken ct = default);
    Task DeleteUnitAsync(Guid id, CancellationToken ct = default);

    Task<IReadOnlyList<CourseModule>> GetModulesAsync(Guid courseId, CancellationToken ct = default);
    Task<IReadOnlyList<CourseModule>> GetModulesByUnitAsync(Guid unitId, CancellationToken ct = default);
    Task<CourseModule?> GetModuleAsync(Guid id, CancellationToken ct = default);
    Task AddModuleAsync(CourseModule module, CancellationToken ct = default);
    Task UpdateModuleAsync(CourseModule module, CancellationToken ct = default);
    /// <summary>Removes the module, its lessons and the progress in those lessons.</summary>
    Task DeleteModuleAsync(Guid id, CancellationToken ct = default);

    Task<IReadOnlyList<Lesson>> GetLessonsByCourseAsync(Guid courseId, CancellationToken ct = default);
    Task<IReadOnlyList<Lesson>> GetLessonsByUnitAsync(Guid unitId, CancellationToken ct = default);
    Task<Lesson?> GetLessonAsync(Guid id, CancellationToken ct = default);
    Task AddLessonAsync(Lesson lesson, CancellationToken ct = default);
    Task UpdateLessonAsync(Lesson lesson, CancellationToken ct = default);
    Task DeleteLessonAsync(Guid id, CancellationToken ct = default);

    Task<IReadOnlyList<LessonProgress>> GetProgressAsync(Guid userId, Guid courseId, CancellationToken ct = default);
    Task<IReadOnlyList<LessonProgress>> GetAllProgressForUserAsync(Guid userId, CancellationToken ct = default);
    Task<IReadOnlyList<LessonProgress>> GetProgressForCourseAsync(Guid courseId, CancellationToken ct = default);
    Task<LessonProgress?> GetLessonProgressAsync(Guid userId, Guid lessonId, CancellationToken ct = default);
    Task UpsertProgressAsync(LessonProgress progress, CancellationToken ct = default);

    /// <summary>Removes every unit, lesson and progress record of a course.</summary>
    Task DeleteCourseCurriculumAsync(Guid courseId, CancellationToken ct = default);
}

/// <summary>Student batches (groups) and the courses assigned to them.</summary>
public interface IBatchRepository
{
    Task<Batch?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<Batch>> GetAllAsync(CancellationToken ct = default);
    Task<Batch> AddAsync(Batch batch, CancellationToken ct = default);
    Task UpdateAsync(Batch batch, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}
