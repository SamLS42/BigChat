using BigChat.Infrastructure.ChatClient;
using BigChat.Infrastructure.Settings;
using BigChat.Utils;
using System.Text.Json;
using Windows.Storage;

namespace BigChat.Settings;

internal sealed class LocalSettingsService : ISettingsService
{
    public ChatCompletionsClientSettings GetChatCompletionsSettings()
    {
        ChatCompletionsClientSettings? storedValue = null;

        if (ApplicationData.Current.LocalSettings.Values.TryGetValue(nameof(LocalSettings.ChatCompletionsClientSettings), out object? item) && item is string value)
        {
            storedValue = JsonSerializer.Deserialize(value, SourceGenerationContext.Default.ChatCompletionsClientSettings);
        }

        return storedValue ?? new();
    }
    public void SetChatCompletionsClientSettings(ChatCompletionsClientSettings value)
    {
        string jsonValue = JsonSerializer.Serialize(value, SourceGenerationContext.Default.ChatCompletionsClientSettings);
        ApplicationData.Current.LocalSettings.Values[nameof(LocalSettings.ChatCompletionsClientSettings)] = jsonValue;
    }

    public OllamaChatClientSettings GetOllamaChatSettings()
    {
        OllamaChatClientSettings? storedValue = null;

        if (ApplicationData.Current.LocalSettings.Values.TryGetValue(nameof(LocalSettings.OllamaChatClientSettings), out object? item) && item is string value)
        {
            storedValue = JsonSerializer.Deserialize(value, SourceGenerationContext.Default.OllamaChatClientSettings);
        }

        return storedValue ?? new();
    }
    public void SetOllamaChatClientSettings(OllamaChatClientSettings value)
    {
        string jsonValue = JsonSerializer.Serialize(value, SourceGenerationContext.Default.OllamaChatClientSettings);
        ApplicationData.Current.LocalSettings.Values[nameof(LocalSettings.OllamaChatClientSettings)] = jsonValue;
    }
    public string GetAppTheme()
    {
        string storedValue = string.Empty;

        if (ApplicationData.Current.LocalSettings.Values.TryGetValue(nameof(LocalSettings.AppTheme), out object? item) && item is string value)
        {
            storedValue = value;
        }

        return storedValue;
    }
    public void SetAppTheme(string value)
    {
        ApplicationData.Current.LocalSettings.Values[nameof(LocalSettings.AppTheme)] = value;
    }

    public SupportedClients GetSelectedClient()
    {
        SupportedClients? storedValue = null;

        if (ApplicationData.Current.LocalSettings.Values.TryGetValue(nameof(LocalSettings.SelectedClient), out object? item) && item is string value)
        {
            storedValue = JsonSerializer.Deserialize(value, SourceGenerationContext.Default.SupportedClients);
        }

        return storedValue ?? default;
    }

    public void SetSelectedClient(SupportedClients value)
    {
        ApplicationData.Current.LocalSettings.Values[nameof(LocalSettings.SelectedClient)] = JsonSerializer.Serialize(value, SourceGenerationContext.Default.SupportedClients);
    }
}