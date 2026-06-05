namespace HallucinationLab.Guard;

public sealed class OntologiaOutputGuard : IOutputGuard
{
    public string Name => "OntologiaOutputGuard";

    public string Apply(
        string truth,
        string prompt, 
        string rawOutput)
    {
        if (string.IsNullOrWhiteSpace(rawOutput))
        {
            return AbstentionResponse;
        }

        var truthScore = Eval.TruthReferenceEvaluator.Evaluate(rawOutput, truth);
        if (truthScore.MissingAnchors > 0)
        {
            return AbstentionResponse;
        }

        return rawOutput;
    }

    private const string AbstentionResponse = "Me abstengo: no hay acoplamiento suficiente.";
}
