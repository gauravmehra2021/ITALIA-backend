using SSeuropa.Domain.Entities;
using SSeuropa.Domain.Enums;

namespace SSeuropa.Infrastructure.Persistence;

/// <summary>
/// In-memory prototype store. Data resets when the API process restarts.
/// Replace with MongoDB adapters without changing Application interfaces.
/// </summary>
public class InMemoryStore
{
    public List<User> Users { get; } = [];
    public List<UserSession> Sessions { get; } = [];
    public List<OtpChallenge> Otps { get; } = [];
    public List<Course> Courses { get; } = [];
    public List<ClassSession> Classes { get; } = [];
    public List<Enrollment> Enrollments { get; } = [];
    public List<Material> Materials { get; } = [];
    public List<Assessment> Assessments { get; } = [];
    public List<TestAttempt> Attempts { get; } = [];
    public List<AttendanceRecord> Attendance { get; } = [];
    public List<LearningProgress> Progress { get; } = [];
    public List<Subscription> Subscriptions { get; } = [];
    public List<Payment> Payments { get; } = [];
    public List<Certificate> Certificates { get; } = [];
    public List<Notification> Notifications { get; } = [];
    public List<Announcement> Announcements { get; } = [];
    public List<SupportTicket> Tickets { get; } = [];
    public List<AuditLog> AuditLogs { get; } = [];
    public List<AiSession> AiSessions { get; } = [];
    public List<Enquiry> Enquiries { get; } = [];

