namespace HallucinationLab.Guard;

public sealed class PassThroughGuard : IOutputGuard
{
    public string Name => "PassThroughGuard";

    public string Apply(string truth, string prompt, string rawOutput)
    {
        return rawOutput;
    }
}
