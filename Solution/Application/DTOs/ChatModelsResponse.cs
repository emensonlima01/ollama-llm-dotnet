namespace Application.DTOs;

public sealed record ChatModelsResponse(
    IReadOnlyList<string> Models
);
