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

        this.WhenAnyValue(x => x.OllamaIsOn, x => x.AzureAIInferenceIsOn)
            .Where(x => x.Item1 || x.Item2)
            .Subscribe(x =>
            {
                if (x.Item1)
                {
                    SettingsService.SetSelectedClient(SupportedClients.Ollama);
                }
                else if (x.Item2)
                {
                    SettingsService.SetSelectedClient(SupportedClients.AzureAIInference);
                }
            });
    }

    [Reactive]
    public partial bool OllamaIsOn { get; set; }
    [Reactive]
    public partial bool AzureAIInferenceIsOn { get; set; }

    public double MaxTemperature => Constants.MaxTemperature;
    public double MinTemperature => Constants.MinTemperature;
    public double MaxTopP => Constants.MaxTopP;
    public double MinTopP => Constants.MinTopP;
    public double MaxFrequencyPenalty => Constants.MaxFrequencyPenalty;
    public double MinFrequencyPenalty => Constants.MinFrequencyPenalty;
    public double MaxPresencePenalty => Constants.MaxPresencePenalty;
    public double MinPresencePenalty => Constants.MinPresencePenalty;

    private ISettingsService SettingsService { get; } = ServiceLocator.GetRequiredService<ISettingsService>();
}
