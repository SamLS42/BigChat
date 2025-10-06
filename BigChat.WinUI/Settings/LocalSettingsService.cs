using BigChat.AppCore.ChatClient;
using BigChat.AppCore.Settings;
using BigChat.AppCore.Settings.AzureAIInference;
using BigChat.AppCore.Settings.Ollama;
using BigChat.AppCore.Settings.Onnx;
using BigChat.Utils;
using System.Text.Json;
using Windows.Storage;

namespace BigChat.Settings;

internal sealed class LocalSettingsService : ISettingsService
{
    public AzureAIInferenceClientSettings GetAzureAIInferenceSettings()
    {
        if (ApplicationData.Current.LocalSettings.Values.TryGetValue(nameof(LocalSettings.AzureAIInferenceClientSettings), out object? item) && item is string value
            && JsonSerializer.Deserialize(value, SourceGenerationContext.Default.AzureAIInferenceClientSettings) is AzureAIInferenceClientSettings deserialized)
        {
            return deserialized;
        }

        AzureAIInferenceClientSettings storedValue = new();
        SetAzureAIInferenceClientSettings(storedValue);
        return storedValue;
    }
    public void SetAzureAIInferenceClientSettings(AzureAIInferenceClientSettings value)
    {
        string jsonValue = JsonSerializer.Serialize(value, SourceGenerationContext.Default.AzureAIInferenceClientSettings);
        ApplicationData.Current.LocalSettings.Values[nameof(LocalSettings.AzureAIInferenceClientSettings)] = jsonValue;
    }

    public OllamaChatClientSettings GetOllamaChatSettings()
    {
        if (ApplicationData.Current.LocalSettings.Values.TryGetValue(nameof(LocalSettings.OllamaChatClientSettings), out object? item) && item is string value
            && JsonSerializer.Deserialize(value, SourceGenerationContext.Default.OllamaChatClientSettings) is OllamaChatClientSettings deserialized)
        {
            return deserialized;
        }

        OllamaChatClientSettings storedValue = new();
        SetOllamaChatClientSettings(storedValue);
        return storedValue;
    }
    public void SetOllamaChatClientSettings(OllamaChatClientSettings value)
    {
        string jsonValue = JsonSerializer.Serialize(value, SourceGenerationContext.Default.OllamaChatClientSettings);
        ApplicationData.Current.LocalSettings.Values[nameof(LocalSettings.OllamaChatClientSettings)] = jsonValue;
    }

    public string GetAppTheme()
    {
        return GetStringValue(nameof(LocalSettings.AppTheme));
    }

    public void SetAppTheme(string value)
    {
        ApplicationData.Current.LocalSettings.Values[nameof(LocalSettings.AppTheme)] = value;
    }

    public SupportedClients GetSelectedClient()
    {
        if (ApplicationData.Current.LocalSettings.Values.TryGetValue(nameof(LocalSettings.SelectedClient), out object? item) && item is string value
            && JsonSerializer.Deserialize(value, SourceGenerationContext.Default.SupportedClients) is SupportedClients deserialized)
        {
            return deserialized;
        }

        SetSelectedClient(SupportedClients.Onnx);
        return SupportedClients.Onnx;
    }

    public void SetSelectedClient(SupportedClients value)
    {
        ApplicationData.Current.LocalSettings.Values[nameof(LocalSettings.SelectedClient)] = JsonSerializer.Serialize(value, SourceGenerationContext.Default.SupportedClients);
    }


    private static string GetStringValue(string key)
    {
        string storedValue = string.Empty;

        if (ApplicationData.Current.LocalSettings.Values.TryGetValue(key, out object? item) && item is string value)
        {
            storedValue = value;
        }

        return storedValue;
    }

    public OnnxChatClientSettings GetOnnxChatSettings()
    {
        if (ApplicationData.Current.LocalSettings.Values.TryGetValue(nameof(LocalSettings.OnnxChatClientSettings), out object? item) && item is string value
            && JsonSerializer.Deserialize(value, SourceGenerationContext.Default.OnnxChatClientSettings) is OnnxChatClientSettings deserialized)
        {
            return deserialized;
        }

        OnnxChatClientSettings storedValue = new();
        SetOnnxChatClientSettings(storedValue);
        return storedValue;
    }

    public void SetOnnxChatClientSettings(OnnxChatClientSettings value)
    {
        string jsonValue = JsonSerializer.Serialize(value, SourceGenerationContext.Default.OnnxChatClientSettings);
        ApplicationData.Current.LocalSettings.Values[nameof(LocalSettings.OnnxChatClientSettings)] = jsonValue;
    }
}