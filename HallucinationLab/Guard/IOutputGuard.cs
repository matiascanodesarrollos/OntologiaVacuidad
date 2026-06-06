namespace HallucinationLab.Guard;

public interface IOutputGuard
{
    string Name { get; }
    string Apply(Core.PromptCase promptCase, string rawOutput);
}
