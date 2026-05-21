namespace EpistemicGuard;

public sealed class Metrics
{
    public double UnsupportedAssertionRate { get; init; }
    public double ConflictRate { get; init; }
    public double MeanConfidence { get; init; }
}

public sealed class MetricsTracker
{
    private readonly List<(ProjectionResult Projection, PolicyDecision Decision)> samples = new();

    public void Add(ProjectionResult projection, PolicyDecision decision)
    {
        samples.Add((projection, decision));
    }

    public Metrics Snapshot()
    {
        if (samples.Count == 0)
        {
            return new Metrics();
        }

        var assertive = samples
            .Where(s => s.Decision.Verdict is Verdict.Confirmed or Verdict.Plausible)
            .ToList();

        var unsupported = assertive
            .Count(s => s.Projection.SupportScore < 0.22 || s.Projection.CoverageScore < 0.25);

        var conflicts = samples.Count(s => s.Projection.ContradictionScore >= s.Projection.SupportScore && s.Projection.ContradictionScore >= 0.2);

        return new Metrics
        {
            UnsupportedAssertionRate = assertive.Count == 0 ? 0.0 : (double)unsupported / assertive.Count,
            ConflictRate = (double)conflicts / samples.Count,
            MeanConfidence = samples.Average(s => s.Decision.Confidence)
        };
    }
}
