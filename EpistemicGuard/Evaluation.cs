namespace EpistemicGuard;

public sealed record EvaluationSummary(
    string SystemName,
    int TotalCases,
    int AssertiveAnswers,
    int Hallucinations,
    int CorrectAbstentions,
    int CorrectAssertions,
    double HallucinationRateOverTotal,
    double HallucinationRateOverAssertive,
    double PrecisionOverAssertive,
    double AbstentionRate,
    double MeanConfidence);

public sealed record CaseOutcome(
    ClaimCase Case,
    ProjectionResult Projection,
    PolicyDecision Decision,
    bool IsAssertive,
    bool IsHallucination,
    bool IsCorrectAbstention,
    bool IsCorrectAssertion);

public interface IDecisionEngine
{
    PolicyDecision Decide(ProjectionResult projection);
}

public sealed class BaselineDecisionEngine : IDecisionEngine
{
    public PolicyDecision Decide(ProjectionResult projection)
    {
        var evidenceIds = projection.TopEvidence.Select(x => x.Evidence.Id).ToList();
        var confidence = Math.Clamp(projection.SupportScore + 0.15, 0.0, 1.0);

        if (projection.TopEvidence.Count == 0)
        {
            return new PolicyDecision(Verdict.Speculative, "Baseline responde incluso sin evidencia.", confidence, evidenceIds);
        }

        if (projection.SupportScore >= 0.08)
        {
            return new PolicyDecision(Verdict.Confirmed, "Baseline favorece responder con evidencia debil.", confidence, evidenceIds);
        }

        return new PolicyDecision(Verdict.Plausible, "Baseline mantiene respuesta asertiva.", confidence, evidenceIds);
    }
}

public sealed class GuardedDecisionEngineAdapter : IDecisionEngine
{
    private readonly PolicyEngine engine = new();

    public PolicyDecision Decide(ProjectionResult projection)
    {
        return engine.Decide(projection);
    }
}

public sealed class EvaluationRunner
{
    private readonly EvidenceProjector projector = new();

    public (EvaluationSummary Summary, IReadOnlyList<CaseOutcome> Outcomes) Run(
        string systemName,
        IDecisionEngine engine,
        IReadOnlyList<ClaimCase> suite,
        IReadOnlyList<EvidenceItem> corpus)
    {
        var outcomes = suite
            .Select(c => ScoreCase(c, engine, corpus))
            .ToList();

        var total = outcomes.Count;
        var assertive = outcomes.Count(o => o.IsAssertive);
        var hallucinations = outcomes.Count(o => o.IsHallucination);
        var correctAbstentions = outcomes.Count(o => o.IsCorrectAbstention);
        var correctAssertions = outcomes.Count(o => o.IsCorrectAssertion);
        var meanConfidence = outcomes.Count == 0 ? 0.0 : outcomes.Average(o => o.Decision.Confidence);

        var summary = new EvaluationSummary(
            systemName,
            total,
            assertive,
            hallucinations,
            correctAbstentions,
            correctAssertions,
            total == 0 ? 0.0 : (double)hallucinations / total,
            assertive == 0 ? 0.0 : (double)hallucinations / assertive,
            assertive == 0 ? 0.0 : (double)correctAssertions / assertive,
            total == 0 ? 0.0 : (double)(total - assertive) / total,
            meanConfidence);

        return (summary, outcomes);
    }

    private CaseOutcome ScoreCase(ClaimCase claimCase, IDecisionEngine engine, IReadOnlyList<EvidenceItem> corpus)
    {
        var projection = projector.Project(claimCase.Claim, corpus, topK: 4);
        var decision = engine.Decide(projection);
        var assertive = decision.Verdict is Verdict.Confirmed or Verdict.Plausible;

        var hallucination = assertive && claimCase.Truth is GroundTruth.Contradicted or GroundTruth.Unknown;
        var correctAssertion = assertive && claimCase.Truth is GroundTruth.Supported;
        var correctAbstention = !assertive && claimCase.Truth is GroundTruth.Contradicted or GroundTruth.Unknown;

        return new CaseOutcome(
            claimCase,
            projection,
            decision,
            assertive,
            hallucination,
            correctAbstention,
            correctAssertion);
    }
}
