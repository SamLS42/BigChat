using BigChat.AppCore.Settings.OpenAI;

namespace BigChat.AppCore.Settings.Ollama;

public class OllamaChatClientSettings : BaseAIClientSettings
{
    public string CompletionModel { get; set; } = string.Empty;
    public string Endpoint { get; set; } = "http://localhost:11434";
}
