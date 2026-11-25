using System.Security.Cryptography;
using System.Text;
using SSeuropa.Application.Interfaces;
using SSeuropa.Domain.Enums;

namespace SSeuropa.Api.Storage;

/// <summary>
/// The syllabus PDFs (study sheets, class slides, phrasebook, PDFs added to lessons) are not
/// public files: they can't be downloaded from their address. A signed-in student who may see the
/// course gets a link that works for a few minutes, and the student app shows the pages inside the
/// app (with the student's name as a watermark, without download or print buttons).
/// Other uploads (assignment files, images, audio, video) are not affected.
/// </summary>
public sealed class ProtectedDocuments(IServiceScopeFactory scopes, IConfiguration config)
{
    public static readonly TimeSpan LinkLifetime = TimeSpan.FromMinutes(10);
    private static readonly TimeSpan IndexLifetime = TimeSpan.FromSeconds(60);

    private readonly SemaphoreSlim _gate = new(1, 1);
    private Dictionary<string, HashSet<Guid>> _index = new(StringComparer.OrdinalIgnoreCase);
    private DateTime _builtAtUtc = DateTime.MinValue;

    private static bool IsPdf(string path) => path.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase);

    /// <summary>Normalises "/content/pdf/x.pdf", "content/pdf/x.pdf" or "https://api…/content/pdf/x.pdf".</summary>
    public static string? Normalize(string? url)
    {
        if (string.IsNullOrWhiteSpace(url)) return null;
        var path = Uri.TryCreate(url, UriKind.Absolute, out var abs) && abs.Scheme.StartsWith("http") ? abs.AbsolutePath : url.Split('?', '#')[0];
        path = "/" + Uri.UnescapeDataString(path).TrimStart('/');
        return path.Contains("..", StringComparison.Ordinal) ? null : path;
    }

    /// <summary>True when the file may not be served by its plain address.</summary>
    public async Task<bool> IsProtectedAsync(string path, CancellationToken ct = default)
    {
        if (!IsPdf(path)) return false;
        if (path.StartsWith("/content/", StringComparison.OrdinalIgnoreCase)) return true;
        return path.StartsWith("/uploads/", StringComparison.OrdinalIgnoreCase) && (await IndexAsync(ct)).ContainsKey(path);
    }

    /// <summary>The courses whose lessons or units use this PDF.</summary>
    public async Task<IReadOnlyCollection<Guid>> CoursesUsingAsync(string path, CancellationToken ct = default) =>
        (await IndexAsync(ct)).TryGetValue(path, out var ids) ? ids : [];

    public void Invalidate() => _builtAtUtc = DateTime.MinValue;

    private async Task<Dictionary<string, HashSet<Guid>>> IndexAsync(CancellationToken ct)
    {
        if (DateTime.UtcNow - _builtAtUtc < IndexLifetime) return _index;
        await _gate.WaitAsync(ct);
        try
        {
            if (DateTime.UtcNow - _builtAtUtc < IndexLifetime) return _index;
            using var scope = scopes.CreateScope();
            var courses = scope.ServiceProvider.GetRequiredService<ICourseRepository>();
            var curriculum = scope.ServiceProvider.GetRequiredService<ICurriculumRepository>();
            var index = new Dictionary<string, HashSet<Guid>>(StringComparer.OrdinalIgnoreCase);
            void Add(string? url, Guid courseId)
            {
                var path = Normalize(url);
                if (path is null || !IsPdf(path)) return;
                if (!index.TryGetValue(path, out var set)) index[path] = set = [];
                set.Add(courseId);
            }
            foreach (var course in (await courses.GetAllAsync(ct)).Where(c => !c.IsDeleted))
            {
                foreach (var u in await curriculum.GetUnitsAsync(course.Id, ct)) Add(u.PdfUrl, course.Id);
                foreach (var l in await curriculum.GetLessonsByCourseAsync(course.Id, ct))
                    foreach (var b in l.Blocks.Where(b => b.Type == LessonBlockType.Pdf)) Add(b.MediaUrl, course.Id);
            }
            _index = index;
            _builtAtUtc = DateTime.UtcNow;
            return index;
        }
        finally
        {
            _gate.Release();
        }
    }

    // ── Signed links ─────────────────────────────────────────────────────────

    private byte[] Key => SHA256.HashData(Encoding.UTF8.GetBytes("protected-documents:" + (config["Jwt:Key"] ?? "sseuropa")));

    private string Sign(string path, long expires) =>
        Base64Url(HMACSHA256.HashData(Key, Encoding.UTF8.GetBytes($"{path}|{expires}")));

    private static string Base64Url(byte[] bytes) => Convert.ToBase64String(bytes).TrimEnd('=').Replace('+', '-').Replace('/', '_');

    /// <summary>A relative link to the file that works for <see cref="LinkLifetime"/>.</summary>
    public string SignedLink(string path)
    {
        var expires = DateTimeOffset.UtcNow.Add(LinkLifetime).ToUnixTimeSeconds();
        return $"/api/documents/view?p={Uri.EscapeDataString(path)}&e={expires}&s={Sign(path, expires)}";
    }

    public bool IsValid(string path, long expires, string signature) =>
        expires >= DateTimeOffset.UtcNow.ToUnixTimeSeconds()
        && CryptographicOperations.FixedTimeEquals(Encoding.ASCII.GetBytes(Sign(path, expires)), Encoding.ASCII.GetBytes(signature ?? string.Empty));

    /// <summary>The file on disk for a protected path, or null.</summary>
    public string? FileFor(string path, IWebHostEnvironment env)
    {
        string? full = null;
        if (path.StartsWith("/content/", StringComparison.OrdinalIgnoreCase))
        {
            var root = Path.GetFullPath(Path.Combine(env.WebRootPath ?? Path.Combine(env.ContentRootPath, "wwwroot"), "content"));
            var candidate = Path.GetFullPath(Path.Combine(root, path["/content/".Length..]));
            if (candidate.StartsWith(root + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase)) full = candidate;
        }
        else if (path.StartsWith("/uploads/", StringComparison.OrdinalIgnoreCase))
        {
            full = UploadStorage.SafePath(UploadStorage.Root(config, env), path["/uploads/".Length..]);
        }
        return full is not null && File.Exists(full) ? full : null;
    }
}
