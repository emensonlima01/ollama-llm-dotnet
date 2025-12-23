namespace Infrastructure.ChatProviders;

public sealed class ChatProviderOptions
{
    public string Provider { get; set; } = "Ollama";
    public OllamaOptions Ollama { get; set; } = new();
}

public sealed class OllamaOptions
{
    public string BaseUrl { get; set; } = "http://localhost:11434";
}
