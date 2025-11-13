using BigChat.AppCore.ChatClients;
using BigChat.AppCore.Settings.AzureAIInference;
using BigChat.AppCore.Settings.Ollama;
using BigChat.AppCore.Settings.Onnx;
using BigChat.AppCore.Settings.OpenAI;

namespace BigChat.AppCore.Settings;

public interface ISettingsService
{
    AzureAIInferenceClientSettings GetAzureAIInferenceSettings();
    OpenAIClientSettings GetOpenAISettings();
    OllamaChatClientSettings GetOllamaChatSettings();
    OnnxChatClientSettings GetOnnxChatSettings();
    string GetAppTheme();
    IObservable<SupportedClients> SelectedClientChanges { get; }
    SupportedClients SelectedClient { get; }
    WindowState GetWindowState();

    void SetAzureAIInferenceClientSettings(AzureAIInferenceClientSettings value);
    void SetOpenAIClientSettings(OpenAIClientSettings value);
    void SetOllamaChatClientSettings(OllamaChatClientSettings value);
    void SetOnnxChatClientSettings(OnnxChatClientSettings value);
    void SetAppTheme(string value);
    void SetSelectedClient(SupportedClients value);
    void SetWindowState(WindowState state);
}
