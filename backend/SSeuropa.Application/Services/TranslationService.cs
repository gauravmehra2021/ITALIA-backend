using System.Text.RegularExpressions;
using SSeuropa.Application.Common;
using SSeuropa.Application.Interfaces;
using SSeuropa.Domain.Entities;
using SSeuropa.Domain.Enums;
using SSeuropa.Domain.Exceptions;

namespace SSeuropa.Application.Services;

/// <summary>Student request: the English texts on screen, and the language wanted.</summary>
public record TranslateTextsRequest(string Language, List<string> Texts);

public record TranslationQuery(Guid? CourseId = null, string? Missing = null, string? Search = null, int Page = 1, int PageSize = 50);

public record TranslationItemDto(string Source, string? Punjabi, string? Hindi, string? Urdu, DateTime? UpdatedAtUtc);

public record TranslationPageDto(
    PagedResult<TranslationItemDto> Page,
    int TotalTexts,
    int TranslatedPunjabi,
    int TranslatedHindi,
    int TranslatedUrdu);

public record SaveTranslationRequest(string Source, string? Punjabi, string? Hindi, string? Urdu);

/// <summary>
/// Punjabi, Hindi and Urdu translations of lesson content (translation memory).
/// Students get translations for the texts on their screen; admins and teachers list the
/// texts of a course and fill or correct the translations.
/// </summary>
public class TranslationService(
    ITranslationRepository translations,
    ICourseRepository courses,
    ICurriculumRepository curriculum,
    ICurrentUserService current)
{
    private const int MaxTextsPerRequest = 3000;
    private const int MaxTextLength = 4000;

    private bool IsStaff => current.IsInRole(UserRole.Mentor, UserRole.Admin, UserRole.SuperAdmin, UserRole.Staff);

    // ── Student side ────────────────────────────────────────────────────────

    /// <summary>Returns { english text → translation } for the texts that have one in the language.</summary>
    public async Task<Dictionary<string, string>> TranslateAsync(TranslateTextsRequest request, CancellationToken ct = default)
    {
        if (current.UserId is null) throw new UnauthorizedAccessDomainException();
        var language = ParseLanguage(request.Language);
        if (language == PreferredLanguage.English) return [];

        var wanted = (request.Texts ?? [])
            .Where(t => !string.IsNullOrWhiteSpace(t) && t.Length <= MaxTextLength)
            .Select(t => t.Trim())
            .Distinct(StringComparer.Ordinal)
            .Take(MaxTextsPerRequest)
            .ToList();

        var found = await translations.GetBySourcesAsync(wanted, ct);
        var map = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (var t in found)
        {
            var text = language switch
            {
                PreferredLanguage.Punjabi => t.Punjabi,
                PreferredLanguage.Hindi => t.Hindi,
                _ => t.Urdu,
            };
            if (!string.IsNullOrWhiteSpace(text)) map[t.Source] = text;
        }
        return map;
    }

    // ── Admin side ──────────────────────────────────────────────────────────

    public async Task<TranslationPageDto> ListAsync(TranslationQuery query, CancellationToken ct = default)
    {
        RequireStaff();

        var sources = new List<string>();
        var seen = new HashSet<string>(StringComparer.Ordinal);
        var all = await courses.GetAllAsync(ct);
        var selected = all.Where(c => !c.IsDeleted && (query.CourseId is null || c.Id == query.CourseId))
            .OrderBy(c => c.DisplayOrder).ThenBy(c => c.Title).ToList();
        if (query.CourseId is not null && selected.Count == 0) throw new NotFoundException("Course", query.CourseId.Value);

        foreach (var course in selected)
        {
            var units = await curriculum.GetUnitsAsync(course.Id, ct);
            var lessons = await curriculum.GetLessonsByCourseAsync(course.Id, ct);
            foreach (var s in ContentTexts.Extract(course, units, lessons))
                if (seen.Add(s)) sources.Add(s);
        }

        var existing = (await translations.GetBySourcesAsync(sources, ct)).ToDictionary(t => t.Source, StringComparer.Ordinal);
        var rows = sources.Select(s => existing.TryGetValue(s, out var t)
            ? new TranslationItemDto(s, t.Punjabi, t.Hindi, t.Urdu, t.UpdatedAtUtc ?? t.CreatedAtUtc)
            : new TranslationItemDto(s, null, null, null, null)).ToList();

        var translatedPa = rows.Count(r => !string.IsNullOrWhiteSpace(r.Punjabi));
        var translatedHi = rows.Count(r => !string.IsNullOrWhiteSpace(r.Hindi));
        var translatedUr = rows.Count(r => !string.IsNullOrWhiteSpace(r.Urdu));

        IEnumerable<TranslationItemDto> filtered = rows;
        switch (query.Missing?.Trim().ToLowerInvariant())
        {
            case "punjabi": filtered = filtered.Where(r => string.IsNullOrWhiteSpace(r.Punjabi)); break;
            case "hindi": filtered = filtered.Where(r => string.IsNullOrWhiteSpace(r.Hindi)); break;
            case "urdu": filtered = filtered.Where(r => string.IsNullOrWhiteSpace(r.Urdu)); break;
            case "any": filtered = filtered.Where(r => string.IsNullOrWhiteSpace(r.Punjabi) || string.IsNullOrWhiteSpace(r.Hindi) || string.IsNullOrWhiteSpace(r.Urdu)); break;
        }
        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var q = query.Search.Trim();
            filtered = filtered.Where(r => r.Source.Contains(q, StringComparison.OrdinalIgnoreCase)
                || (r.Punjabi?.Contains(q, StringComparison.OrdinalIgnoreCase) ?? false)
                || (r.Hindi?.Contains(q, StringComparison.Ordinal) ?? false)
                || (r.Urdu?.Contains(q, StringComparison.OrdinalIgnoreCase) ?? false));
        }

        var list = filtered.ToList();
        var pageSize = Math.Clamp(query.PageSize, 1, 200);
        var page = Math.Max(query.Page, 1);
        return new TranslationPageDto(
            new PagedResult<TranslationItemDto>
            {
                Items = list.Skip((page - 1) * pageSize).Take(pageSize).ToList(),
                Page = page,
                PageSize = pageSize,
                TotalCount = list.Count,
            },
            rows.Count, translatedPa, translatedHi, translatedUr);
    }

    public async Task<TranslationItemDto> SaveAsync(SaveTranslationRequest request, CancellationToken ct = default)
    {
        RequireStaff();
        var source = request.Source?.Trim();
        if (string.IsNullOrEmpty(source)) throw new ValidationDomainException("The English text is required.");
        if (source.Length > MaxTextLength) throw new ValidationDomainException("The text is too long.");

        // A language left out of the request (null) keeps its current value, so an older
        // client that doesn't know a language can't erase it; an empty string removes it.
        static string? Clean(string? s) => string.IsNullOrWhiteSpace(s) ? null : s.Trim();
        var pa = Clean(request.Punjabi);
        var hi = Clean(request.Hindi);
        var ur = Clean(request.Urdu);
        if ((pa?.Length ?? 0) > MaxTextLength * 2 || (hi?.Length ?? 0) > MaxTextLength * 2 || (ur?.Length ?? 0) > MaxTextLength * 2)
            throw new ValidationDomainException("The translation is too long.");

        var item = await translations.GetBySourceAsync(source, ct);
        if (item is null)
        {
            item = new ContentTranslation { Source = source, Punjabi = pa, Hindi = hi, Urdu = ur, UpdatedByUserId = current.UserId, UpdatedAtUtc = DateTime.UtcNow };
            await translations.AddRangeAsync([item], ct);
        }
        else
        {
            if (request.Punjabi is not null) item.Punjabi = pa;
            if (request.Hindi is not null) item.Hindi = hi;
            if (request.Urdu is not null) item.Urdu = ur;
            item.UpdatedByUserId = current.UserId;
            await translations.UpdateAsync(item, ct);
        }
        return new TranslationItemDto(item.Source, item.Punjabi, item.Hindi, item.Urdu, item.UpdatedAtUtc);
    }

    private void RequireStaff()
    {
        if (current.UserId is null) throw new UnauthorizedAccessDomainException();
        if (!IsStaff) throw new UnauthorizedAccessDomainException("Only teachers and admins can manage translations.");
    }

    private static PreferredLanguage ParseLanguage(string? value)
    {
        if (Enum.TryParse<PreferredLanguage>(value?.Trim(), true, out var lang) && Enum.IsDefined(lang)
            && lang is PreferredLanguage.English or PreferredLanguage.Punjabi or PreferredLanguage.Hindi or PreferredLanguage.Urdu)
            return lang;
        var v = value?.Trim().ToLowerInvariant();
        return v switch
        {
            "pa" => PreferredLanguage.Punjabi,
            "hi" => PreferredLanguage.Hindi,
            "ur" => PreferredLanguage.Urdu,
            "en" => PreferredLanguage.English,
            _ => throw new ValidationDomainException("Choose English, Punjabi, Hindi or Urdu."),
        };
    }
}

