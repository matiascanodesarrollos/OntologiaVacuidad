namespace HallucinationLab.Guard;

public sealed class OntologiaOutputGuard : IOutputGuard
{
    public string Name => "OntologiaOutputGuard";

    public string Apply(Core.PromptCase promptCase, string rawOutput)
    {
        if (string.IsNullOrWhiteSpace(rawOutput))
        {
            return AbstentionResponse;
        }

        var hallucina = Eval.TestParityHallucinationEvaluator.DetectHallucination(promptCase, rawOutput);
        if (hallucina)
        {
            return AbstentionResponse;
        }

        return rawOutput;
    }

    private const string AbstentionResponse = "Me abstengo: no hay acoplamiento suficiente.";
}
