using BigChat.Infrastructure.Settings;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BigChat.AppCore.Settings;
public partial class OllamaChatSettingsViewModel : ObservableObject
{
    [ObservableProperty] public partial string Endpoint { get; set; }
    [ObservableProperty] public partial string ModelId { get; set; }
    [ObservableProperty] public partial double Temperature { get; set; }
    [ObservableProperty] public partial int MaxOutputTokens { get; set; }
    [ObservableProperty] public partial double TopP { get; set; }
    [ObservableProperty] public partial double FrequencyPenalty { get; set; }
    [ObservableProperty] public partial double PresencePenalty { get; set; }

    private OllamaChatClientSettings ChatSettings { get; }

    private ISettingsService SettingsService { get; }

    public OllamaChatSettingsViewModel(ISettingsService settingsService)
    {
        SettingsService = settingsService;

        ChatSettings = SettingsService.GetOllamaChatSettings();

        Endpoint = ChatSettings.Endpoint;
        ModelId = ChatSettings.ModelId ?? string.Empty;

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

    [RelayCommand]
    private void RestoreDefaults()
    {
        Temperature = Constants.DefaultTemperature;
        MaxOutputTokens = Constants.DefaultMaxOutputTokens;
        TopP = Constants.DefaultTopP;
        FrequencyPenalty = Constants.DefaultFrequencyPenalty;
        PresencePenalty = Constants.DefaultPresencePenalty;

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
        ChatSettings.ModelId = ModelId;
        ChatSettings.Temperature = Temperature;
        ChatSettings.MaxOutputTokens = MaxOutputTokens;
        ChatSettings.TopP = TopP;
        ChatSettings.FrequencyPenalty = FrequencyPenalty;
        ChatSettings.PresencePenalty = PresencePenalty;

        if (string.IsNullOrWhiteSpace(ChatSettings.Endpoint))
        {
            ChatSettings.Endpoint = "http://localhost:11434";
        }

        SettingsService.SetOllamaChatClientSettings(ChatSettings);

        LoadSettings();
    }
}
