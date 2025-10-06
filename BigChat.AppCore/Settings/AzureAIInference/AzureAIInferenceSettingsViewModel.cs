using BigChat.AppCore.Settings.AzureAIInference;
using ReactiveUI;
using ReactiveUI.SourceGenerators;

namespace BigChat.AppCore.Settings;

public partial class AzureAIInferenceSettingsViewModel : ReactiveObject
{
    [Reactive] public partial string Endpoint { get; set; }
    [Reactive] public partial string APIKey { get; set; }
    [Reactive] public partial string ModelId { get; set; }
    [Reactive] public partial double Temperature { get; set; }
    [Reactive] public partial int MaxOutputTokens { get; set; }
    [Reactive] public partial double TopP { get; set; }
    [Reactive] public partial double FrequencyPenalty { get; set; }
    [Reactive] public partial double PresencePenalty { get; set; }

    private AzureAIInferenceClientSettings ChatSettings { get; }

    private ISettingsService SettingsService { get; }

    public AzureAIInferenceSettingsViewModel(ISettingsService settingsService)
    {
        SettingsService = settingsService;

        ChatSettings = SettingsService.GetAzureAIInferenceSettings() ?? new();
        Endpoint = ChatSettings.Endpoint;
        APIKey = ChatSettings.APIKey;
        ModelId = ChatSettings.ModelId;

        LoadSettings();
    }

    private void LoadSettings()
    {
        Temperature = ChatSettings.Temperature;
        MaxOutputTokens = ChatSettings.MaxOutputTokens;
        TopP = ChatSettings.TopP;
        FrequencyPenalty = ChatSettings.FrequencyPenalty;
        PresencePenalty = ChatSettings.PresencePenalty;
    }

    [ReactiveCommand]
    private void RestoreDefaults()
    {
        ChatSettings.Temperature = Constants.DefaultTemperature;
        ChatSettings.MaxOutputTokens = Constants.DefaultMaxOutputTokens;
        ChatSettings.TopP = Constants.DefaultTopP;
        ChatSettings.FrequencyPenalty = Constants.DefaultFrequencyPenalty;
        ChatSettings.PresencePenalty = Constants.DefaultPresencePenalty;

        Save();
    }

    public void Save()
    {
        ChatSettings.Endpoint = Endpoint;
        ChatSettings.APIKey = APIKey;
        ChatSettings.ModelId = ModelId;
        ChatSettings.Temperature = Temperature;
        ChatSettings.MaxOutputTokens = MaxOutputTokens;
        ChatSettings.TopP = TopP;
        ChatSettings.FrequencyPenalty = FrequencyPenalty;
        ChatSettings.PresencePenalty = PresencePenalty;

        SettingsService.SetAzureAIInferenceClientSettings(ChatSettings);

        LoadSettings();
    }
}
