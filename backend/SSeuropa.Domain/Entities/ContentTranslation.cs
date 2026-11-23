using SSeuropa.Domain.Common;

namespace SSeuropa.Domain.Entities;

/// <summary>
/// Translation memory for lesson content: one English text (as shown in courses, units,
/// lessons and exercises) with its Punjabi, Hindi and Urdu translations. A text that appears in
/// many lessons is translated once. The student portal shows the translation in small
/// letters under the original; admins and teachers edit them on the Translations page.
/// </summary>
public class ContentTranslation : EntityBase
{
    /// <summary>The English text exactly as it appears in the content (trimmed).</summary>
    public string Source { get; set; } = string.Empty;

    /// <summary>Punjabi, written in Roman letters.</summary>
    public string? Punjabi { get; set; }

    /// <summary>Hindi, written in Devanagari.</summary>
    public string? Hindi { get; set; }

    /// <summary>Urdu, written in Urdu script (right-to-left).</summary>
    public string? Urdu { get; set; }

    public Guid? UpdatedByUserId { get; set; }
}
