namespace Domain.Abstractions;

public interface IChatModelProvider
{
    IAsyncEnumerable<string> GenerateAsync(string prompt, string model, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<string>> ListModelsAsync(CancellationToken cancellationToken = default);
}
