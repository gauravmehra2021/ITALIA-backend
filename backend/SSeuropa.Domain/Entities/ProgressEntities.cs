using SSeuropa.Domain.Common;
using SSeuropa.Domain.Enums;

namespace SSeuropa.Domain.Entities;

public class AttendanceRecord : EntityBase
{
    public Guid ClassSessionId { get; set; }
    public Guid UserId { get; set; }
    public bool Present { get; set; }
    public DateTime? MarkedAtUtc { get; set; }
}

public class LearningProgress : EntityBase
{
    public Guid UserId { get; set; }
    public Guid CourseId { get; set; }
    public decimal CompletionPercent { get; set; }
    public int StreakDays { get; set; }
    public decimal GrammarScore { get; set; }
    public decimal VocabularyScore { get; set; }
    public decimal SpeakingScore { get; set; }
    public DateTime? LastActivityAtUtc { get; set; }
}

public class Subscription : EntityBase
{
    public Guid UserId { get; set; }
    public Guid CourseId { get; set; }
    public string PlanName { get; set; } = string.Empty;
    public SubscriptionStatus Status { get; set; } = SubscriptionStatus.Pending;
    public string? PayPalSubscriptionId { get; set; }
    public DateTime? StartsAtUtc { get; set; }
    public DateTime? EndsAtUtc { get; set; }
}

public class Payment : EntityBase
{
    public Guid UserId { get; set; }
    public Guid? SubscriptionId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "EUR";
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
    public string? PayPalReference { get; set; }
    public string Description { get; set; } = string.Empty;
}

public class Certificate : EntityBase
{
    public Guid UserId { get; set; }
    public Guid CourseId { get; set; }
    public string CertificateCode { get; set; } = string.Empty;
    public DateTime IssuedAtUtc { get; set; } = DateTime.UtcNow;
    public string QrVerificationToken { get; set; } = string.Empty;
}

public class Notification : EntityBase
{
    public Guid? UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public NotificationType Type { get; set; } = NotificationType.System;
    public bool IsRead { get; set; }
}

public class Announcement : EntityBase
{
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public Guid CreatedByUserId { get; set; }
    public bool IsPublished { get; set; } = true;
}

public class SupportTicket : EntityBase
{
    public Guid UserId { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public SupportTicketStatus Status { get; set; } = SupportTicketStatus.Open;
    public string? AdminResponse { get; set; }
}

public class AuditLog : EntityBase
{
    public Guid? ActorUserId { get; set; }
    public string Action { get; set; } = string.Empty;
    public string Resource { get; set; } = string.Empty;
    public string Details { get; set; } = string.Empty;
    public string? IpAddress { get; set; }
}

public class AiSession : EntityBase
{
    public Guid UserId { get; set; }
    public string Mode { get; set; } = "tutor";
    public string Prompt { get; set; } = string.Empty;
    public string Response { get; set; } = string.Empty;
    public int TokensUsed { get; set; }
}
