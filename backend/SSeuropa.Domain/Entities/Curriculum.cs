using SSeuropa.Domain.Common;
using SSeuropa.Domain.Enums;

namespace SSeuropa.Domain.Entities;

// ─────────────────────────────────────────────────────────────────────────────
// Curriculum model (blended learning):
//
//   Course (CEFR level or track)
//     └─ Unit  = one real-life topic ≈ one week, with "can-do" goals,
//               a printable study sheet (PDF) and a live speaking class
//          └─ Lesson = 5–15 min self-study, one goal
//               ├─ Blocks    : what the student studies (words, dialogue,
//               │              grammar notes, video, audio, image, PDF, tips)
//               └─ Exercises : what the student practises (listen & pick,
//                              translate, match, fill the gap, word order…)
//
// Students earn 0–3 stars and XP per lesson (LessonProgress). Course
// completion is derived from completed lessons.
// ─────────────────────────────────────────────────────────────────────────────

public class CourseUnit : EntityBase
{
    public Guid CourseId { get; set; }
    public int Order { get; set; }
    public string Title { get; set; } = string.Empty;
    /// <summary>Italian title shown under the English one, e.g. "Ciao! Mi chiamo…".</summary>
    public string TitleIt { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    /// <summary>CEFR-style learning outcomes: "I can introduce myself…".</summary>
    public List<string> CanDo { get; set; } = [];
    /// <summary>Cover illustration (URL).</summary>
    public string? ImageUrl { get; set; }
    /// <summary>Printable study sheet for the unit.</summary>
    public string? PdfUrl { get; set; }
    /// <summary>What the weekly live speaking class practises.</summary>
    public string? LiveClassFocus { get; set; }
    public LessonStatus Status { get; set; } = LessonStatus.Draft;
}

public class Lesson : EntityBase
{
    public Guid CourseId { get; set; }
    public Guid UnitId { get; set; }
    public int Order { get; set; }
    public string Title { get; set; } = string.Empty;
    public string TitleIt { get; set; } = string.Empty;
    public LessonKind Kind { get; set; } = LessonKind.Vocabulary;
    public string Summary { get; set; } = string.Empty;
    public int EstimatedMinutes { get; set; } = 10;
    public int XpReward { get; set; } = 20;
    public LessonStatus Status { get; set; } = LessonStatus.Draft;
    /// <summary>Free preview lessons can be opened before enrolling.</summary>
    public bool IsFreePreview { get; set; }
    public List<LessonBlock> Blocks { get; set; } = [];
    public List<LessonExercise> Exercises { get; set; } = [];
    /// <summary>Speaking/writing task to prepare for the live class.</summary>
    public string? Homework { get; set; }
}

public class LessonBlock
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public LessonBlockType Type { get; set; } = LessonBlockType.Text;
    public string? Title { get; set; }
    /// <summary>Explanation text. Supports **bold**, line breaks and "- " bullet lists.</summary>
    public string? Body { get; set; }
    /// <summary>Video / audio / image / PDF URL (uploaded file or external link such as YouTube).</summary>
    public string? MediaUrl { get; set; }
    public string? Caption { get; set; }
    public List<VocabularyItem> Items { get; set; } = [];
    public List<DialogueLine> Lines { get; set; } = [];
    /// <summary>Grammar tables: first row is the header.</summary>
    public List<List<string>> Table { get; set; } = [];
}

public class VocabularyItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Italian { get; set; } = string.Empty;
    public string English { get; set; } = string.Empty;
    /// <summary>Simple pronunciation guide, e.g. "CHOW".</summary>
    public string? Pronunciation { get; set; }
    public string? ImageUrl { get; set; }
    /// <summary>Recorded audio. When empty, apps use the device's Italian voice.</summary>
    public string? AudioUrl { get; set; }
    public string? Example { get; set; }
    public string? ExampleEnglish { get; set; }
}

public class DialogueLine
{
    public string Speaker { get; set; } = string.Empty;
    public string Italian { get; set; } = string.Empty;
    public string English { get; set; } = string.Empty;
    public string? AudioUrl { get; set; }

    /// <summary>Role-play only: this is the learner's turn (they say Italian or one of the Replies).</summary>
    public bool IsLearner { get; set; }

    /// <summary>Role-play only: other good answers the learner may choose for this turn.</summary>
    public List<DialogueReply> Replies { get; set; } = [];
}

public class DialogueReply
{
    public string Italian { get; set; } = string.Empty;
    public string English { get; set; } = string.Empty;
}

public class LessonExercise
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public ExerciseType Type { get; set; } = ExerciseType.PickTranslation;
    /// <summary>Instruction / question shown to the student.</summary>
    public string Prompt { get; set; } = string.Empty;
    /// <summary>Italian text spoken aloud (listening exercises) or shown as the item to translate.</summary>
    public string? Italian { get; set; }
    public string? ImageUrl { get; set; }
    public List<ExerciseOption> Options { get; set; } = [];
    public int CorrectIndex { get; set; }
    /// <summary>Accepted answer for FillBlank / WordOrder (WordOrder words are separated by spaces).</summary>
    public string? Answer { get; set; }
    /// <summary>MatchPairs: Left = Italian, Right = English.</summary>
    public List<MatchPair> Pairs { get; set; } = [];
    /// <summary>Shown after answering, e.g. a grammar hint.</summary>
    public string? Explanation { get; set; }
}

public class ExerciseOption
{
    public string Text { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
}

public class MatchPair
{
    public string Left { get; set; } = string.Empty;
    public string Right { get; set; } = string.Empty;
}

public class LessonProgress : EntityBase
{
    public Guid UserId { get; set; }
    public Guid CourseId { get; set; }
    public Guid UnitId { get; set; }
    public Guid LessonId { get; set; }
    public bool Completed { get; set; }
    public int BestScorePercent { get; set; }
    public int Stars { get; set; }
    public int Attempts { get; set; }
    public int XpEarned { get; set; }
    public DateTime? CompletedAtUtc { get; set; }
    public DateTime LastAttemptAtUtc { get; set; } = DateTime.UtcNow;
}
