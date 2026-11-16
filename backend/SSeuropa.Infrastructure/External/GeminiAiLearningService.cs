using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SSeuropa.Application.Interfaces;

namespace SSeuropa.Infrastructure.External;

public class GeminiAiLearningService : IAiLearningService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<GeminiAiLearningService> _logger;
    private readonly string _apiKey;
    private readonly string _model;

    public GeminiAiLearningService(
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<GeminiAiLearningService> logger)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;
        _apiKey = _configuration["Gemini:ApiKey"]?.Trim()
            ?? _configuration["GEMINI_API_KEY"]?.Trim()
            ?? Environment.GetEnvironmentVariable("GEMINI_API_KEY")?.Trim()
            ?? string.Empty;
        _model = _configuration["Gemini:Model"]?.Trim()
            ?? _configuration["GEMINI_MODEL"]?.Trim()
            ?? Environment.GetEnvironmentVariable("GEMINI_MODEL")?.Trim()
            ?? "gemini-flash-latest";
    }

    public async Task<string> GenerateResponseAsync(
        string mode,
        string prompt,
        string? sourceLanguage = null,
        string? targetLanguage = null,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(_apiKey))
        {
            _logger.LogWarning("Gemini API key is not configured. Falling back to educational default.");
            return GetFallbackResponse(mode, prompt, sourceLanguage, targetLanguage);
        }

        try
        {
            var systemInstruction = 
                "You are the official conversational Italian tutor and learning assistant for SS Europa (Language Academy in Italy). " +
                "Help the student learn Italian effectively, explain grammar rules, vocabulary, pronunciation, and cultural context. " +
                "Respond in a warm, polite, and encouraging tone. When speaking Italian, provide English explanations where helpful. " +
                "Keep responses concise and well-formatted.";

            var requestBody = new
            {
                system_instruction = new
                {
                    parts = new[]
                    {
                        new { text = systemInstruction }
                    }
                },
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new { text = $"[Mode: {mode}] [Source: {sourceLanguage ?? "English"}, Target: {targetLanguage ?? "Italian"}]\nStudent prompt: {prompt}" }
                        }
                    }
                },
                generationConfig = new
                {
                    temperature = 0.7,
                    maxOutputTokens = 1024
                }
            };

            var url = $"https://generativelanguage.googleapis.com/v1beta/models/{_model}:generateContent?key={_apiKey}";
            using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            timeoutCts.CancelAfter(TimeSpan.FromSeconds(6));

            var response = await _httpClient.PostAsJsonAsync(url, requestBody, timeoutCts.Token);

            if (!response.IsSuccessStatusCode)
            {
                var errorText = await response.Content.ReadAsStringAsync(timeoutCts.Token);
                _logger.LogWarning("Gemini API error (Status {StatusCode}): {Error}. Attempting fallback model gemini-3.8-flash.", response.StatusCode, errorText);

                // If flash-latest model failed, attempt gemini-3.8-flash
                if (_model != "gemini-3.8-flash")
                {
                    var fallbackUrl = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-3.8-flash:generateContent?key={_apiKey}";
                    var fallbackResponse = await _httpClient.PostAsJsonAsync(fallbackUrl, requestBody, timeoutCts.Token);
                    if (fallbackResponse.IsSuccessStatusCode)
                    {
                        var fallbackResult = await fallbackResponse.Content.ReadFromJsonAsync<GeminiGenerateContentResponse>(timeoutCts.Token);
                        var reply = fallbackResult?.Candidates?.FirstOrDefault()?.Content?.Parts?.FirstOrDefault()?.Text;
                        if (!string.IsNullOrWhiteSpace(reply))
                        {
                            return reply.Trim();
                        }
                    }
                }

                return GetFallbackResponse(mode, prompt, sourceLanguage, targetLanguage);
            }

            var result = await response.Content.ReadFromJsonAsync<GeminiGenerateContentResponse>(timeoutCts.Token);
            var content = result?.Candidates?.FirstOrDefault()?.Content?.Parts?.FirstOrDefault()?.Text;

            if (!string.IsNullOrWhiteSpace(content))
            {
                return content.Trim();
            }

            return GetFallbackResponse(mode, prompt, sourceLanguage, targetLanguage);
        }
        catch (Exception ex)
        {
            _logger.LogWarning("Gemini API call bypassed or timed out: {Message}. Returning educational tutor response.", ex.Message);
            return GetFallbackResponse(mode, prompt, sourceLanguage, targetLanguage);
        }
    }

    private static string GetFallbackResponse(string mode, string prompt, string? sourceLanguage, string? targetLanguage)
    {
        return mode.ToLowerInvariant() switch
        {
            "tutor" => $"Ciao! In Italian language learning for \"{prompt}\": Remember that nouns ending in -o are typically masculine (il ragazzo), and nouns ending in -a are feminine (la ragazza). Would you like to practice a sentence?",
            "translator" => $"{sourceLanguage ?? "English"} → {targetLanguage ?? "Italian"}: \"{prompt}\" translates with natural spoken Italian idioms depending on formal (Lei) or informal (tu) context.",
            "conversation" => $"Ottimo! Let's talk about \"{prompt}\". Buongiorno! Come ti chiami e da dove vieni? (Reply in Italian and I will help you with verb conjugations!)",
            "speaking" => $"Speaking Practice: Try pronouncing \"{prompt}\" with clear vowels and rolling the 'r' gently against your palate.",
            _ => $"Ciao! Regarding \"{prompt}\": How can I help you improve your Italian skills today?"
        };
    }

    private sealed class GeminiGenerateContentResponse
    {
        [JsonPropertyName("candidates")]
        public List<Candidate>? Candidates { get; set; }
    }

    private sealed class Candidate
    {
        [JsonPropertyName("content")]
        public Content? Content { get; set; }
    }

    private sealed class Content
    {
        [JsonPropertyName("parts")]
        public List<Part>? Parts { get; set; }
    }

    private sealed class Part
    {
        [JsonPropertyName("text")]
        public string? Text { get; set; }
    }
}

