namespace HallucinationLab.Backends;

public interface ITextModelBackend
{
    string Name { get; }
    Task<string> GenerateAsync(string prompt, CancellationToken cancellationToken);
}
