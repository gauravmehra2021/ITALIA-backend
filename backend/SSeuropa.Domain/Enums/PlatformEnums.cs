namespace SSeuropa.Domain.Enums;

public enum UserRole
{
    User = 0,
    Mentor = 1,
    Admin = 2,
    SuperAdmin = 3,
    Staff = 4
}

public enum AccountStatus
{
    PendingVerification = 0,
    Active = 1,
    Suspended = 2,
    Disabled = 3
}

public enum CourseLevel
{
    Beginner = 0,
    Intermediate = 1,
    Advanced = 2
}

public enum CourseStatus
{
    Draft = 0,
    Published = 1,
    Archived = 2
}

public enum ClassStatus
{
    Scheduled = 0,
    Live = 1,
    Completed = 2,
    Cancelled = 3
}

public enum EnrollmentStatus
{
    Pending = 0,
    Active = 1,
    Suspended = 2,
    Completed = 3,
    Cancelled = 4
}

public enum MaterialType
{
    Pdf = 0,
    Video = 1,
    Audio = 2,
    Worksheet = 3,
    Other = 4
}

public enum SubscriptionStatus
{
    Pending = 0,
    Active = 1,
    PastDue = 2,
    Cancelled = 3,
    Suspended = 4
}

public enum PaymentStatus
{
    Pending = 0,
    Completed = 1,
    Failed = 2,
    Refunded = 3
}

public enum SupportTicketStatus
{
    Open = 0,
    InProgress = 1,
    Resolved = 2,
    Closed = 3
}

public enum NotificationType
{
    Class = 0,
    Payment = 1,
    Test = 2,
    Announcement = 3,
    Security = 4,
    System = 5
}

public enum PreferredLanguage
{
    English = 0,
    Italian = 1,
    Punjabi = 2,
    Hindi = 3,
    Urdu = 4
}

public enum EnquiryStatus
{
    New = 0,
    Contacted = 1,
    Qualified = 2,
    Enrolled = 3,
    Closed = 4,
    /// <summary>Payment received (in the client's own system). Saving this creates the student account.</summary>
    Paid = 5
}

// ── Curriculum ──────────────────────────────────────────────────────────────

public enum LessonStatus
{
    Draft = 0,
    Published = 1
}

public enum LessonKind
{
    Vocabulary = 0,
    Dialogue = 1,
    Grammar = 2,
    Review = 3,
    Video = 4,
    Exam = 5,
    /// <summary>Speaking practice: a role-play where the learner answers out loud.</summary>
    Conversation = 6
}

public enum LessonBlockType
{
    Text = 0,
    Vocabulary = 1,
    Dialogue = 2,
    Video = 3,
    Audio = 4,
    Image = 5,
    Pdf = 6,
    Tip = 7,
    Table = 8,
    /// <summary>Interactive role-play: lines marked IsLearner are the learner's turns.</summary>
    Roleplay = 9
}

public enum ExerciseType
{
    /// <summary>Hear an Italian word, pick the matching picture (or word).</summary>
    ListenPick = 0,
    /// <summary>See an Italian word/sentence, pick the English meaning.</summary>
    PickTranslation = 1,
    /// <summary>See a picture, pick the Italian word.</summary>
    PickWord = 2,
    /// <summary>Match Italian and English pairs.</summary>
    MatchPairs = 3,
    /// <summary>Choose the missing word in a sentence (multiple choice).</summary>
    FillBlank = 4,
    /// <summary>Put the words in the right order.</summary>
    WordOrder = 5,
    /// <summary>Read/listen and decide true or false.</summary>
    TrueFalse = 6
}
