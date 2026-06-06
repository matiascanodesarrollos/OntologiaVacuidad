namespace HallucinationLab.Backends;

public interface ITextModelBackend
{
    string Name { get; }
    Task<string> GenerateAsync(string caseId, string prompt, CancellationToken cancellationToken);
}
