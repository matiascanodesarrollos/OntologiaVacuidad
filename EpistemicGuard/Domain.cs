namespace EpistemicGuard;

public sealed record Claim(string Text);

public sealed record EvidenceItem(
    string Id,
    string Source,
    string Text,
    double Reliability,
    DateTimeOffset Timestamp);

public sealed record ProjectedEvidence(
    EvidenceItem Evidence,
    double LexicalSimilarity,
    double WeightedSupport,
    bool ContradictsClaim);

public sealed record ProjectionResult(
    IReadOnlyList<ProjectedEvidence> TopEvidence,
    double SupportScore,
    double ContradictionScore,
    double CoverageScore);

public enum Verdict
{
    Confirmed,
    Plausible,
    Speculative,
    Abstain
}

public sealed record PolicyDecision(
    Verdict Verdict,
    string Rationale,
    double Confidence,
    IReadOnlyList<string> EvidenceIds);
