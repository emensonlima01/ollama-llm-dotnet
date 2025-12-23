using System.Buffers;
using System.Text;
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
        var stream = useCase.Handle(request, cancellationToken);
        return Results.Stream(async responseStream =>
        {
            await foreach (var chunk in stream.WithCancellation(cancellationToken))
            {
                var byteCount = Encoding.UTF8.GetByteCount(chunk);
                var buffer = ArrayPool<byte>.Shared.Rent(byteCount);
                try
                {
                    var bytesWritten = Encoding.UTF8.GetBytes(chunk, buffer);
                    await responseStream.WriteAsync(buffer.AsMemory(0, bytesWritten), cancellationToken);
                }
                finally
                {
                    ArrayPool<byte>.Shared.Return(buffer);
                }

                await responseStream.FlushAsync(cancellationToken);
            }
        }, "text/plain; charset=utf-8");
    }

    private static async Task<IResult> ListModels(
        [FromServices] ListChatModelsUseCase useCase,
        CancellationToken cancellationToken)
    {
        var response = await useCase.Handle(cancellationToken);
        return Results.Ok(response);
    }
}
