namespace HallucinationLab.Eval;

public static class HallucinationEvaluator
{
    public static HallucinationScore Evaluate(string output, string truth)
    {
        var truthScore = TruthReferenceEvaluator.Evaluate(output, truth);
        var hallucinationDetected = truthScore.MissingAnchors > 0;
        var rate = hallucinationDetected ? 1.0 : 0.0;

        return new HallucinationScore
        {
            SupportedFacts = truthScore.AnchorsFound,
            MissingFacts = truthScore.MissingAnchors,
            HallucinationDetected = hallucinationDetected,
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
            AvgHallucinationRate = list.Average(s => s.HallucinationRate)
        };
    }
}
