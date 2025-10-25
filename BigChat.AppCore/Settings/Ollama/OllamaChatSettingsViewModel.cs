using BigChat.AppCore.Settings.Ollama;
using DynamicData;
using DynamicData.Binding;
using OllamaSharp;
using OllamaSharp.Models;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using System.Reactive.Linq;

namespace BigChat.AppCore.Settings;

public partial class OllamaSettingsViewModel : ReactiveObject
{
    private bool IsInitialzied { get; set; }
    [Reactive] public partial OllamaState OllamaState { get; set; }
    [Reactive] public partial string Endpoint { get; set; }
    [Reactive] public partial Model? CompletionModel { get; set; }
    [Reactive] public partial double Temperature { get; set; }
    [Reactive] public partial int MaxOutputTokens { get; set; }
    [Reactive] public partial double TopP { get; set; }
    [Reactive] public partial double FrequencyPenalty { get; set; }
    [Reactive] public partial double PresencePenalty { get; set; }
    private SourceList<Model> CompletionModelsSource { get; } = new();
    public IObservableList<Model> CompletionModels => CompletionModelsSource.AsObservableList();
    private OllamaChatClientSettings ChatSettings { get; }
    private ISettingsService SettingsService { get; } = ServiceLocator.GetRequiredService<ISettingsService>();

    public OllamaSettingsViewModel()
    {
        ChatSettings = SettingsService.GetOllamaChatSettings();

        Endpoint = ChatSettings.Endpoint;

        this.WhenAnyValue(x => x.Endpoint)
            .Where(e => !string.IsNullOrWhiteSpace(e))
            .Throttle(TimeSpan.FromMilliseconds(1000))
            .ObserveOn(RxApp.MainThreadScheduler)
            .SelectMany(_ => Observable.FromAsync(CheckAvailabilityAsync))
            .Subscribe();

        LoadSettings();

        this.WhenAnyPropertyChanged()
            .SkipWhile(_ => !IsInitialzied)
            .Subscribe(_ => Save());
    }

    private async Task CheckAvailabilityAsync(CancellationToken cancellationToken = default)
    {
        OllamaState = OllamaState.Checking;

        using OllamaApiClient ollama = new(Endpoint);

        try
        {
            await ollama.GetVersionAsync(cancellationToken);
            OllamaState = OllamaState.Available;
            LoadModelsCommand.Execute().Subscribe();
        }
        catch (HttpRequestException)
        {
            OllamaState = OllamaState.NotAvailable;
            return;
        }
    }

    [ReactiveCommand]
    private async Task LoadModelsAsync(CancellationToken cancellationToken = default)
    {
        using OllamaApiClient ollama = new(Endpoint);

        Model[] models = [.. await ollama.ListLocalModelsAsync(cancellationToken)];
        using SemaphoreSlim semaphore = new(initialCount: 8);

        IEnumerable<Task<(Model Model, string[] Capabilities)>> tasks = models.Select(async model =>
        {
            await semaphore.WaitAsync();
            try
            {
                ShowModelResponse info = await ollama.ShowModelAsync(new() { Model = model.Name });
                return (Model: model, Capabilities: info.Capabilities ?? []);
            }
            finally
            {
                semaphore.Release();
            }
        });

        (Model Model, string[] Capabilities)[] results = await Task.WhenAll(tasks);

        Model[] completionModels = [.. results
            .Where(r => r.Capabilities.Contains(Constants.CapabilityCompletion))
            .Select(r => r.Model)];

        CompletionModelsSource.Edit(list =>
        {
            list.Clear();
            list.AddRange(completionModels);
        });

        CompletionModel = CompletionModelsSource.Items.SingleOrDefault(n => n.Name == ChatSettings.CompletionModel);

        IsInitialzied = true;
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

        SettingsService.SetOllamaChatClientSettings(ChatSettings);

        LoadSettings();
    }

    private void Save()
    {
        ChatSettings.Endpoint = Endpoint;
        ChatSettings.CompletionModel = CompletionModel?.Name ?? string.Empty;
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

public enum OllamaState
{
    Checking,
    Available,
    NotAvailable
}