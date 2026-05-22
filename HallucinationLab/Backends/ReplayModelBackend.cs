using System.Text.Json;

namespace HallucinationLab.Backends;

public sealed class ReplayModelBackend : ITextModelBackend
{
    private readonly Dictionary<string, string> responseMap;

    public ReplayModelBackend(string responseMapPath)
    {
        var json = File.ReadAllText(responseMapPath);
        responseMap = JsonSerializer.Deserialize<Dictionary<string, string>>(json)
            ?? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
    }

    public string Name => "ReplayModelBackend";

    public Task<string> GenerateAsync(string prompt, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (responseMap.TryGetValue(prompt, out var output))
        {
            return Task.FromResult(output);
        }

        return Task.FromResult("[no replay output configured for this prompt]");
    }
}