    public static InMemoryStore CreateSeeded(Func<string, string> hashPassword)
    {
        var store = new InMemoryStore();

        var superAdminId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var adminId = Guid.Parse("22222222-2222-2222-2222-222222222222");
        var mentorId = Guid.Parse("33333333-3333-3333-3333-333333333333");
        var studentId = Guid.Parse("44444444-4444-4444-4444-444444444444");
        var student2Id = Guid.Parse("55555555-5555-5555-5555-555555555555");
        var staffId = Guid.Parse("66666666-6666-6666-6666-666666666666");

        store.Users.AddRange([
            new User
            {
                Id = superAdminId,
                Email = "superadmin@sseuropa.demo",
                PasswordHash = hashPassword("Demo@12345"),
                FirstName = "Sofia",
                LastName = "Rossi",
                Role = UserRole.SuperAdmin,
                Status = AccountStatus.Active,
                EmailVerified = true,
                PreferredLanguage = PreferredLanguage.Italian
            },
            new User
            {
                Id = adminId,
                Email = "admin@sseuropa.demo",
                PasswordHash = hashPassword("Demo@12345"),
                FirstName = "Marco",
                LastName = "Bianchi",
                Role = UserRole.Admin,
                Status = AccountStatus.Active,
                EmailVerified = true
            },
            new User
            {
                Id = mentorId,
                Email = "mentor@sseuropa.demo",
                PasswordHash = hashPassword("Demo@12345"),
                FirstName = "Giulia",
                LastName = "Conti",
                Role = UserRole.Mentor,
                Status = AccountStatus.Active,
                EmailVerified = true,
                PreferredLanguage = PreferredLanguage.Italian
            },
            new User
            {
                Id = studentId,
                Email = "student@sseuropa.demo",
                PasswordHash = hashPassword("Demo@12345"),
                FirstName = "Aarav",
                LastName = "Singh",
                Role = UserRole.User,
                Status = AccountStatus.Active,
                EmailVerified = true,
                PreferredLanguage = PreferredLanguage.English
            },
            new User
            {
                Id = student2Id,
                Email = "priya@sseuropa.demo",
                PasswordHash = hashPassword("Demo@12345"),
                FirstName = "Priya",
                LastName = "Sharma",
                Role = UserRole.User,
                Status = AccountStatus.Active,
                EmailVerified = true,
                PreferredLanguage = PreferredLanguage.Hindi
            },
            new User
            {
                Id = staffId,
                Email = "staff@sseuropa.demo",
                PasswordHash = hashPassword("Demo@12345"),
                FirstName = "Elena",
                LastName = "Moretti",
                Role = UserRole.Staff,
                Status = AccountStatus.Active,
                EmailVerified = true,
                PreferredLanguage = PreferredLanguage.Italian
            }
        ]);

        var course1 = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
        var course2 = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");
        var course3 = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc");

        store.Courses.AddRange([
            new Course
            {
                Id = course1,
                Title = "Italian Essentials A1",
                Description = "Foundational Italian for absolute beginners: greetings, numbers, present tense and everyday phrases.",
                Level = CourseLevel.Beginner,
                Status = CourseStatus.Published,
                Capacity = 40,
                EnrolledCount = 2,
                MentorId = mentorId,
                Language = "Italian",
                Price = 49,
                Currency = "EUR",
                DurationWeeks = 8,
                ThumbnailUrl = "/images/course-a1.svg"
            },
            new Course
            {
                Id = course2,
                Title = "Conversational Italian B1",
                Description = "Build fluency with guided dialogues, listening practice and cultural context for daily life in Italy.",
                Level = CourseLevel.Intermediate,
                Status = CourseStatus.Published,
                Capacity = 25,
                EnrolledCount = 1,
                MentorId = mentorId,
                Language = "Italian",
                Price = 79,
                Currency = "EUR",
                DurationWeeks = 10,
                ThumbnailUrl = "/images/course-b1.svg"
            },
            new Course
            {
                Id = course3,
                Title = "Advanced Grammar Studio C1",
                Description = "Master subjunctive, nuanced connectors and formal writing for academic and professional Italian.",
                Level = CourseLevel.Advanced,
                Status = CourseStatus.Draft,
                Capacity = 15,
                EnrolledCount = 0,
                MentorId = mentorId,
                Language = "Italian",
                Price = 99,
                Currency = "EUR",
                DurationWeeks = 12
            }
        ]);

        var class1 = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd");
        var class2 = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee");

        store.Classes.AddRange([
            new ClassSession
            {
                Id = class1,
                CourseId = course1,
                MentorId = mentorId,
                Title = "Week 1 — Ciao e presentazioni",
                ScheduledAtUtc = DateTime.UtcNow.AddDays(2).Date.AddHours(16),
                DurationMinutes = 60,
                Status = ClassStatus.Scheduled,
                MeetingUrl = "https://meet.google.com/mock-essentials-w1",
                MeetingProvider = "GoogleMeet"
            },
            new ClassSession
            {
                Id = class2,
                CourseId = course2,
                MentorId = mentorId,
                Title = "Live dialogue lab — al ristorante",
                ScheduledAtUtc = DateTime.UtcNow.AddDays(4).Date.AddHours(18),
                DurationMinutes = 75,
                Status = ClassStatus.Scheduled,
                MeetingUrl = "https://meet.google.com/mock-conversation-w3",
                MeetingProvider = "GoogleMeet"
            }
        ]);

        store.Enrollments.AddRange([
            new Enrollment
            {
                Id = Guid.Parse("f1111111-1111-1111-1111-111111111111"),
                UserId = studentId,
                CourseId = course1,
                Status = EnrollmentStatus.Active,
                ActivatedAtUtc = DateTime.UtcNow.AddDays(-14)
            },
            new Enrollment
            {
                Id = Guid.Parse("f2222222-2222-2222-2222-222222222222"),
                UserId = studentId,
                CourseId = course2,
                Status = EnrollmentStatus.Active,
                ActivatedAtUtc = DateTime.UtcNow.AddDays(-7)
            },
            new Enrollment
            {
                Id = Guid.Parse("f3333333-3333-3333-3333-333333333333"),
                UserId = student2Id,
                CourseId = course1,
                Status = EnrollmentStatus.Active,
                ActivatedAtUtc = DateTime.UtcNow.AddDays(-10)
            }
        ]);

        store.Materials.AddRange([
            new Material
            {
                CourseId = course1,
                Title = "Alphabet & pronunciation PDF",
                Description = "Printable guide with IPA notes and practice drills.",
                Type = MaterialType.Pdf,
                StorageKey = "mock/course1/alphabet.pdf",
                SortOrder = 1
            },
            new Material
            {
                CourseId = course1,
                Title = "Greetings audio pack",
                Description = "Native speaker recordings for ciao, buongiorno and polite forms.",
                Type = MaterialType.Audio,
                StorageKey = "mock/course1/greetings.mp3",
                SortOrder = 2
            },
            new Material
            {
                CourseId = course2,
                Title = "Restaurant dialogue video",
                Description = "Scene-based listening with captions.",
                Type = MaterialType.Video,
                StorageKey = "mock/course2/restaurant.mp4",
                SortOrder = 1
            }
        ]);

        var assessmentId = Guid.Parse("a1a1a1a1-a1a1-a1a1-a1a1-a1a1a1a1a1a1");
        store.Assessments.Add(new Assessment
        {
            Id = assessmentId,
            CourseId = course1,
            Title = "A1 Checkpoint Quiz",
            Description = "Quick check on greetings, articles and present tense.",
            PassingScore = 60,
            TimeLimitMinutes = 20,
            Questions =
            [
                new AssessmentQuestion
                {
                    Prompt = "How do you say 'Good morning' in Italian?",
                    Options = ["Buonasera", "Buongiorno", "Arrivederci", "Per favore"],
                    CorrectOptionIndex = 1,
                    Points = 2
                },
                new AssessmentQuestion
                {
                    Prompt = "Which article pairs with 'libro'?",
                    Options = ["la", "le", "il", "gli"],
                    CorrectOptionIndex = 2,
                    Points = 2
                },
                new AssessmentQuestion
                {
                    Prompt = "Select the correct conjugation: Io ___ italiano.",
                    Options = ["parli", "parla", "parlo", "parliamo"],
                    CorrectOptionIndex = 2,
                    Points = 2
                }
            ]
        });

        store.Progress.AddRange([
            new LearningProgress
            {
                UserId = studentId,
                CourseId = course1,
                CompletionPercent = 62,
                StreakDays = 5,
                GrammarScore = 71,
                VocabularyScore = 68,
                SpeakingScore = 55,
                LastActivityAtUtc = DateTime.UtcNow.AddHours(-6)
            },
            new LearningProgress
            {
                UserId = studentId,
                CourseId = course2,
                CompletionPercent = 34,
                StreakDays = 2,
                GrammarScore = 58,
                VocabularyScore = 61,
                SpeakingScore = 49,
                LastActivityAtUtc = DateTime.UtcNow.AddDays(-1)
            },
            new LearningProgress
            {
                UserId = student2Id,
                CourseId = course1,
                CompletionPercent = 81,
                StreakDays = 9,
                GrammarScore = 84,
                VocabularyScore = 79,
                SpeakingScore = 72,
                LastActivityAtUtc = DateTime.UtcNow.AddHours(-2)
            }
        ]);

        store.Subscriptions.Add(new Subscription
        {
            UserId = studentId,
            CourseId = course1,
            PlanName = "Monthly Essentials",
            Status = SubscriptionStatus.Active,
            PayPalSubscriptionId = "MOCK-SUB-1001",
            StartsAtUtc = DateTime.UtcNow.AddDays(-14),
            EndsAtUtc = DateTime.UtcNow.AddDays(16)
        });

        store.Payments.Add(new Payment
        {
            UserId = studentId,
            Amount = 49,
            Currency = "EUR",
            Status = PaymentStatus.Completed,
            PayPalReference = "MOCK-PAY-9001",
            Description = "Monthly Essentials – Italian Essentials A1"
        });

        store.Certificates.Add(new Certificate
        {
            UserId = student2Id,
            CourseId = course1,
            CertificateCode = "SSE-20261001-PRYA81",
            QrVerificationToken = "verify-priya-a1",
            IssuedAtUtc = DateTime.UtcNow.AddDays(-1)
        });

        store.Notifications.AddRange([
            new Notification
            {
                UserId = studentId,
                Title = "Upcoming class",
                Message = "Ciao! Your Week 1 class starts in 2 days.",
                Type = NotificationType.Class
            },
            new Notification
            {
                UserId = studentId,
                Title = "New material",
                Message = "Greetings audio pack is now available.",
                Type = NotificationType.Announcement
            },
            new Notification
            {
                UserId = mentorId,
                Title = "Attendance reminder",
                Message = "Mark attendance after tomorrow's live session.",
                Type = NotificationType.System
            }
        ]);

        store.Announcements.Add(new Announcement
        {
            Title = "Welcome to SS Europa",
            Body = "Explore courses, join live classes, and practise with the AI tutor. This prototype uses in-memory data.",
            CreatedByUserId = adminId,
            IsPublished = true
        });

        store.Tickets.Add(new SupportTicket
        {
            UserId = studentId,
            Subject = "Cannot open worksheet",
            Description = "The alphabet PDF link expires quickly on mobile.",
            Status = SupportTicketStatus.Open
        });

        store.AuditLogs.Add(new AuditLog
        {
            ActorUserId = adminId,
            Action = "seed",
            Resource = "platform",
            Details = "In-memory store seeded for prototype"
        });

        store.Enquiries.AddRange([
            new Enquiry
            {
                FullName = "Matteo Ricci",
                Email = "matteo.ricci@example.it",
                Phone = "+39 347 123 4567",
                InterestedCourse = "Italian Essentials A1",
                PreferredLanguage = "Italian",
                Message = "Hello! I am looking to start Italian basics for my family move to Florence. Are evening slots available?",
                Status = EnquiryStatus.New,
                CreatedAtUtc = DateTime.UtcNow.AddHours(-3)
            },
            new Enquiry
            {
                FullName = "Gurpreet Singh",
                Email = "gurpreet.singh@example.com",
                Phone = "+39 388 987 6543",
                InterestedCourse = "B1 Conversational Fluency",
                PreferredLanguage = "Punjabi",
                Message = "Sat Sri Akal! I need B1 Italian certificate for citizenship. What is the batch duration?",
                Status = EnquiryStatus.Contacted,
                StaffNotes = "Called on WhatsApp. Explained 8-week curriculum and weekday evening timings. Follow up scheduled on Thursday.",
                AssignedStaffId = staffId,
                AssignedStaffName = "Elena Moretti",
                CreatedAtUtc = DateTime.UtcNow.AddDays(-1)
            },
            new Enquiry
            {
                FullName = "Sara Al-Mansoor",
                Email = "sara.almansoor@example.com",
                Phone = "+39 320 555 1290",
                InterestedCourse = "Medical & Workplace Italian",
                PreferredLanguage = "English",
                Message = "I am a healthcare worker joining a hospital in Milan. Looking for specialized medical Italian practice.",
                Status = EnquiryStatus.Qualified,
                StaffNotes = "Strong candidate. Budget confirmed (€49/mo). Recommended starting next Monday cohort. Sent course brochure and enrollment link.",
                AssignedStaffId = staffId,
                AssignedStaffName = "Elena Moretti",
                CreatedAtUtc = DateTime.UtcNow.AddDays(-2)
            },
            new Enquiry
            {
                FullName = "Rajesh Kumar",
                Email = "rajesh.kumar@example.in",
                Phone = "+91 98123 45678",
                InterestedCourse = "Italian Essentials A1",
                PreferredLanguage = "Hindi",
                Message = "Interested in the A1 beginner package. Can I access recordings if I miss a live class?",
                Status = EnquiryStatus.Enrolled,
                StaffNotes = "Payment confirmed. Successfully converted to enrolled student.",
                AssignedStaffId = staffId,
                AssignedStaffName = "Elena Moretti",
                ConvertedUserId = studentId,
                CreatedAtUtc = DateTime.UtcNow.AddDays(-4)
            },
            new Enquiry
            {
                FullName = "Lucas Meyer",
                Email = "lucas.meyer@example.de",
                Phone = "+49 170 555 4321",
                InterestedCourse = "C1 Advanced Mastery",
                PreferredLanguage = "English",
                Message = "Do you offer C1 literature courses?",
                Status = EnquiryStatus.Closed,
                StaffNotes = "Informed that our current focus is A1 to B2. Closed for now, will notify when C1 cohort opens.",
                AssignedStaffId = staffId,
                AssignedStaffName = "Elena Moretti",
                CreatedAtUtc = DateTime.UtcNow.AddDays(-7)
            }
        ]);

        return store;
    }
}
