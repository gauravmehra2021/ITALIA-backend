using SSeuropa.Domain.Common;

namespace SSeuropa.Domain.Entities;

public class Assignment : EntityBase
{
    public Guid CourseId { get; set; }
    public string ChapterTitle { get; set; } = "Chapter 1: Practical Assignments";
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? ReferenceFileUrl { get; set; }
    public DateTime? DueDateUtc { get; set; }
    public int TotalMarks { get; set; } = 100;
    public bool IsActive { get; set; } = true;
}

public class AssignmentSubmission : EntityBase
{
    public Guid AssignmentId { get; set; }
    public Guid StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string StudentEmail { get; set; } = string.Empty;
    public string? SubmissionText { get; set; }
    public string? SubmissionFileUrl { get; set; }
    public DateTime SubmittedAtUtc { get; set; } = DateTime.UtcNow;
    public int? MarksObtained { get; set; }
    public string? TeacherFeedback { get; set; }
    public DateTime? GradedAtUtc { get; set; }
    public Guid? GradedByMentorId { get; set; }
    public string Status { get; set; } = "Submitted"; // Submitted, Graded
}

