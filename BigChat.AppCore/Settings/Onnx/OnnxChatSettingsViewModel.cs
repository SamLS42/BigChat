using BigChat.AppCore.Settings.Onnx;
using ReactiveUI;
using ReactiveUI.SourceGenerators;

namespace BigChat.AppCore.Settings;

public partial class OnnxChatSettingsViewModel : ReactiveObject
{
    [Reactive] public partial string OnnxModelDir { get; set; }
    [Reactive] public partial double Temperature { get; set; }
    [Reactive] public partial int MaxOutputTokens { get; set; }
    [Reactive] public partial double TopP { get; set; }
    [Reactive] public partial double FrequencyPenalty { get; set; }
    [Reactive] public partial double PresencePenalty { get; set; }

    private OnnxChatClientSettings ChatSettings { get; }

    private ISettingsService SettingsService { get; }

    public OnnxChatSettingsViewModel(ISettingsService settingsService)
    {
        SettingsService = settingsService;

        ChatSettings = SettingsService.GetOnnxChatSettings();

        OnnxModelDir = ChatSettings.OnnxModelDir;

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
        ChatSettings.OnnxModelDir = OnnxModelDir;
        ChatSettings.Temperature = Temperature;
        ChatSettings.MaxOutputTokens = MaxOutputTokens;
        ChatSettings.TopP = TopP;
        ChatSettings.FrequencyPenalty = FrequencyPenalty;
        ChatSettings.PresencePenalty = PresencePenalty;

        SettingsService.SetOnnxChatClientSettings(ChatSettings);

        LoadSettings();
    }
}
