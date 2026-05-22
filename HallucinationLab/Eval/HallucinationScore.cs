namespace HallucinationLab.Eval;

public sealed class HallucinationScore
{
    public int SupportedFacts { get; init; }
    public int MissingFacts { get; init; }
    public int ForbiddenClaimsFound { get; init; }
    public double HallucinationRate { get; init; }
}

public sealed class AggregatedMetrics
{
    public int CaseCount { get; init; }
    public double AvgSupportedFacts { get; init; }
    public double AvgMissingFacts { get; init; }
    public double AvgForbiddenClaims { get; init; }
    public double AvgHallucinationRate { get; init; }
}
