namespace HallucinationLab.Guard;

public sealed class PassThroughGuard : IOutputGuard
{
    public string Name => "PassThroughGuard";

    public string Apply(string truth, string prompt, string rawOutput, IReadOnlyList<string> expectedFacts, IReadOnlyList<string> forbiddenClaims)
    {
        return rawOutput;
    }
}
