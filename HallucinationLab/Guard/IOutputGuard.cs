namespace HallucinationLab.Guard;

public interface IOutputGuard
{
    string Name { get; }
    string Apply(string prompt, string rawOutput, IReadOnlyList<string> expectedFacts, IReadOnlyList<string> forbiddenClaims);
}
