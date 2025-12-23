using Domain.Abstractions;
using Application.DTOs;

namespace Application.UseCases;

public sealed class GenerateChatCompletionUseCase(IChatModelProvider chatModelProvider)
{
    public IAsyncEnumerable<string> Handle(ChatCompletionRequest request, CancellationToken cancellationToken = default)
        => chatModelProvider.GenerateAsync(request.Prompt, request.Model, cancellationToken);
}