/// <summary>
/// The English texts of a course that students see and that can be translated.
/// Same rules as ITALIA-backend/content/translatable.mjs (keep them in step):
/// Italian text, standard exercise instructions and Italian table cells are left out.
/// </summary>
public static class ContentTexts
{
    private static readonly Regex[] Standard =
    [
        new(@"^(\d+[A-Z]? · )?(Listen and choose the right picture|Listen and choose the word you hear|Listen: which word do you hear\?|Listen and choose the number|Listen and choose the sentence you hear|Listen and choose what you hear|Listen and choose the time|Listen: what time is it\?|Listen and choose the price or weight|Listen: how much is it\?|What is this in Italian\?|What does it mean\?|Match the Italian words with their meaning|Match each Italian word with its meaning|Match each word with its meaning|Match each expression with its meaning|Choose the missing word|True or false\?|Choose the correct sentence:?|Choose the correct form:)$", RegexOptions.Compiled),
        new(@"^(New words|Key phrases|Useful phrases|Examples|The situation|Good to know|What you can do now|Unit study sheet \(PDF\)|Course phrasebook \(PDF\))$", RegexOptions.Compiled),
        new(@"^All the words, dialogues and grammar of this unit on one printable sheet\.$", RegexOptions.Compiled),
        new(@"^The slides your teacher uses in class\. Open them on your phone or print them\.$", RegexOptions.Compiled),
        new(@"^Class slides: .+ \(PDF\)$", RegexOptions.Compiled),
    ];

