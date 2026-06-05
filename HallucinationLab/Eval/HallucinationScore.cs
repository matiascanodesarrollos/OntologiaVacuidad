namespace HallucinationLab.Eval;

public sealed class HallucinationScore
{
    public int SupportedFacts { get; init; }
    public int MissingFacts { get; init; }
    public bool HallucinationDetected { get; init; }
    public double HallucinationRate { get; init; }
}

public sealed class AggregatedMetrics
{
    public int CaseCount { get; init; }
    public double AvgSupportedFacts { get; init; }
    public double AvgMissingFacts { get; init; }
    public double AvgHallucinationRate { get; init; }
}
