using SSeuropa.Domain.Common;

namespace SSeuropa.Domain.Entities;

public class Assessment : EntityBase
{
    public Guid CourseId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int PassingScore { get; set; } = 60;
    public int TimeLimitMinutes { get; set; } = 30;
    public List<AssessmentQuestion> Questions { get; set; } = [];
}

public class AssessmentQuestion
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Prompt { get; set; } = string.Empty;
    public List<string> Options { get; set; } = [];
    public int CorrectOptionIndex { get; set; }
    public int Points { get; set; } = 1;
}

public class TestAttempt : EntityBase
{
    public Guid AssessmentId { get; set; }
    public Guid UserId { get; set; }
    public DateTime StartedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAtUtc { get; set; }
    public int Score { get; set; }
    public int MaxScore { get; set; }
    public bool Passed { get; set; }
    public List<int> SelectedAnswers { get; set; } = [];
}
