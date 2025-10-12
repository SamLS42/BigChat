using BigChat.AppCore.Settings.Ollama;
using DynamicData;
using DynamicData.Binding;
using OllamaSharp;
using OllamaSharp.Models;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using System.Numerics;
using System.Reactive.Linq;

namespace BigChat.AppCore.Settings;

public partial class OllamaSettingsViewModel : ReactiveObject
{
    private bool IsInitialzied { get; set; }
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

        LoadSettings();

        this.WhenAnyPropertyChanged()
            .SkipWhile(_ => !IsInitialzied)
            .Subscribe(_ => Save());
    }

    [ReactiveCommand]
    private async Task LoadModelsAsync()
    {
        using OllamaApiClient ollama = new(Endpoint);

        Model[] models = [.. await ollama.ListLocalModelsAsync()];
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
