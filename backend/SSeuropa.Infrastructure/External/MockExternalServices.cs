using SSeuropa.Application.Interfaces;

namespace SSeuropa.Infrastructure.External;

/// <summary>
/// Mock Azure Blob Storage adapter. Returns deterministic short-lived mock SAS-like URLs.
/// </summary>
public class MockStorageService : IStorageService
{
    public Task<string> GetProtectedAccessUrlAsync(string storageKey, TimeSpan ttl, CancellationToken ct = default)
    {
        var expiry = DateTimeOffset.UtcNow.Add(ttl).ToUnixTimeSeconds();
        var url = $"https://sseuropa.mock.blob.core.windows.net/{storageKey}?sv=mock&se={expiry}&sig=demo";
        return Task.FromResult(url);
    }
}

/// <summary>
/// Mock PayPal gateway. Always succeeds for prototype flows without live credentials.
/// </summary>
public class MockPaymentGateway : IPaymentGateway
{
    public Task<(string Reference, bool Success)> CreateSubscriptionAsync(
        Guid userId, Guid courseId, string planName, decimal amount, CancellationToken ct = default)
    {
        var reference = $"MOCK-PAYPAL-{Guid.NewGuid().ToString()[..8].ToUpperInvariant()}";
        return Task.FromResult((reference, true));
    }

    public Task<bool> VerifyPaymentAsync(string reference, CancellationToken ct = default) =>
        Task.FromResult(reference.StartsWith("MOCK-", StringComparison.OrdinalIgnoreCase));
}

/// <summary>
/// Mock AI learning service. Returns deterministic educational responses without LLM calls.
/// </summary>
public class MockAiLearningService : IAiLearningService
{
    public Task<string> GenerateResponseAsync(
        string mode, string prompt, string? sourceLanguage, string? targetLanguage, CancellationToken ct = default)
    {
        var response = mode.ToLowerInvariant() switch
        {
            "tutor" => $"AI Tutor (mock): For \"{prompt}\", remember that Italian nouns have gender. Example: il libro (m.), la casa (f.). Try building a sentence with both.",
            "translator" => $"AI Translator (mock): {sourceLanguage ?? "auto"} → {targetLanguage ?? "Italian"}. Suggested rendering of \"{prompt}\": «{prompt}» with context notes on register (formal/informal).",
            "conversation" => $"Conversation Practice (mock): Let's chat about \"{prompt}\". Mentor: Buongiorno! Come stai oggi? Reply in Italian and I'll give feedback on articles and verb tense.",
            "speaking" => $"Speaking Coach (mock): Focus on rolling the double consonant in \"{prompt}\". Tip: keep the tongue tip near the alveolar ridge and hold the sound slightly longer.",
            "practice" => $"Personalized Practice (mock): Based on your prompt \"{prompt}\", complete: 1) Fill the blank: Io ____ (essere) studente. 2) Translate: 'Where is the station?' 3) Say aloud three food vocabulary words.",
            _ => $"AI (mock): Received mode '{mode}' with prompt '{prompt}'."
        };
        return Task.FromResult(response);
    }
}
