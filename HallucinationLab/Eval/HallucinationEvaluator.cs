namespace HallucinationLab.Eval;

public static class HallucinationEvaluator
{
    public static HallucinationScore Evaluate(string output, IReadOnlyList<string> expectedFacts, IReadOnlyList<string> forbiddenClaims)
    {
        var supported = expectedFacts.Count(f => ContainsIgnoreCase(output, f));
        var missing = expectedFacts.Count - supported;
        var forbidden = forbiddenClaims.Count(c => ContainsIgnoreCase(output, c));

        var denominator = Math.Max(1, expectedFacts.Count + forbiddenClaims.Count);
        var rate = forbidden / (double)denominator;

        return new HallucinationScore
        {
            SupportedFacts = supported,
            MissingFacts = missing,
            ForbiddenClaimsFound = forbidden,
            HallucinationRate = rate
        };
    }

    public static AggregatedMetrics Aggregate(IEnumerable<HallucinationScore> scores)
    {
        var list = scores.ToList();
        if (list.Count == 0)
        {
            return new AggregatedMetrics();
        }

        return new AggregatedMetrics
        {
            CaseCount = list.Count,
            AvgSupportedFacts = list.Average(s => s.SupportedFacts),
            AvgMissingFacts = list.Average(s => s.MissingFacts),
            AvgForbiddenClaims = list.Average(s => s.ForbiddenClaimsFound),
            AvgHallucinationRate = list.Average(s => s.HallucinationRate)
        };
    }

    private static bool ContainsIgnoreCase(string text, string fragment)
    {
        return text.Contains(fragment, StringComparison.OrdinalIgnoreCase);
    }
}
