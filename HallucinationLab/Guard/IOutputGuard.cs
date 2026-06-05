namespace HallucinationLab.Guard;

public interface IOutputGuard
{
    string Name { get; }
    string Apply(string truth, string prompt, string rawOutput);
}
