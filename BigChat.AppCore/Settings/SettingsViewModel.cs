using BigChat.AppCore.ChatClient;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BigChat.AppCore.Settings;

public sealed partial class SettingsViewModel : ObservableObject
{
    [ObservableProperty] public partial bool OllamaIsOn { get; set; }
    [ObservableProperty] public partial bool AzureAIInferenceIsOn { get; set; }

    public double MaxTemperature => Constants.MaxTemperature;
    public double MinTemperature => Constants.MinTemperature;
    public double MaxTopP => Constants.MaxTopP;
    public double MinTopP => Constants.MinTopP;
    public double MaxFrequencyPenalty => Constants.MaxFrequencyPenalty;
    public double MinFrequencyPenalty => Constants.MinFrequencyPenalty;
    public double MaxPresencePenalty => Constants.MaxPresencePenalty;
    public double MinPresencePenalty => Constants.MinPresencePenalty;

    private ISettingsService SettingsService { get; set; }

    public SettingsViewModel(ISettingsService settingsService)
    {
        ArgumentNullException.ThrowIfNull(settingsService);

        SettingsService = settingsService;

        SetValues();
    }

    private void SetValues()
    {
        SupportedClients selectedClient = SettingsService.GetSelectedClient();
        OllamaIsOn = selectedClient is SupportedClients.Ollama;
        AzureAIInferenceIsOn = selectedClient is SupportedClients.AzureAIInference;
    }


    partial void OnOllamaIsOnChanged(bool oldValue, bool newValue)
    {
        if (newValue && oldValue != newValue)
        {
            SettingsService.SetSelectedClient(SupportedClients.Ollama);
            SetValues();
        }
    }

    partial void OnAzureAIInferenceIsOnChanged(bool oldValue, bool newValue)
    {
        if (newValue && oldValue != newValue)
        {
            SettingsService.SetSelectedClient(SupportedClients.AzureAIInference);
            SetValues();
        }
    }
}
