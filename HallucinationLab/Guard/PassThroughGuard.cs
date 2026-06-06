namespace HallucinationLab.Guard;

public sealed class PassThroughGuard : IOutputGuard
{
    public string Name => "PassThroughGuard";

    public string Apply(Core.PromptCase promptCase, string rawOutput)
    {
        return rawOutput;
    }
}
