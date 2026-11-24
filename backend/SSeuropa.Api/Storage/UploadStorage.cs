namespace SSeuropa.Api.Storage;

/// <summary>
/// Where uploaded files (lesson videos, images, PDFs) live and how they are linked.
///
/// - <c>Storage:UploadsPath</c>: folder for uploads. Default: "uploads" inside the app folder.
///   On a live server set it to a folder outside the deploy folder (e.g. /var/sseuropa/uploads),
///   so a new deploy never deletes the files teachers uploaded.
/// - <c>Storage:PublicBaseUrl</c>: public address of the API (e.g. https://api.sseuropa.com),
///   used to build file links. Without it the link is built from the request, which behind
///   a reverse proxy can come out as http:// or localhost.
/// </summary>
public static class UploadStorage
{
    public const long MaxUploadBytes = 500L * 1024 * 1024; // 500 MB (lesson videos)

    public static string Root(IConfiguration config, IWebHostEnvironment env)
    {
        var configured = config["Storage:UploadsPath"];
        var root = string.IsNullOrWhiteSpace(configured)
            ? Path.Combine(env.ContentRootPath, "uploads")
            : Path.GetFullPath(configured);
        Directory.CreateDirectory(root);
        return root;
    }

    /// <summary>Full path for a relative key, or null if the key would leave the uploads folder.</summary>
    public static string? SafePath(string root, string? key)
    {
        if (string.IsNullOrWhiteSpace(key)) return null;
        var rootFull = Path.GetFullPath(root).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar) + Path.DirectorySeparatorChar;
        var full = Path.GetFullPath(Path.Combine(rootFull, key.Replace('\\', '/').TrimStart('/')));
        return full.StartsWith(rootFull, StringComparison.OrdinalIgnoreCase) ? full : null;
    }

    public static string PublicUrl(IConfiguration config, HttpRequest request, string relativeUrl)
    {
        var baseUrl = config["Storage:PublicBaseUrl"];
        if (string.IsNullOrWhiteSpace(baseUrl)) baseUrl = $"{request.Scheme}://{request.Host}";
        return baseUrl.TrimEnd('/') + "/" + relativeUrl.TrimStart('/');
    }
}
