namespace HallucinationLab.Eval;

public static class HallucinationEvaluator
{
    public static HallucinationScore Evaluate(Core.PromptCase promptCase, string output)
    {
        var hallucinationDetected = TestParityHallucinationEvaluator.DetectHallucination(promptCase, output);
        var rate = hallucinationDetected ? 1.0 : 0.0;

        return new HallucinationScore
        {
            ExpectedHallucination = promptCase.ExpectedHallucination,
            HallucinationDetected = hallucinationDetected,
            MatchesExpectation = hallucinationDetected == promptCase.ExpectedHallucination,
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
            ExpectationAccuracy = list.Average(s => s.MatchesExpectation ? 1.0 : 0.0),
            AvgHallucinationRate = list.Average(s => s.HallucinationRate)
        };
    }
}
