namespace HallucinationLab.Core;

public sealed class PromptCase
{
    public string Id { get; set; } = string.Empty;
    public string Truth { get; set; } = string.Empty;
    public string Prompt { get; set; } = string.Empty;
    public double ToleranciaDefase { get; set; }
    public double FactorUmbralMagnitud { get; set; }
    public bool ExpectedHallucination { get; set; }
    public List<double> ReferenciaPromptVerdad { get; set; } = new();
    public List<double> ReferenciaRespuestaPrompt { get; set; } = new();
}

public sealed class CaseResult
{
    public required string Id { get; init; }
    public required string Prompt { get; init; }
    public required string BaselineOutput { get; init; }
    public required string GuardedOutput { get; init; }
    public required Eval.HallucinationScore BaselineScore { get; init; }
    public required Eval.HallucinationScore GuardedScore { get; init; }
}

public sealed class ExperimentReport
{
    public DateTimeOffset ExecutedAtUtc { get; init; } = DateTimeOffset.UtcNow;
    public required string BackendName { get; init; }
    public required IReadOnlyList<CaseResult> Cases { get; init; }
    public required Eval.AggregatedMetrics Baseline { get; init; }
    public required Eval.AggregatedMetrics Guarded { get; init; }
}
