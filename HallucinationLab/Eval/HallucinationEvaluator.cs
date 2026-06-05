namespace HallucinationLab.Eval;

public sealed class HallucinationEvaluator
{
    public HallucinationScore Evaluate(Core.PromptCase promptCase, string output)
    {
        var wasAbstention = IsAbstention(output);
        var hallucinationDetected = TestParityHallucinationEvaluator.DetectHallucination(promptCase, output);
        var rate = hallucinationDetected ? 1.0 : 0.0;
        var matchesExpectation = hallucinationDetected == promptCase.ExpectedHallucination;

        if (wasAbstention
            && promptCase.ExpectedHallucination)
        {
            matchesExpectation = true;
        }

        return new HallucinationScore
        {
            ExpectedHallucination = promptCase.ExpectedHallucination,
            HallucinationDetected = hallucinationDetected,
            WasAbstention = wasAbstention,
            MatchesExpectation = matchesExpectation,
            HallucinationRate = rate
        };
    }

    private bool IsAbstention(string output)
    {
        return output.StartsWith("Me abstengo:", StringComparison.OrdinalIgnoreCase);
    }

    public AggregatedMetrics Aggregate(IEnumerable<HallucinationScore> scores)
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
