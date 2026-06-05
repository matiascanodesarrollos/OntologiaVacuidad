namespace HallucinationLab.Eval;

public sealed class TruthReferenceScore
{
    public int AnchorsFound { get; init; }
    public int MissingAnchors { get; init; }
}

public static class TruthReferenceEvaluator
{
    public static TruthReferenceScore Evaluate(string output, string truth)
    {
        var anchors = truth
            .Split(':', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Where(anchor => !string.IsNullOrWhiteSpace(anchor))
            .ToArray();

        var found = anchors.Count(anchor => output.Contains(anchor, StringComparison.OrdinalIgnoreCase));

        return new TruthReferenceScore
        {
            AnchorsFound = found,
            MissingAnchors = anchors.Length - found
        };
    }
}