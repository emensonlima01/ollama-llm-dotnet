namespace Infrastructure.ChatProviders;

public sealed class ChatProviderSettings
{
    public string ActiveProvider { get; set; } = "Ollama";
    public OllamaProviderSettings Ollama { get; set; } = new();
}

public sealed class OllamaProviderSettings
{
    public string BaseUrl { get; set; } = "http://localhost:11434";
}
