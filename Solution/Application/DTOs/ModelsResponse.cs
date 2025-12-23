namespace Application.DTOs;

public sealed record ModelsResponse(
    IReadOnlyList<string> Models
);
