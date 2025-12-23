namespace Domain.Abstractions;

public interface IChatModelProvider
{
    Task<string> GenerateAsync(string prompt, string model, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<string>> ListModelsAsync(CancellationToken cancellationToken = default);
}
