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

        group.MapPost("/", SendPrompt)
            .WithName("ChatPrompt");
    }

    private static async Task<IResult> SendPrompt(
        [FromBody] ChatRequest request,
        [FromServices] ChatUseCase useCase)
    {
        var response = await useCase.Handle(request);
        return Results.Ok(response);
    }
}
