using SSeuropa.Domain.Entities;
using SSeuropa.Domain.Enums;

namespace SSeuropa.Application.DTOs;

// ── Shared ───────────────────────────────────────────────────────────────────

public record LessonSummaryDto(
    Guid Id,
    Guid UnitId,
    int Order,
    string Title,
    string TitleIt,
    LessonKind Kind,
    string Summary,
    int EstimatedMinutes,
    int XpReward,
    LessonStatus Status,
    bool IsFreePreview,
    int VocabularyCount,
    int ExerciseCount,
    bool HasVideo,
    bool HasAudio,
    bool HasPdf,
    // Student view only (null for staff views):
    bool? Completed = null,
    int? Stars = null,
    int? BestScorePercent = null,
    bool? Locked = null);

public record UnitDto(
    Guid Id,
    Guid CourseId,
    int Order,
    string Title,
    string TitleIt,
    string Description,
    List<string> CanDo,
    string? ImageUrl,
    string? PdfUrl,
    string? LiveClassFocus,
    LessonStatus Status,
    List<LessonSummaryDto> Lessons,
    int CompletedLessons = 0,
    int Stars = 0);

public record CourseOutlineDto(
    CourseDto Course,
    List<UnitDto> Units,
    int TotalLessons,
    int CompletedLessons,
    int CompletionPercent,
    int Stars,
    int MaxStars,
    int XpEarned,
    bool IsEnrolled,
    Guid? NextLessonId);

public record LessonDto(
    Guid Id,
    Guid CourseId,
    Guid UnitId,
    string UnitTitle,
    int Order,
    string Title,
    string TitleIt,
    LessonKind Kind,
    string Summary,
    int EstimatedMinutes,
    int XpReward,
    LessonStatus Status,
    bool IsFreePreview,
    List<LessonBlock> Blocks,
    List<LessonExercise> Exercises,
    string? Homework,
    Guid? PreviousLessonId,
    Guid? NextLessonId,
    int? Stars = null,
    int? BestScorePercent = null,
    bool? Completed = null);

// ── Staff: editing ───────────────────────────────────────────────────────────

public record SaveUnitRequest(
    string Title,
    string? TitleIt,
    string? Description,
    List<string>? CanDo,
    string? ImageUrl,
    string? PdfUrl,
    string? LiveClassFocus,
    LessonStatus Status = LessonStatus.Draft,
    int? Order = null);

public record SaveLessonRequest(
    string Title,
    string? TitleIt,
    LessonKind Kind,
    string? Summary,
    int EstimatedMinutes = 10,
    int XpReward = 20,
    LessonStatus Status = LessonStatus.Draft,
    bool IsFreePreview = false,
    List<LessonBlock>? Blocks = null,
    List<LessonExercise>? Exercises = null,
    string? Homework = null,
    int? Order = null,
    Guid? UnitId = null);

public record MoveRequest(int Direction);

// ── Student: results & overview ──────────────────────────────────────────────

public record CompleteLessonRequest(int ScorePercent, int CorrectAnswers = 0, int TotalQuestions = 0);

/// <summary>Language of the student portal: "English", "Punjabi", "Hindi" or "Urdu".</summary>
public record SetLanguageRequest(string Language);

public record LessonResultDto(
    Guid LessonId,
    int ScorePercent,
    int Stars,
    bool Completed,
    bool IsNewBest,
    int XpGained,
    int TotalXp,
    int StreakDays,
    int CourseCompletionPercent,
    Guid? NextLessonId,
    bool UnitCompleted,
    bool CourseCompleted);

public record CourseCardDto(
    CourseDto Course,
    bool IsEnrolled,
    int CompletionPercent,
    int CompletedLessons,
    int TotalLessons,
    Guid? NextLessonId,
    string? NextLessonTitle);

public record LearnerSummaryDto(
    int TotalXp,
    int StreakDays,
    int LessonsCompleted,
    int StarsEarned,
    int WordsLearned,
    List<CourseCardDto> MyCourses,
    CourseCardDto? ContinueCourse);

// ── Staff: student progress ──────────────────────────────────────────────────

public record StudentCourseProgressDto(
    Guid UserId,
    string FullName,
    string Email,
    int CompletedLessons,
    int TotalLessons,
    int CompletionPercent,
    int Stars,
    int XpEarned,
    int AverageScorePercent,
    DateTime? LastActivityAtUtc,
    DateTime? EnrolledAtUtc);

public record IllustrationDto(string Name, string Url);
