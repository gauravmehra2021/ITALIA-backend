using System.Text;
using System.Text.RegularExpressions;

namespace SSeuropa.Application.Services;

/// <summary>
/// How Punjabi appears in lessons. Same rules as ITALIA-backend/content/plainPunjabi.mjs and
/// translatable.mjs (keep them in step).
/// </summary>
public static partial class PunjabiText
{
    private const string Marks = "āīūṇṭḍṛṁṅñḷēōĀĪŪṆṬḌṚṀṄÑḶĒŌ";
    private const string Vowels = "aeiouāīūēō";

    [GeneratedRegex(@"[\p{L}\p{M}]+")]
    private static partial Regex Word();

    [GeneratedRegex(@"[aeiouāīūēō]+")]
    private static partial Regex VowelGroup();

    /// <summary>
    /// Roman Punjabi as Punjabi speakers type it: "Main Simran haan", not "Maiṁ Simran hāṁ".
    /// Only words with one of the scholarly marks change, so Italian (à, è…) and English never do.
    /// </summary>
    public static string? Plain(string? text) =>
        string.IsNullOrEmpty(text) || text.IndexOfAny(Marks.ToCharArray()) < 0 ? text : Word().Replace(text, m => PlainWord(m.Value));

    private static string PlainWord(string word)
    {
        if (word.IndexOfAny(Marks.ToCharArray()) < 0) return word;
        var upper = char.IsUpper(word[0]);
        var w = word.ToLowerInvariant();
        if (w == "ā") return upper ? "Aa" : "aa";

        // Nasal vowels.
        w = w.Replace("aiṁ", "ain").Replace("āṁ", "aan");
        if (w.EndsWith("īṁ", StringComparison.Ordinal)) w = w[..^2] + "i";
        w = w.Replace("īṁ", "in");
        if (w.EndsWith("ūṁ", StringComparison.Ordinal)) w = w[..^2] + "u";
        w = w.Replace("ūṁ", "un").Replace("ṁ", "n").Replace("ṅ", "n").Replace("ñ", "n");

        // Long vowels: ā is "aa" in the first syllable, ī / ū are "ee" / "oo" only in one-syllable
        // words; single at the end of the word or next to another vowel.
        static bool IsVowel(char c) => Vowels.Contains(c);
        var syllables = VowelGroup().Matches(w).Count;
        var sb = new StringBuilder(w.Length + 4);
        var syllable = 0;
        for (var i = 0; i < w.Length; i++)
        {
            var c = w[i];
            if (IsVowel(c) && (i == 0 || !IsVowel(w[i - 1]))) syllable++;
            var single = i + 1 >= w.Length || IsVowel(w[i + 1]) || (i > 0 && IsVowel(w[i - 1]));
            sb.Append(c switch
            {
                'ā' => !single && syllable == 1 ? "aa" : "a",
                'ī' => !single && syllables == 1 ? "ee" : "i",
                'ū' => !single && syllables == 1 ? "oo" : "u",
                _ => c.ToString(),
            });
        }
        var output = sb.ToString().Replace('ṛ', 'd').Replace('ṇ', 'n').Replace('ṭ', 't').Replace('ḍ', 'd').Replace('ḷ', 'l').Replace('ē', 'e').Replace('ō', 'o');
        return upper ? char.ToUpperInvariant(output[0]) + output[1..] : output;
    }

    [GeneratedRegex(@"^English\s*[·/]\s*Punjabi$")]
    private static partial Regex MixedHeader();

    /// <summary>"the biggest · sab ton vadda" → ("the biggest", "sab ton vadda"); Punjabi may be missing.</summary>
    public static (string English, string? Punjabi) SplitMeaning(string? cell)
    {
        cell ??= string.Empty;
        var i = cell.IndexOf(" · ", StringComparison.Ordinal);
        return i < 0 ? (cell, null) : (cell[..i], cell[(i + 3)..]);
    }

    /// <summary>
    /// Grammar tables with meanings: (English, Punjabi) of every row — from an "English" and a "Punjabi"
    /// column, or from one "English · Punjabi" column. Students see the Punjabi only in Punjabi; in Hindi
    /// and Urdu the translation of the English.
    /// </summary>
    public static IEnumerable<(string English, string? Punjabi)> TableMeanings(IReadOnlyList<List<string>> table)
    {
        if (table.Count == 0) yield break;
        var head = table[0];
        var en = head.IndexOf("English");
        var pa = head.IndexOf("Punjabi");
        var mixed = head.FindIndex(h => MixedHeader().IsMatch(h.Trim()));
        foreach (var row in table.Skip(1))
        {
            (string English, string? Punjabi) m =
                en >= 0 && pa >= 0 ? (Cell(row, en), Cell(row, pa))
                : mixed >= 0 ? SplitMeaning(Cell(row, mixed))
                : (string.Empty, null);
            if (!string.IsNullOrWhiteSpace(m.English)) yield return m;
        }
    }

    private static string Cell(List<string> row, int i) => i < row.Count ? row[i] : string.Empty;
}
