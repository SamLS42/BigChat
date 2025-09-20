using BigChat.AppCore.ChatClient;

namespace BigChat.AppCore.Settings;

public interface ISettingsService
{
    ChatCompletionsClientSettings GetChatCompletionsSettings();
    OllamaChatClientSettings GetOllamaChatSettings();
    string GetAppTheme();
    SupportedClients GetSelectedClient();

    void SetChatCompletionsClientSettings(ChatCompletionsClientSettings value);
    void SetOllamaChatClientSettings(OllamaChatClientSettings value);
    void SetAppTheme(string value);
    void SetSelectedClient(SupportedClients value);
}
