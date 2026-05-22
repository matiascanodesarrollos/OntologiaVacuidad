namespace HallucinationLab.Guard;

public sealed class PassThroughGuard : IOutputGuard
{
    public string Name => "PassThroughGuard";

    public string Apply(string prompt, string rawOutput)
    {
        return rawOutput;
    }
}
