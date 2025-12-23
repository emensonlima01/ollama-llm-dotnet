using System.Buffers;
using System.IO.Pipelines;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using Domain.Abstractions;

namespace Infrastructure.ChatProviders.Ollama;

public sealed class OllamaChatModelProvider(HttpClient httpClient) : IChatModelProvider
{
    public async IAsyncEnumerable<string> GenerateAsync(
        string prompt,
        string model,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var payload = new OllamaGenerateRequest(model, prompt) { Stream = true };
        using var request = new HttpRequestMessage(HttpMethod.Post, "/api/generate")
        {
            Content = JsonContent.Create(payload)
        };

        using var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        response.EnsureSuccessStatusCode();

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        var reader = PipeReader.Create(stream);

        while (true)
        {
            var readResult = await reader.ReadAsync(cancellationToken);
            var buffer = readResult.Buffer;

            while (TryReadLine(ref buffer, out var line))
            {
                if (line.IsEmpty)
                {
                    continue;
                }

                if (TryParseChunk(line, out var chunk, out var done))
                {
                    if (!string.IsNullOrEmpty(chunk))
                    {
                        yield return chunk;
                    }

                    if (done)
                    {
                        await reader.CompleteAsync();
                        yield break;
                    }
                }
            }

            reader.AdvanceTo(buffer.Start, buffer.End);

            if (readResult.IsCompleted)
            {
                break;
            }
        }

        await reader.CompleteAsync();
    }

    public async Task<IReadOnlyList<string>> ListModelsAsync(CancellationToken cancellationToken = default)
    {
        var response = await httpClient.GetFromJsonAsync<OllamaTagsResponse>("/api/tags", cancellationToken);
        return response?.Models?.Select(m => m.Name).ToArray() ?? [];
    }

    private sealed record OllamaGenerateRequest(
        string Model,
        string Prompt)
    {
        [JsonPropertyName("stream")]
        public bool Stream { get; init; } = false;
    }

    private sealed record OllamaTagsResponse(
        [property: JsonPropertyName("models")] IReadOnlyList<OllamaModelInfo> Models);

    private sealed record OllamaModelInfo(
        [property: JsonPropertyName("name")] string Name);

    private static bool TryReadLine(ref ReadOnlySequence<byte> buffer, out ReadOnlySequence<byte> line)
    {
        var position = buffer.PositionOf((byte)'\n');
        if (position is null)
        {
            line = default;
            return false;
        }

        line = buffer.Slice(0, position.Value);
        buffer = buffer.Slice(buffer.GetPosition(1, position.Value));

        if (line.Length > 0)
        {
            var lastByte = line.Slice(line.Length - 1, 1).FirstSpan[0];
            if (lastByte == (byte)'\r')
            {
                line = line.Slice(0, line.Length - 1);
            }
        }

        return true;
    }

    private static bool TryParseChunk(ReadOnlySequence<byte> line, out string? chunk, out bool done)
    {
        chunk = null;
        done = false;

        try
        {
            var reader = new Utf8JsonReader(line, isFinalBlock: true, state: default);
            while (reader.Read())
            {
                if (reader.TokenType != JsonTokenType.PropertyName)
                {
                    continue;
                }

                if (reader.ValueTextEquals("response"))
                {
                    if (!reader.Read())
                    {
                        break;
                    }

                    if (reader.TokenType == JsonTokenType.String)
                    {
                        chunk = reader.GetString();
                    }
                }
                else if (reader.ValueTextEquals("done"))
                {
                    if (!reader.Read())
                    {
                        break;
                    }

                    if (reader.TokenType is JsonTokenType.True or JsonTokenType.False)
                    {
                        done = reader.GetBoolean();
                    }
                }
                else
                {
                    if (!reader.Read())
                    {
                        break;
                    }

                    if (reader.TokenType is JsonTokenType.StartObject or JsonTokenType.StartArray)
                    {
                        reader.Skip();
                    }
                }
            }

            return chunk is not null || done;
        }
        catch (JsonException)
        {
            return false;
        }
    }
}
