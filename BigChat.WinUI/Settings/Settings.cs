using BigChat.AppCore.Settings.AzureAIInference;
using BigChat.AppCore.Settings.Ollama;
using BigChat.AppCore.Settings.Onnx;

namespace BigChat.Settings;

internal sealed class Settings
{
    public string? AppTheme { get; set; }
    public string? SelectedClient { get; set; }
    public string? OnnxModelDir { get; set; }
    public required AzureAIInferenceClientSettings AzureAIInferenceClientSettings { get; set; }
    public required OllamaChatClientSettings OllamaChatClientSettings { get; set; }
    public required OnnxChatClientSettings OnnxChatClientSettings { get; set; }
}
