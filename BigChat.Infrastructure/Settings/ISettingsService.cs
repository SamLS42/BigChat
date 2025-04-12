using BigChat.Infrastructure.ChatClient;

namespace BigChat.Infrastructure.Settings;

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
