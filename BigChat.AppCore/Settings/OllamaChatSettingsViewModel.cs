using BigChat.Infrastructure.Settings;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BigChat.AppCore.Settings;
public partial class OllamaChatSettingsViewModel : ObservableObject
{
    [ObservableProperty] public partial string Endpoint { get; set; }
    [ObservableProperty] public partial string ModelId { get; set; }
    [ObservableProperty] public partial double Temperature { get; set; }
    [ObservableProperty] public partial double MaxOutputTokens { get; set; }
    [ObservableProperty] public partial double TopP { get; set; }
    [ObservableProperty] public partial double FrequencyPenalty { get; set; }
    [ObservableProperty] public partial double PresencePenalty { get; set; }

    private OllamaChatClientSettings ChatSettings { get; }

    private ISettingsService SettingsService { get; }

    public OllamaChatSettingsViewModel(ISettingsService settingsService)
    {
        SettingsService = settingsService;

        ChatSettings = SettingsService.GetOllamaChatSettings() ?? new();

        Endpoint = ChatSettings.Endpoint ?? string.Empty;
        ModelId = ChatSettings.ModelId ?? string.Empty;
        Temperature = ChatSettings.Temperature ?? default;
        MaxOutputTokens = ChatSettings.MaxOutputTokens ?? default;
        TopP = ChatSettings.TopP ?? default;
        FrequencyPenalty = ChatSettings.FrequencyPenalty ?? default;
        PresencePenalty = ChatSettings.PresencePenalty ?? default;
    }

    [RelayCommand]
    private void RestoreDefaults()
    {
        Temperature = Constants.DefaultTemperature;
        MaxOutputTokens = Constants.DefaultMaxOutputTokens;
        TopP = Constants.DefaultTopP;
        FrequencyPenalty = Constants.DefaultFrequencyPenalty;
        PresencePenalty = Constants.DefaultPresencePenalty;

        ChatSettings.Temperature = (float?)Constants.DefaultTemperature;
        ChatSettings.MaxOutputTokens = (int?)Constants.DefaultMaxOutputTokens;
        ChatSettings.TopP = (float?)Constants.DefaultTopP;
        ChatSettings.FrequencyPenalty = (float?)Constants.DefaultFrequencyPenalty;
        ChatSettings.PresencePenalty = (float?)Constants.DefaultPresencePenalty;

        Save();
    }

    public void Save()
    {
        ChatSettings.Endpoint = Endpoint;
        ChatSettings.ModelId = ModelId;
        ChatSettings.Temperature = (float?)Temperature;
        ChatSettings.MaxOutputTokens = (int?)MaxOutputTokens;
        ChatSettings.TopP = (float?)TopP;
        ChatSettings.FrequencyPenalty = (float?)FrequencyPenalty;
        ChatSettings.PresencePenalty = (float?)PresencePenalty;

        SettingsService.SetOllamaChatClientSettings(ChatSettings);
    }
}
