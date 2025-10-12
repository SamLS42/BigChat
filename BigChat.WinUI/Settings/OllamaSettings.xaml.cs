using BigChat.AppCore;
using BigChat.AppCore.Localization;
using BigChat.AppCore.Settings;
using DynamicData;
using OllamaSharp.Models;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using System.Reactive.Linq;

namespace BigChat.Settings;

public sealed partial class OllamaSettings : ReactiveOllamaSettings, IDisposable
{
    private CompositeDisposable Disposables { get; } = [];
    private LocalizedTexts Loc { get; } = ServiceLocator.GetRequiredService<LocalizedTexts>();
    private readonly ReadOnlyObservableCollection<Model> CompletionModels = null!;
    public OllamaSettings()
    {
        InitializeComponent();
        ViewModel = ServiceLocator.GetRequiredService<OllamaSettingsViewModel>();

        this.Bind(ViewModel, x => x.Endpoint, v => v.EndpointBox.Text).DisposeWith(Disposables);
        this.Bind(ViewModel, x => x.Temperature, v => v.TemperatureSlider.Value).DisposeWith(Disposables);
        this.Bind(ViewModel, x => x.CompletionModel, v => v.CompletionModelSelector.SelectedItem).DisposeWith(Disposables);
        this.Bind(ViewModel, x => x.MaxOutputTokens, v => v.MaxOutputTokensBox.Text).DisposeWith(Disposables);
        this.Bind(ViewModel, x => x.TopP, v => v.TopPSlider.Value).DisposeWith(Disposables);
        this.Bind(ViewModel, x => x.FrequencyPenalty, v => v.FrequencyPenaltySlider.Value).DisposeWith(Disposables);
        this.Bind(ViewModel, x => x.PresencePenalty, v => v.PresencePenaltySlider.Value).DisposeWith(Disposables);
        this.BindCommand(ViewModel, x => x.RestoreDefaultsCommand, v => v.RestoreDefaultsBtn).DisposeWith(Disposables);

        ViewModel.LoadModelsCommand.IsExecuting
            .Subscribe(isExecuting =>
            {
                CompletionModelSelector.IsEnabled = !isExecuting;

                LoadCompletionModelsProgress.Visibility = isExecuting
                ? Microsoft.UI.Xaml.Visibility.Visible
                : Microsoft.UI.Xaml.Visibility.Collapsed;
            })
            .DisposeWith(Disposables);

        ViewModel.CompletionModels.Connect()
            .Bind(out CompletionModels)
            .Subscribe()
            .DisposeWith(Disposables);

        ViewModel.LoadModelsCommand.Execute().Subscribe();
    }

    public void Dispose()
    {
        Disposables.Dispose();
    }
}

public partial class ReactiveOllamaSettings : ReactiveUserControl<OllamaSettingsViewModel>;