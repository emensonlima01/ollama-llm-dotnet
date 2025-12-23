using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Application.Abstractions;

namespace Infrastructure.ChatProviders.Ollama;

public sealed class OllamaChatProvider(HttpClient httpClient) : IChatProvider
{
    public async Task<string> GenerateAsync(string prompt, string model, CancellationToken cancellationToken = default)
    {
        var payload = new OllamaGenerateRequest(model, prompt);
        using var response = await httpClient.PostAsJsonAsync("/api/generate", payload, cancellationToken);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<OllamaGenerateResponse>(cancellationToken: cancellationToken);
        return result?.Response ?? string.Empty;
    }

    public async Task<IReadOnlyList<string>> ListModelsAsync(CancellationToken cancellationToken = default)
    {
        var response = await httpClient.GetFromJsonAsync<OllamaTagsResponse>("/api/tags", cancellationToken);
        return response?.Models?.Select(m => m.Name).ToArray() ?? Array.Empty<string>();
    }

    private sealed record OllamaGenerateRequest(
        string Model,
        string Prompt)
    {
        [JsonPropertyName("stream")]
        public bool Stream { get; init; } = false;
    }

    private sealed record OllamaGenerateResponse(
        [property: JsonPropertyName("response")] string Response);

    private sealed record OllamaTagsResponse(
        [property: JsonPropertyName("models")] IReadOnlyList<OllamaModelInfo> Models);

    private sealed record OllamaModelInfo(
        [property: JsonPropertyName("name")] string Name);
}
