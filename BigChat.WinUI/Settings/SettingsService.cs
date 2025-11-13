using BigChat.AppCore.ChatClients;
using BigChat.AppCore.Settings;
using BigChat.AppCore.Settings.AzureAIInference;
using BigChat.AppCore.Settings.Ollama;
using BigChat.AppCore.Settings.Onnx;
using BigChat.AppCore.Settings.OpenAI;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text.Json;
using Windows.Storage;

namespace BigChat.Settings;

internal sealed class SettingsService : ISettingsService
{
    public SettingsService()
    {
        SupportedClients selectedCliente = SupportedClients.None;

        if (ApplicationData.Current.LocalSettings.Values.TryGetValue(nameof(Settings.SelectedClient), out object? item) && item is string value
            && JsonSerializer.Deserialize(value, SourceGenerationContext.Default.SupportedClients) is SupportedClients deserialized)
        {
            selectedCliente = deserialized;
        }

        SelectedClienteSource = new BehaviorSubject<SupportedClients>(selectedCliente);
    }

    public AzureAIInferenceClientSettings GetAzureAIInferenceSettings()
    {
        if (ApplicationData.Current.LocalSettings.Values.TryGetValue(nameof(Settings.AzureAIInferenceClientSettings), out object? item) && item is string value
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
        ApplicationData.Current.LocalSettings.Values[nameof(Settings.AzureAIInferenceClientSettings)] = jsonValue;
    }

    public OllamaChatClientSettings GetOllamaChatSettings()
    {
        if (ApplicationData.Current.LocalSettings.Values.TryGetValue(nameof(Settings.OllamaChatClientSettings), out object? item) && item is string value
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
        ApplicationData.Current.LocalSettings.Values[nameof(Settings.OllamaChatClientSettings)] = jsonValue;
    }

    public string GetAppTheme()
    {
        return GetStringValue(nameof(Settings.AppTheme));
    }

    public void SetAppTheme(string value)
    {
        ApplicationData.Current.LocalSettings.Values[nameof(Settings.AppTheme)] = value;
    }

    private BehaviorSubject<SupportedClients> SelectedClienteSource { get; }
    public IObservable<SupportedClients> SelectedClientChanges => SelectedClienteSource.AsObservable();
    public SupportedClients SelectedClient => SelectedClienteSource.Value;

    public WindowState GetWindowState()
    {
        if (ApplicationData.Current.LocalSettings.Values.TryGetValue(nameof(Settings.WindowState), out object? item) && item is string value
            && JsonSerializer.Deserialize(value, SourceGenerationContext.Default.WindowState) is WindowState deserialized)
        {
            return deserialized;
        }

        WindowState storedValue = new();
        SetWindowState(storedValue);
        return storedValue;
    }

    public void SetWindowState(WindowState value)
    {
        string jsonValue = JsonSerializer.Serialize(value, SourceGenerationContext.Default.WindowState);
        ApplicationData.Current.LocalSettings.Values[nameof(Settings.WindowState)] = jsonValue;
    }

    public void SetSelectedClient(SupportedClients value)
    {
        if (value == SelectedClienteSource.Value)
            return;

        ApplicationData.Current.LocalSettings.Values[nameof(Settings.SelectedClient)] = JsonSerializer.Serialize(value, SourceGenerationContext.Default.SupportedClients);
        SelectedClienteSource.OnNext(value);
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
        if (ApplicationData.Current.LocalSettings.Values.TryGetValue(nameof(Settings.OnnxChatClientSettings), out object? item) && item is string value
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
        ApplicationData.Current.LocalSettings.Values[nameof(Settings.OnnxChatClientSettings)] = jsonValue;
    }

    public OpenAIClientSettings GetOpenAISettings()
    {
        if (ApplicationData.Current.LocalSettings.Values.TryGetValue(nameof(Settings.OpenAIClientSettings), out object? item) && item is string value
            && JsonSerializer.Deserialize(value, SourceGenerationContext.Default.OpenAIClientSettings) is OpenAIClientSettings deserialized)
        {
            return deserialized;
        }

        OpenAIClientSettings storedValue = new();
        SetOpenAIClientSettings(storedValue);
        return storedValue;
    }

    public void SetOpenAIClientSettings(OpenAIClientSettings value)
    {
        string jsonValue = JsonSerializer.Serialize(value, SourceGenerationContext.Default.OpenAIClientSettings);
        ApplicationData.Current.LocalSettings.Values[nameof(Settings.OpenAIClientSettings)] = jsonValue;
    }
}