namespace Application.DTOs;

public sealed record ChatCompletionRequest(
    string Prompt,
    string Model
);
