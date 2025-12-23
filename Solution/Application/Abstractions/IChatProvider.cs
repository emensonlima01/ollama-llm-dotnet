namespace Application.Abstractions;

public interface IChatProvider
{
    Task<string> GenerateAsync(string prompt, string model, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<string>> ListModelsAsync(CancellationToken cancellationToken = default);
}
