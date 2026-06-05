namespace Models.Tests;

public sealed class TruthReferenceScore
{
    public int TruthAnchorsFound { get; init; }
    public int MissingTruthAnchors { get; init; }
}

internal static class TruthReferenceEvaluator
{
    internal static TruthReferenceScore Evaluate(string output, string truth)
    {
        var anchors = truth
            .Split(':', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Where(anchor => !string.IsNullOrWhiteSpace(anchor))
            .ToArray();

        var found = anchors.Count(anchor => output.Contains(anchor, StringComparison.OrdinalIgnoreCase));

        return new TruthReferenceScore
        {
            TruthAnchorsFound = found,
            MissingTruthAnchors = anchors.Length - found
        };
    }
}