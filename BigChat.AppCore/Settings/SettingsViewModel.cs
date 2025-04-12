using BigChat.Infrastructure.ChatClient;
using BigChat.Infrastructure.Settings;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BigChat.AppCore.Settings;
public sealed partial class SettingsViewModel : ObservableObject
{
    [ObservableProperty] public partial bool OllamaIsOn { get; set; }
    [ObservableProperty] public partial bool AzureAIInferenceIsOn { get; set; }

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
