namespace Application.DTOs;

public record ChatRequest(
    string Prompt,
    string Model
);