    private static readonly Regex TrueFalseInner = new(@"^(?:\d+[A-Z]? · )?True or false\? \((.+)\)$", RegexOptions.Compiled | RegexOptions.Singleline);
    private static readonly Regex WordOrderInner = new(@"^(?:\d+[A-Z]? · )?Put the words in order: ""(.+)""$", RegexOptions.Compiled | RegexOptions.Singleline);
    private static readonly Regex HasLetter = new("[A-Za-z]", RegexOptions.Compiled);
    private static readonly Regex Paragraphs = new(@"\n{2,}", RegexOptions.Compiled);

    public static List<string> Extract(Course course, IEnumerable<CourseUnit> units, IEnumerable<Lesson> lessons)
    {
        var output = new List<string>();
        var seen = new HashSet<string>(StringComparer.Ordinal);
        void Add(string? s)
        {
            if (s is null) return;
            var t = s.Trim();
            if (t.Length == 0 || !HasLetter.IsMatch(t) || Standard.Any(r => r.IsMatch(t))) return;
            if (seen.Add(t)) output.Add(t);
        }

        Add(course.Title);
        Add(course.ShortDescription);
        foreach (var p in Paragraphs.Split(course.Description ?? string.Empty)) Add(p);
        foreach (var o in (course.Objectives ?? string.Empty).Split('\n')) Add(o);

        var byUnit = lessons.Where(l => !l.IsDeleted).GroupBy(l => l.UnitId).ToDictionary(g => g.Key, g => g.OrderBy(l => l.Order).ToList());
        foreach (var u in units.Where(u => !u.IsDeleted).OrderBy(u => u.Order))
        {
            Add(u.Title);
            Add(u.Description);
            Add(u.LiveClassFocus);
            foreach (var c in u.CanDo) Add(c);
            if (!byUnit.TryGetValue(u.Id, out var unitLessons)) continue;
            foreach (var l in unitLessons)
            {
                Add(l.Title);
                Add(l.Summary);
                Add(l.Homework);
                foreach (var b in l.Blocks)
                {
                    Add(b.Title);
                    if (b.Type is LessonBlockType.Text or LessonBlockType.Tip or LessonBlockType.Pdf) Add(b.Body);
                    if (b.Type == LessonBlockType.Text) Add(b.Caption);
                    if (b.Type == LessonBlockType.Table && b.Table.Count > 0) foreach (var h in b.Table[0]) Add(h);
                    // …and the English cells of tables that carry a Punjabi meaning (shown in Hindi / Urdu).
                    if (b.Type == LessonBlockType.Table) foreach (var (en, _) in PunjabiText.TableMeanings(b.Table)) Add(en);
                    foreach (var it in b.Items) { Add(it.English); Add(it.ExampleEnglish); }
                    foreach (var ln in b.Lines) { Add(ln.English); foreach (var r in ln.Replies) Add(r.English); }
                }
                foreach (var e in l.Exercises)
                {
                    var inner = TrueFalseInner.Match(e.Prompt ?? string.Empty);
                    if (!inner.Success) inner = WordOrderInner.Match(e.Prompt ?? string.Empty);
                    Add(inner.Success ? inner.Groups[1].Value : e.Prompt);
                    if (e.Type == ExerciseType.PickTranslation && (e.Prompt ?? string.Empty).EndsWith("What does it mean?", StringComparison.Ordinal))
                        foreach (var o in e.Options) Add(o.Text);
                    if (e.Type == ExerciseType.MatchPairs)
                        foreach (var p in e.Pairs) Add(PunjabiText.SplitMeaning(p.Right).English);
                }
            }
        }
        return output;
    }
}
