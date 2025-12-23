using Application.Abstractions;
using Application.DTOs;

namespace Application.UseCases;

public class ChatUseCase(IChatProvider chatProvider)
{
    public async Task<ChatResponse> Handle(ChatRequest request, CancellationToken cancellationToken = default)
    {
        var response = await chatProvider.GenerateAsync(request.Prompt, request.Model, cancellationToken);
        return new ChatResponse(response);
    }
}
