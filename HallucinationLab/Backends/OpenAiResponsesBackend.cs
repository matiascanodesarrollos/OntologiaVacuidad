using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace HallucinationLab.Backends;

public sealed class OpenAiResponsesBackend : ITextModelBackend
{
    private readonly HttpClient httpClient;
    private readonly string endpoint;
    private readonly string model;

    public OpenAiResponsesBackend(string apiKey, string model, string baseUrl = "https://api.openai.com")
    {
        httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

        this.model = model;
        endpoint = $"{baseUrl.TrimEnd('/')}/v1/responses";
    }

    public string Name => $"OpenAI:{model}";

    public async Task<string> GenerateAsync(string prompt, CancellationToken cancellationToken)
    {
        var payload = new
        {
            model,
            input = prompt
        };

        var json = JsonSerializer.Serialize(payload);
        using var content = new StringContent(json, Encoding.UTF8, "application/json");
        using var response = await httpClient.PostAsync(endpoint, content, cancellationToken);
        var body = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException($"OpenAI call failed ({(int)response.StatusCode}): {body}");
        }

        return ExtractText(body);
    }

    private static string ExtractText(string body)
    {
        using var doc = JsonDocument.Parse(body);
        var root = doc.RootElement;

        if (root.TryGetProperty("output_text", out var outputText) && outputText.ValueKind == JsonValueKind.String)
        {
            return outputText.GetString() ?? string.Empty;
        }

        if (root.TryGetProperty("output", out var output) && output.ValueKind == JsonValueKind.Array)
        {
            var parts = new List<string>();
            foreach (var item in output.EnumerateArray())
            {
                if (!item.TryGetProperty("content", out var content) || content.ValueKind != JsonValueKind.Array)
                {
                    continue;
                }

                foreach (var chunk in content.EnumerateArray())
                {
                    if (chunk.TryGetProperty("text", out var text) && text.ValueKind == JsonValueKind.String)
                    {
                        parts.Add(text.GetString() ?? string.Empty);
                    }
                }
            }

            if (parts.Count > 0)
            {
                return string.Join("\n", parts).Trim();
            }
        }

        return body;
    }
}
