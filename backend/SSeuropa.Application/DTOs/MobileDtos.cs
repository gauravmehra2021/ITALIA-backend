namespace SSeuropa.Application.DTOs;

public record MobileStudentDto(
    string Id,
    string DisplayName,
    string Email,
    string PreferredLanguage
);

public record MobileCourseSummaryDto(
    string Id,
    string Title,
    string Category,
    string Level,
    double ProgressPercent,
    int CompletedLessons,
    int TotalLessons,
    string? AccentColor,
    string EnrollmentStatus // "enrolled" | "available" | "completed"
);

public record MobileLessonDto(
    string Id,
    string Title,
    string ContentType, // "video" | "pdf" | "audio" | "interactive"
    double? DurationMinutes,
    bool Completed,
    string? ContentUrl
);

public record MobileQuizSummaryDto(
    string Id,
    string Title,
    int QuestionCount,
    double DurationMinutes,
    string Status, // "new" | "in-progress" | "completed"
    string Kind // "topic" | "random" | "exam" | "weak-topics"
);

public record MobileQuizQuestionAnswerDto(
    string Id,
    string Text
);

public record MobileQuizQuestionDto(
    string Id,
    string Prompt,
    string? AudioUrl,
    IReadOnlyList<MobileQuizQuestionAnswerDto> Answers
);

public record MobileQuizSessionDto(
    string Id,
    string QuizId,
    string Title,
    int DurationSeconds,
    IReadOnlyList<MobileQuizQuestionDto> Questions
);

public record MobileQuizAnswerRequest(
    string SessionId,
    string QuestionId,
    string AnswerId
);

public record MobileQuizAnswerResultDto(
    bool Correct,
    string Explanation,
    string CorrectAnswerId,
    double? ScorePercent
);

public record MobileDashboardSummaryDto(
    MobileStudentDto Student,
    double WeeklyGoalMinutes,
    double MinutesStudiedThisWeek,
    double AverageQuizScorePercent,
    int XpPoints,
    int CurrentStreakDays,
    int EnrolledCourseCount,
    MobileLiveClassDto? UpcomingClass,
    int UnreadNotifications
);

public record MobileWeakTopicDto(
    string Id,
    string Name,
    double MasteryPercent
);

public record MobileRecommendationDto(
    string Id,
    string Title,
    string Description
);

public record MobileProgressSummaryDto(
    IReadOnlyList<double> WeeklyActivityMinutes,
    double AverageQuizScorePercent,
    int CompletedLessonsThisMonth,
    int CurrentStreakDays,
    int BestStreakDays,
    IReadOnlyList<MobileWeakTopicDto> WeakTopics,
    IReadOnlyList<MobileRecommendationDto> Recommendations
);

public record MobileLiveClassDto(
    string Id,
    string Title,
    string StartsAt,
    double DurationMinutes,
    string InstructorName,
    string CourseTitle,
    string? MeetUrl,
    string AttendanceStatus // "not-marked" | "present" | "absent"
);

public record MobileCertificateDto(
    string Id,
    string CertificateNumber,
    string CourseTitle,
    string IssuedAt,
    string VerificationUrl,
    string? DownloadUrl
);

public record MobileSubscriptionDto(
    string Status, // "active" | "pending" | "expired" | "none"
    string? PlanName,
    double? Amount,
    string? Currency,
    string? RenewsAt,
    string? Provider, // "paypal" | "other"
    string? MaskedPaymentMethod,
    string? ManagementUrl
);

public record MobileNotificationDto(
    string Id,
    string Category, // "class" | "test" | "announcement" | "security"
    string Title,
    string Message,
    string CreatedAt,
    bool Read
);

public record MobileTutorMessageDto(
    string Id,
    string Role, // "student" | "assistant"
    string Content,
    string? AudioUrl,
    string CreatedAt
);

public record MobileTutorReplyDto(
    IReadOnlyList<MobileTutorMessageDto> Messages
);

public record MobileTutorMessageRequest(
    string Message
);

public record MobileLoginRequest(
    string Email,
    string Password,
    string? DeviceId = null,
    string? DeviceName = null
);

public record MobileAuthChallengeDto(
    string ChallengeId,
    string ExpiresAt
);

public record MobileAuthSessionDto(
    string AccessToken,
    string RefreshToken,
    MobileStudentDto Student
);

public record MobileRefreshRequest(
    string RefreshToken
);

public record MobileRefreshTokensDto(
    string AccessToken,
    string RefreshToken
);

public record MobileOtpVerifyRequest(
    string ChallengeId,
    string Code
);

public record MobileLanguagePreferenceRequest(
    string Language
);

public record MobileRegisterRequest(
    string FirstName,
    string LastName,
    string Email,
    string Password,
    string? PreferredLanguage = "en"
);

public record MobileForgotPasswordRequest(
    string Email
);

public record MobileResetPasswordRequest(
    string ChallengeId,
    string Code,
    string NewPassword
);
