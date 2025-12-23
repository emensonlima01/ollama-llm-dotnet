using Domain.Abstractions;
using Application.DTOs;

namespace Application.UseCases;

public sealed class ListChatModelsUseCase(IChatModelProvider chatModelProvider)
{
    public async Task<ChatModelsResponse> Handle(CancellationToken cancellationToken = default)
    {
        var models = await chatModelProvider.ListModelsAsync(cancellationToken);
        return new ChatModelsResponse(models);
    }
}
