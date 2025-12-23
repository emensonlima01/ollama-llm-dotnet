using Application.Abstractions;
using Application.DTOs;

namespace Application.UseCases;

public sealed class ListModelsUseCase
{
    private readonly IChatProvider _chatProvider;

    public ListModelsUseCase(IChatProvider chatProvider)
    {
        _chatProvider = chatProvider;
    }

    public async Task<ModelsResponse> Handle(CancellationToken cancellationToken = default)
    {
        var models = await _chatProvider.ListModelsAsync(cancellationToken);
        return new ModelsResponse(models);
    }
}
