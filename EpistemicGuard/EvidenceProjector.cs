namespace EpistemicGuard;

public sealed class EvidenceProjector
{
    public ProjectionResult Project(Claim claim, IReadOnlyList<EvidenceItem> corpus, int topK = 3)
    {
        var claimTokens = TextProcessing.Tokenize(claim.Text);
        var claimNegated = TextProcessing.HasNegation(claim.Text);

        var projected = corpus
            .Select(item => ProjectEvidence(claimTokens, claimNegated, item))
            .OrderByDescending(x => x.WeightedSupport)
            .Take(topK)
            .ToList();

        if (projected.Count == 0)
        {
            return new ProjectionResult(Array.Empty<ProjectedEvidence>(), 0.0, 0.0, 0.0);
        }

        var support = projected.Average(x => x.ContradictsClaim ? 0.0 : x.WeightedSupport);
        var contradiction = projected.Average(x => x.ContradictsClaim ? x.WeightedSupport : 0.0);

        var evidenceTokens = projected
            .SelectMany(x => TextProcessing.Tokenize(x.Evidence.Text))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var covered = claimTokens.Intersect(evidenceTokens, StringComparer.OrdinalIgnoreCase).Count();
        var coverage = claimTokens.Count == 0 ? 0.0 : (double)covered / claimTokens.Count;

        return new ProjectionResult(projected, support, contradiction, coverage);
    }

    private static ProjectedEvidence ProjectEvidence(HashSet<string> claimTokens, bool claimNegated, EvidenceItem item)
    {
        var evidenceTokens = TextProcessing.Tokenize(item.Text);
        var lexical = TextProcessing.JaccardSimilarity(claimTokens, evidenceTokens);

        var ageDays = Math.Max(0.0, (DateTimeOffset.UtcNow - item.Timestamp).TotalDays);
        var recencyWeight = Math.Exp(-ageDays / 365.0);

        var weighted = lexical * item.Reliability * recencyWeight;

        var evidenceNegated = TextProcessing.HasNegation(item.Text);
        var contradicts = claimNegated != evidenceNegated && lexical >= 0.25;

        return new ProjectedEvidence(item, lexical, weighted, contradicts);
    }
}
