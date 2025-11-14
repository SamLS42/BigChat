using DynamicData.Binding;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using System.Reactive.Linq;

namespace BigChat.AppCore.Settings.OpenAI;

public partial class OpenAISettingsViewModel : ReactiveObject
{
    [Reactive] public partial string Endpoint { get; set; }
    [Reactive] public partial string APIKey { get; set; }
    [Reactive] public partial string ModelId { get; set; }
    [Reactive] public partial double Temperature { get; set; }
    [Reactive] public partial int MaxOutputTokens { get; set; }
    [Reactive] public partial double TopP { get; set; }
    [Reactive] public partial double FrequencyPenalty { get; set; }
    [Reactive] public partial double PresencePenalty { get; set; }
    [Reactive] public partial bool IsSelected { get; set; }

    private OpenAIClientSettings ChatSettings { get; }
    private ISettingsService SettingsService { get; } = ServiceLocator.GetRequiredService<ISettingsService>();
    private bool IsInitialzied { get; set; }
    private bool SuppressSave { get; set; }

    public OpenAISettingsViewModel()
    {
        ChatSettings = SettingsService.GetOpenAISettings();

        Endpoint = ChatSettings.Endpoint;
        APIKey = ChatSettings.APIKey;
        ModelId = ChatSettings.ModelId;

        SettingsService.SelectedClientChanges
            .Select(c => c == ChatClients.SupportedClients.OpenAI)
            .Subscribe(v => IsSelected = v);

        this.WhenAnyValue(x => x.IsSelected)
            .Subscribe(v => UpdateSelectedClient(v));

        LoadSettings();

        this.WhenAnyPropertyChanged(
                nameof(Endpoint),
                nameof(APIKey),
                nameof(ModelId),
                nameof(Temperature),
                nameof(MaxOutputTokens),
                nameof(TopP),
                nameof(FrequencyPenalty),
                nameof(PresencePenalty))
            .Where(_ => !SuppressSave)
            .Where(_ => IsInitialzied)
            .Subscribe(_ => Save());

        IsInitialzied = true;
    }

    private void UpdateSelectedClient(bool v)
    {
        if (v)
        {
            SettingsService.SetSelectedClient(ChatClients.SupportedClients.OpenAI);
            return;
        }
        if (SettingsService.SelectedClient == ChatClients.SupportedClients.OpenAI)
        {
            SettingsService.SetSelectedClient(ChatClients.SupportedClients.Unconfigured);
        }
    }

    private void LoadSettings()
    {
        SuppressSave = true;
        try
        {
            Temperature = ChatSettings.Temperature;
            MaxOutputTokens = ChatSettings.MaxOutputTokens;
            TopP = ChatSettings.TopP;
            FrequencyPenalty = ChatSettings.FrequencyPenalty;
            PresencePenalty = ChatSettings.PresencePenalty;
        }
        finally
        {
            SuppressSave = false;
        }
    }

    [ReactiveCommand]
    private void RestoreDefaults()
    {
        ChatSettings.Temperature = Constants.DefaultTemperature;
        ChatSettings.MaxOutputTokens = Constants.DefaultMaxOutputTokens;
        ChatSettings.TopP = Constants.DefaultTopP;
        ChatSettings.FrequencyPenalty = Constants.DefaultFrequencyPenalty;
        ChatSettings.PresencePenalty = Constants.DefaultPresencePenalty;

        SettingsService.SetOpenAIClientSettings(ChatSettings);

        LoadSettings();
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

        SettingsService.SetOpenAIClientSettings(ChatSettings);
    }
}
