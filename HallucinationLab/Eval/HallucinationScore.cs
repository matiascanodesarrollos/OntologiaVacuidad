namespace HallucinationLab.Eval;

public sealed class HallucinationScore
{
    public bool ExpectedHallucination { get; init; }
    public bool HallucinationDetected { get; init; }
    public bool WasAbstention { get; init; }
    public bool MatchesExpectation { get; init; }
    public double HallucinationRate { get; init; }
}

public sealed class AggregatedMetrics
{
    public int CaseCount { get; init; }
    public double ExpectationAccuracy { get; init; }
    public double AvgHallucinationRate { get; init; }
}
