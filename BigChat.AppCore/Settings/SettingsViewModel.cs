using BigChat.AppCore.ChatClient;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using System.Reactive.Linq;

namespace BigChat.AppCore.Settings;

public sealed partial class SettingsViewModel : ReactiveObject
{
    public SettingsViewModel()
    {
        SupportedClients selectedClient = SettingsService.GetSelectedClient();
        OllamaIsOn = selectedClient is SupportedClients.Ollama;
        AzureAIInferenceIsOn = selectedClient is SupportedClients.AzureAIInference;

        this.WhenAnyValue(x => x.OllamaIsOn)
            .Subscribe(x =>
            {
                if (x)
                {
                    SettingsService.SetSelectedClient(SupportedClients.Ollama);
                    AzureAIInferenceIsOn = false;
                }
            });

        this.WhenAnyValue(x => x.AzureAIInferenceIsOn)
            .Subscribe(x =>
            {
                if (x)
                {
                    SettingsService.SetSelectedClient(SupportedClients.AzureAIInference);
                    OllamaIsOn = false;
                }
            });
    }

    [Reactive]
    public partial bool OllamaIsOn { get; set; }
    [Reactive]
    public partial bool AzureAIInferenceIsOn { get; set; }

    private ISettingsService SettingsService { get; } = ServiceLocator.GetRequiredService<ISettingsService>();
}
