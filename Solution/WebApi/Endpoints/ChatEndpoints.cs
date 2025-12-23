using Application.DTOs;
using Application.UseCases;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Endpoints;

public static class ChatEndpoints
{
    public static void MapChatEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/chat")
            .WithTags("Chat");

        group.MapPost("/", GenerateCompletion)
            .WithName("ChatCompletion");

        group.MapGet("/models", ListModels)
            .WithName("ChatModels");
    }

    private static async Task<IResult> GenerateCompletion(
        [FromBody] ChatCompletionRequest request,
        [FromServices] GenerateChatCompletionUseCase useCase,
        CancellationToken cancellationToken)
    {
        var response = await useCase.Handle(request, cancellationToken);
        return Results.Ok(response);
    }

    private static async Task<IResult> ListModels(
        [FromServices] ListChatModelsUseCase useCase,
        CancellationToken cancellationToken)
    {
        var response = await useCase.Handle(cancellationToken);
        return Results.Ok(response);
    }
}
