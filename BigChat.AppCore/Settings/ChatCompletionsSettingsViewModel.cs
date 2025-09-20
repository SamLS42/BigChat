using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BigChat.AppCore.Settings;
public partial class ChatCompletionsSettingsViewModel : ObservableObject
{
    [ObservableProperty] public partial string Endpoint { get; set; }
    [ObservableProperty] public partial string APIKey { get; set; }
    [ObservableProperty] public partial string ModelId { get; set; }
    [ObservableProperty] public partial double Temperature { get; set; }
    [ObservableProperty] public partial int MaxOutputTokens { get; set; }
    [ObservableProperty] public partial double TopP { get; set; }
    [ObservableProperty] public partial double FrequencyPenalty { get; set; }
    [ObservableProperty] public partial double PresencePenalty { get; set; }

    private ChatCompletionsClientSettings ChatSettings { get; }

    private ISettingsService SettingsService { get; }

    public ChatCompletionsSettingsViewModel(ISettingsService settingsService)
    {
        SettingsService = settingsService;

        ChatSettings = SettingsService.GetChatCompletionsSettings() ?? new();
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

    [RelayCommand]
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

        SettingsService.SetChatCompletionsClientSettings(ChatSettings);

        LoadSettings();
    }
}
