using Domain.Abstractions;
using Application.DTOs;

namespace Application.UseCases;

public sealed class GenerateChatCompletionUseCase(IChatModelProvider chatModelProvider)
{
    public async Task<ChatCompletionResponse> Handle(ChatCompletionRequest request, CancellationToken cancellationToken = default)
    {
        var response = await chatModelProvider.GenerateAsync(request.Prompt, request.Model, cancellationToken);
        return new ChatCompletionResponse(response);
    }
}
