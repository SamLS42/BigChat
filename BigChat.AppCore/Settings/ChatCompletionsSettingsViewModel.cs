using BigChat.Infrastructure.Settings;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BigChat.AppCore.Settings;
public partial class ChatCompletionsSettingsViewModel : ObservableObject
{
    [ObservableProperty] public partial string Endpoint { get; set; }
    [ObservableProperty] public partial string APIKey { get; set; }
    [ObservableProperty] public partial string ModelId { get; set; }
    [ObservableProperty] public partial double Temperature { get; set; }
    [ObservableProperty] public partial double MaxOutputTokens { get; set; }
    [ObservableProperty] public partial double TopP { get; set; }
    [ObservableProperty] public partial double FrequencyPenalty { get; set; }
    [ObservableProperty] public partial double PresencePenalty { get; set; }

    private ChatCompletionsClientSettings ChatSettings { get; }

    private ISettingsService SettingsService { get; }

    public ChatCompletionsSettingsViewModel(ISettingsService settingsService)
    {
        SettingsService = settingsService;

        ChatSettings = SettingsService.GetChatCompletionsSettings() ?? new();
        Endpoint = ChatSettings.Endpoint ?? string.Empty;
        APIKey = ChatSettings.APIKey ?? string.Empty;
        ModelId = ChatSettings.ModelId ?? string.Empty;

        LoadSettings();
    }

    private void LoadSettings()
    {
        Temperature = ChatSettings.Temperature ?? Constants.DefaultTemperature;
        MaxOutputTokens = ChatSettings.MaxOutputTokens ?? Constants.DefaultMaxOutputTokens;
        TopP = ChatSettings.TopP ?? Constants.DefaultTopP;
        FrequencyPenalty = ChatSettings.FrequencyPenalty ?? Constants.DefaultFrequencyPenalty;
        PresencePenalty = ChatSettings.PresencePenalty ?? Constants.DefaultPresencePenalty;

        Save();
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
        ChatSettings.APIKey = APIKey;
        ChatSettings.ModelId = ModelId;
        ChatSettings.Temperature = (float?)Temperature;
        ChatSettings.MaxOutputTokens = (int?)MaxOutputTokens;
        ChatSettings.TopP = (float?)TopP;
        ChatSettings.FrequencyPenalty = (float?)FrequencyPenalty;
        ChatSettings.PresencePenalty = (float?)PresencePenalty;

        SettingsService.SetChatCompletionsClientSettings(ChatSettings);
    }
}
