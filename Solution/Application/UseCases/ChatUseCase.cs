using Application.DTOs;

namespace Application.UseCases;

public class ChatUseCase
{
    public Task<ChatResponse> Handle(ChatRequest request)
    {
        // TODO: implementar regras de negocio para gerar a resposta.
        var response = new ChatResponse("TODO: implementar regras de negocio.");
        return Task.FromResult(response);
    }
}
