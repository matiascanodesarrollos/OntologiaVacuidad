namespace HallucinationLab.Guard;

public sealed class OntologiaOutputGuard : IOutputGuard
{
    public string Name => "OntologiaOutputGuard";

    public string Apply(
        string prompt, 
        string rawOutput, 
        IReadOnlyList<string> expectedFacts, 
        IReadOnlyList<string> forbiddenClaims)
    {
        if (string.IsNullOrWhiteSpace(rawOutput))
        {
            return AbstentionResponse;
        }

        return rawOutput;
    }

    private const string AbstentionResponse = "Me abstengo: no hay acoplamiento suficiente.";
}
