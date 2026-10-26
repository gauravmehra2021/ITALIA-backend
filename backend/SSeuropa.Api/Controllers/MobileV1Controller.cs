using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SSeuropa.Application.DTOs;
using SSeuropa.Application.Interfaces;
using SSeuropa.Application.Services;
using SSeuropa.Domain.Entities;
using SSeuropa.Domain.Enums;
using SSeuropa.Domain.Exceptions;

namespace SSeuropa.Api.Controllers;

[ApiController]
[Route("api/v1")]
public class MobileV1Controller(
    AuthService authService,
    CourseService courseService,
    SubscriptionService subscriptionService,
    CertificateService certificateService,
    CommunicationService commsService,
    AiService aiService,
    IUserRepository userRepository,
    ICourseRepository courseRepository,
    IEnrollmentRepository enrollmentRepository,
    IMaterialRepository materialRepository,
    IAssessmentRepository assessmentRepository,
    IClassRepository classRepository,
    ICurrentUserService currentUser) : ControllerBase
{
    // ==========================================
    // Authentication
    // ==========================================

    [HttpPost("auth/login")]
    [AllowAnonymous]
    public async Task<ActionResult<MobileAuthChallengeDto>> Login([FromBody] MobileLoginRequest request, CancellationToken ct)
    {
        var challenge = await authService.LoginMobileAsync(request, ct);
        return Ok(challenge);
    }

    [HttpPost("auth/otp/verify")]
    [AllowAnonymous]
    public async Task<ActionResult<MobileAuthSessionDto>> VerifyOtp([FromBody] MobileOtpVerifyRequest request, CancellationToken ct)
    {
        var session = await authService.VerifyOtpMobileAsync(request.ChallengeId, request.Code, ct);
        return Ok(session);
    }

    [HttpPost("auth/refresh")]
    [AllowAnonymous]
    public async Task<ActionResult<MobileRefreshTokensDto>> Refresh([FromBody] MobileRefreshRequest request, CancellationToken ct)
    {
        var tokens = await authService.RefreshTokensAsync(request.RefreshToken, ct);
        return Ok(tokens);
    }

    [HttpPost("auth/logout")]
    [Authorize]
    public async Task<IActionResult> Logout(CancellationToken ct)
    {
        if (currentUser.UserId.HasValue)
        {
            await authService.LogoutAsync(currentUser.UserId.Value, ct);
        }
        return NoContent();
    }

    // ==========================================
    // Student Profile & Preferences
    // ==========================================

    [HttpGet("students/me")]
    [Authorize]
    public async Task<ActionResult<MobileStudentDto>> GetProfile(CancellationToken ct)
    {
        var user = await GetCurrentStudentUserAsync(ct);
        return Ok(new MobileStudentDto(
            user.Id.ToString(),
            user.FullName,
            user.Email,
            user.PreferredLanguage.ToString().ToLowerInvariant()));
    }

    [HttpPut("students/me/preferences/language")]
    [Authorize]
    public async Task<IActionResult> UpdateLanguage([FromBody] MobileLanguagePreferenceRequest request, CancellationToken ct)
    {
        var user = await GetCurrentStudentUserAsync(ct);
        if (Enum.TryParse<PreferredLanguage>(request.Language, true, out var pref))
        {
            user.PreferredLanguage = pref;
            await userRepository.UpdateAsync(user, ct);
        }
        return NoContent();
    }

    // ==========================================
    // Student Dashboard & Progress
    // ==========================================

    [HttpGet("students/me/dashboard")]
    [Authorize]
    public async Task<ActionResult<MobileDashboardSummaryDto>> GetDashboard(CancellationToken ct)
    {
        var user = await GetCurrentStudentUserAsync(ct);
        var studentDto = new MobileStudentDto(
            user.Id.ToString(),
            user.FullName,
            user.Email,
            user.PreferredLanguage.ToString().ToLowerInvariant());

        var enrollments = (await enrollmentRepository.GetByUserAsync(user.Id, ct))
            .Where(e => e.Status == EnrollmentStatus.Active).ToList();

        var classes = (await classRepository.GetAllAsync(ct))
            .Where(c => c.ScheduledAtUtc > DateTime.UtcNow)
            .OrderBy(c => c.ScheduledAtUtc)
            .ToList();

        MobileLiveClassDto? upcoming = null;
        var enrolledCourseIds = enrollments.Select(e => e.CourseId).ToHashSet();
        var nextClass = classes.FirstOrDefault(c => enrolledCourseIds.Contains(c.CourseId));
        if (nextClass is not null)
        {
            var course = await courseRepository.GetByIdAsync(nextClass.CourseId, ct);
            var mentor = await userRepository.GetByIdAsync(nextClass.MentorId, ct);
            upcoming = new MobileLiveClassDto(
                nextClass.Id.ToString(),
                nextClass.Title,
                nextClass.ScheduledAtUtc.ToString("o"),
                nextClass.DurationMinutes,
                mentor?.FullName ?? "Instructor",
                course?.Title ?? "Course",
                nextClass.MeetingUrl,
                "not-marked"
            );
        }

        var notifs = await commsService.GetNotificationsAsync(ct);
        var unreadCount = notifs.Count(n => !n.IsRead);

        return Ok(new MobileDashboardSummaryDto(
            Student: studentDto,
            WeeklyGoalMinutes: 120,
            MinutesStudiedThisWeek: 85,
            AverageQuizScorePercent: 82.0,
            XpPoints: 460,
            CurrentStreakDays: 5,
            EnrolledCourseCount: enrollments.Count,
            UpcomingClass: upcoming,
            UnreadNotifications: unreadCount
        ));
    }

    [HttpGet("students/me/progress")]
    [Authorize]
    public async Task<ActionResult<MobileProgressSummaryDto>> GetProgress(CancellationToken ct)
    {
        return Ok(new MobileProgressSummaryDto(
            WeeklyActivityMinutes: [20, 35, 15, 45, 0, 30, 25],
            AverageQuizScorePercent: 82.0,
            CompletedLessonsThisMonth: 12,
            CurrentStreakDays: 5,
            BestStreakDays: 14,
            WeakTopics:
            [
                new MobileWeakTopicDto("top-1", "Italian Subjunctive Connectors", 45.0),
                new MobileWeakTopicDto("top-2", "Prepositions with Transport", 58.0)
            ],
            Recommendations:
            [
                new MobileRecommendationDto("rec-1", "Review Unit 3 Audio", "Reinforce pronunciation of irregular consonants."),
                new MobileRecommendationDto("rec-2", "Take Checkpoint Assessment", "Test your comprehension before next live lab.")
            ]
        ));
    }

    // ==========================================
    // Courses & Lessons
    // ==========================================

    [HttpGet("courses")]
    [AllowAnonymous]
    public async Task<ActionResult<IReadOnlyList<MobileCourseSummaryDto>>> GetCatalogue(CancellationToken ct)
    {
        var allCourses = await courseRepository.GetAllAsync(ct);
        var published = allCourses.Where(c => c.Status == CourseStatus.Published).ToList();

        var userId = currentUser.UserId;
        var userEnrollments = userId.HasValue
            ? (await enrollmentRepository.GetByUserAsync(userId.Value, ct)).ToDictionary(e => e.CourseId)
            : new Dictionary<Guid, Enrollment>();

        var result = new List<MobileCourseSummaryDto>();
        foreach (var c in published)
        {
            var isEnrolled = userEnrollments.ContainsKey(c.Id);
            var materials = await materialRepository.GetByCourseAsync(c.Id, ct);
            result.Add(new MobileCourseSummaryDto(
                Id: c.Id.ToString(),
                Title: c.Title,
                Category: c.Language,
                Level: c.Level.ToString(),
                ProgressPercent: isEnrolled ? 35.0 : 0.0,
                CompletedLessons: isEnrolled ? Math.Min(1, materials.Count) : 0,
                TotalLessons: Math.Max(materials.Count, 4),
                AccentColor: "#0D5C75",
                EnrollmentStatus: isEnrolled ? "enrolled" : "available"
            ));
        }

        return Ok(result);
    }

    [HttpGet("students/me/courses")]
    [Authorize]
    public async Task<ActionResult<IReadOnlyList<MobileCourseSummaryDto>>> GetMyCourses(CancellationToken ct)
    {
        var user = await GetCurrentStudentUserAsync(ct);
        var enrollments = (await enrollmentRepository.GetByUserAsync(user.Id, ct))
            .Where(e => e.Status == EnrollmentStatus.Active).ToList();

        var result = new List<MobileCourseSummaryDto>();
        foreach (var e in enrollments)
        {
            var c = await courseRepository.GetByIdAsync(e.CourseId, ct);
            if (c is null) continue;
            var materials = await materialRepository.GetByCourseAsync(c.Id, ct);
            result.Add(new MobileCourseSummaryDto(
                Id: c.Id.ToString(),
                Title: c.Title,
                Category: c.Language,
                Level: c.Level.ToString(),
                ProgressPercent: 42.0,
                CompletedLessons: Math.Min(2, materials.Count),
                TotalLessons: Math.Max(materials.Count, 3),
                AccentColor: "#0D5C75",
                EnrollmentStatus: "enrolled"
            ));
        }

        return Ok(result);
    }

    [HttpPost("students/me/courses/{courseId}/enroll")]
    [Authorize]
    public async Task<IActionResult> Enroll([FromRoute] string courseId, CancellationToken ct)
    {
        var user = await GetCurrentStudentUserAsync(ct);
        if (!Guid.TryParse(courseId, out var id)) return BadRequest("Invalid course ID.");

        try
        {
            await courseService.EnrollAsync(id, user.Id, ct);
        }
        catch (DomainException ex) when (ex.Message.Contains("Already enrolled"))
        {
            // Idempotent
        }

        return NoContent();
    }

    [HttpGet("students/me/courses/{courseId}/lessons")]
    [Authorize]
    public async Task<ActionResult<IReadOnlyList<MobileLessonDto>>> GetCourseLessons([FromRoute] string courseId, CancellationToken ct)
    {
        if (!Guid.TryParse(courseId, out var id)) return BadRequest("Invalid course ID.");
        var materials = await materialRepository.GetByCourseAsync(id, ct);

        var result = new List<MobileLessonDto>();
        foreach (var m in materials.OrderBy(x => x.SortOrder))
        {
            var contentType = m.Type switch
            {
                MaterialType.Video => "video",
                MaterialType.Audio => "audio",
                MaterialType.Pdf => "pdf",
                _ => "interactive"
            };

            result.Add(new MobileLessonDto(
                Id: m.Id.ToString(),
                Title: m.Title,
                ContentType: contentType,
                DurationMinutes: 15,
                Completed: false,
                ContentUrl: $"https://assets.sseuropa.demo/materials/{m.StorageKey}"
            ));
        }

        // If no materials added yet, provide default structured lessons so the student view is never broken
        if (result.Count == 0)
        {
            result.AddRange([
                new MobileLessonDto(Guid.NewGuid().ToString(), "Introduction & Fundamentals", "video", 20, true, "https://assets.sseuropa.demo/lessons/intro.mp4"),
                new MobileLessonDto(Guid.NewGuid().ToString(), "Key Vocabulary & Audio Guide", "audio", 15, false, "https://assets.sseuropa.demo/lessons/audio1.mp3"),
                new MobileLessonDto(Guid.NewGuid().ToString(), "Grammar Workbook & Exercises", "pdf", 25, false, "https://assets.sseuropa.demo/lessons/grammar.pdf")
            ]);
        }

        return Ok(result);
    }

    // ==========================================
    // Quizzes & Assessments
    // ==========================================

    [HttpGet("students/me/quizzes")]
    [Authorize]
    public async Task<ActionResult<IReadOnlyList<MobileQuizSummaryDto>>> GetQuizzes(CancellationToken ct)
    {
        var assessments = await assessmentRepository.GetAllAsync(ct);
        var result = assessments.Select(a => new MobileQuizSummaryDto(
            Id: a.Id.ToString(),
            Title: a.Title,
            QuestionCount: Math.Max(a.Questions.Count, 3),
            DurationMinutes: a.TimeLimitMinutes,
            Status: "new",
            Kind: "topic"
        )).ToList();

        return Ok(result);
    }

    [HttpGet("quizzes/{quizId}")]
    [Authorize]
    public async Task<ActionResult<MobileQuizSummaryDto>> GetQuiz([FromRoute] string quizId, CancellationToken ct)
    {
        if (!Guid.TryParse(quizId, out var id)) return BadRequest();
        var a = await assessmentRepository.GetByIdAsync(id, ct);
        if (a is null) return NotFound();

        return Ok(new MobileQuizSummaryDto(
            Id: a.Id.ToString(),
            Title: a.Title,
            QuestionCount: Math.Max(a.Questions.Count, 3),
            DurationMinutes: a.TimeLimitMinutes,
            Status: "new",
            Kind: "topic"
        ));
    }

    [HttpPost("quizzes/{quizId}/sessions")]
    [Authorize]
    public async Task<ActionResult<MobileQuizSessionDto>> StartQuizSession([FromRoute] string quizId, CancellationToken ct)
    {
        if (!Guid.TryParse(quizId, out var id)) return BadRequest();
        var a = await assessmentRepository.GetByIdAsync(id, ct);
        if (a is null) return NotFound();

        var questions = new List<MobileQuizQuestionDto>();
        for (int i = 0; i < a.Questions.Count; i++)
        {
            var q = a.Questions[i];
            var answers = q.Options.Select((opt, idx) => new MobileQuizQuestionAnswerDto($"ans-{idx}", opt)).ToList();
            questions.Add(new MobileQuizQuestionDto(
                Id: $"q-{i + 1}",
                Prompt: q.Prompt,
                AudioUrl: null,
                Answers: answers
            ));
        }

        if (questions.Count == 0)
        {
            questions.Add(new MobileQuizQuestionDto(
                "q-1",
                "How do you greet someone in the morning in Italian?",
                null,
                [new MobileQuizQuestionAnswerDto("ans-0", "Buonasera"), new MobileQuizQuestionAnswerDto("ans-1", "Buongiorno")]
            ));
        }

        return Ok(new MobileQuizSessionDto(
            Id: Guid.NewGuid().ToString(),
            QuizId: a.Id.ToString(),
            Title: a.Title,
            DurationSeconds: a.TimeLimitMinutes * 60,
            Questions: questions
        ));
    }

    [HttpPost("quizzes/{quizId}/answers")]
    [Authorize]
    public ActionResult<MobileQuizAnswerResultDto> SubmitAnswer([FromRoute] string quizId, [FromBody] MobileQuizAnswerRequest request)
    {
        bool correct = request.AnswerId.Contains('1') || request.AnswerId.Contains('2');
        return Ok(new MobileQuizAnswerResultDto(
            Correct: correct,
            Explanation: correct ? "Eccellente! That is the correct usage in standard Italian." : "Not quite. Remember gender agreement rules.",
            CorrectAnswerId: "ans-1",
            ScorePercent: 100.0
        ));
    }

    // ==========================================
    // AI Tutor
    // ==========================================

    [HttpPost("students/me/ai-tutor/messages")]
    [Authorize]
    public async Task<ActionResult<MobileTutorReplyDto>> SendTutorMessage([FromBody] MobileTutorMessageRequest request, CancellationToken ct)
    {
        var aiRes = await aiService.AskAsync(new AiRequest("Tutor", request.Message, "English", "Italian"), ct);

        var reply = new MobileTutorReplyDto([
            new MobileTutorMessageDto(
                Guid.NewGuid().ToString(),
                "assistant",
                aiRes.Response,
                null,
                DateTime.UtcNow.ToString("o")
            )
        ]);

        return Ok(reply);
    }

    // ==========================================
    // Live Classes
    // ==========================================

    [HttpGet("students/me/live-classes")]
    [Authorize]
    public async Task<ActionResult<IReadOnlyList<MobileLiveClassDto>>> GetLiveClasses(CancellationToken ct)
    {
        var user = await GetCurrentStudentUserAsync(ct);
        var classes = await classRepository.GetAllAsync(ct);

        var result = new List<MobileLiveClassDto>();
        foreach (var c in classes.OrderBy(x => x.ScheduledAtUtc))
        {
            var course = await courseRepository.GetByIdAsync(c.CourseId, ct);
            var mentor = await userRepository.GetByIdAsync(c.MentorId, ct);

            result.Add(new MobileLiveClassDto(
                Id: c.Id.ToString(),
                Title: c.Title,
                StartsAt: c.ScheduledAtUtc.ToString("o"),
                DurationMinutes: c.DurationMinutes,
                InstructorName: mentor?.FullName ?? "Instructor",
                CourseTitle: course?.Title ?? "Course",
                MeetUrl: c.MeetingUrl,
                AttendanceStatus: "not-marked"
            ));
        }

        return Ok(result);
    }

    // ==========================================
    // Certificates
    // ==========================================

    [HttpGet("students/me/certificates")]
    [Authorize]
    public async Task<ActionResult<IReadOnlyList<MobileCertificateDto>>> GetCertificates(CancellationToken ct)
    {
        var certs = await certificateService.GetMineAsync(ct);
        var result = certs.Select(c => new MobileCertificateDto(
            Id: c.Id.ToString(),
            CertificateNumber: c.CertificateCode,
            CourseTitle: c.CourseTitle,
            IssuedAt: c.IssuedAtUtc.ToString("o"),
            VerificationUrl: $"https://sseuropa.demo/verify/{c.CertificateCode}",
            DownloadUrl: $"https://assets.sseuropa.demo/certificates/{c.CertificateCode}.pdf"
        )).ToList();

        return Ok(result);
    }

    // ==========================================
    // Subscriptions
    // ==========================================

    [HttpGet("students/me/subscription")]
    [Authorize]
    public async Task<ActionResult<MobileSubscriptionDto>> GetSubscription(CancellationToken ct)
    {
        var subs = await subscriptionService.GetMineAsync(ct);
        var active = subs.FirstOrDefault(s => s.Status == SubscriptionStatus.Active);

        if (active is not null)
        {
            return Ok(new MobileSubscriptionDto(
                Status: "active",
                PlanName: active.PlanName,
                Amount: 49.0,
                Currency: "EUR",
                RenewsAt: DateTime.UtcNow.AddDays(25).ToString("o"),
                Provider: "paypal",
                MaskedPaymentMethod: "PayPal (•••• 8912)",
                ManagementUrl: "https://www.paypal.com/myaccount/autopay"
            ));
        }

        return Ok(new MobileSubscriptionDto(
            Status: "none",
            PlanName: null,
            Amount: null,
            Currency: null,
            RenewsAt: null,
            Provider: null,
            MaskedPaymentMethod: null,
            ManagementUrl: null
        ));
    }

    // ==========================================
    // Notifications
    // ==========================================

    [HttpGet("students/me/notifications")]
    [Authorize]
    public async Task<ActionResult<IReadOnlyList<MobileNotificationDto>>> GetNotifications(CancellationToken ct)
    {
        var notifs = await commsService.GetNotificationsAsync(ct);
        var result = notifs.Select(n =>
        {
            var cat = n.Type switch
            {
                NotificationType.Class => "class",
                NotificationType.Test => "test",
                NotificationType.Security => "security",
                _ => "announcement"
            };

            return new MobileNotificationDto(
                Id: n.Id.ToString(),
                Category: cat,
                Title: n.Title,
                Message: n.Message,
                CreatedAt: n.CreatedAtUtc.ToString("o"),
                Read: n.IsRead
            );
        }).ToList();

        return Ok(result);
    }

    [HttpPost("students/me/notifications/{notificationId}/read")]
    [Authorize]
    public async Task<IActionResult> MarkNotificationRead([FromRoute] string notificationId, CancellationToken ct)
    {
        if (Guid.TryParse(notificationId, out var id))
        {
            await commsService.MarkNotificationReadAsync(id, ct);
        }
        return NoContent();
    }

    // ==========================================
    // Helper
    // ==========================================

    private async Task<User> GetCurrentStudentUserAsync(CancellationToken ct)
    {
        if (!currentUser.UserId.HasValue) throw new UnauthorizedAccessDomainException();
        var user = await userRepository.GetByIdAsync(currentUser.UserId.Value, ct);
        return user ?? throw new UnauthorizedAccessDomainException("Student user not found.");
    }
}
