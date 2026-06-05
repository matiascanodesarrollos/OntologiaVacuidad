namespace HallucinationLab.Guard;

public sealed class OntologiaOutputGuard : IOutputGuard
{
    public string Name => "OntologiaOutputGuard";

    public string Apply(
        string truth,
        string prompt, 
        string rawOutput, 
        IReadOnlyList<string> expectedFacts, 
        IReadOnlyList<string> forbiddenClaims)
    {
        if (string.IsNullOrWhiteSpace(rawOutput))
        {
            return AbstentionResponse;
        }

        var score = HallucinationLab.Eval.HallucinationEvaluator.Evaluate(rawOutput, expectedFacts, forbiddenClaims);
        var truthScore = HallucinationLab.Eval.TruthReferenceEvaluator.Evaluate(rawOutput, truth);
        if (score.MissingFacts > 0 || score.ForbiddenClaimsFound > 0 || truthScore.MissingTruthAnchors > 0)
        {
            return AbstentionResponse;
        }

        return rawOutput;
    }

    private const string AbstentionResponse = "Me abstengo: no hay acoplamiento suficiente.";
}
