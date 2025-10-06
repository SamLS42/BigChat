using BigChat.AppCore.ChatClient;
using BigChat.AppCore.Settings.AzureAIInference;
using BigChat.AppCore.Settings.Ollama;
using BigChat.AppCore.Settings.Onnx;

namespace BigChat.AppCore.Settings;

public interface ISettingsService
{
    AzureAIInferenceClientSettings GetAzureAIInferenceSettings();
    OllamaChatClientSettings GetOllamaChatSettings();
    OnnxChatClientSettings GetOnnxChatSettings();
    string GetAppTheme();
    SupportedClients GetSelectedClient();

    void SetAzureAIInferenceClientSettings(AzureAIInferenceClientSettings value);
    void SetOllamaChatClientSettings(OllamaChatClientSettings value);
    void SetOnnxChatClientSettings(OnnxChatClientSettings value);
    void SetAppTheme(string value);
    void SetSelectedClient(SupportedClients value);
}
