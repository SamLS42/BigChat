using BigChat.AppCore;
using BigChat.AppCore.Localization;
using BigChat.AppCore.Settings.OpenAI;
using ReactiveUI;
using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace BigChat.Settings;

public sealed partial class OpenAISettings : ReactiveOpenAISettings
{
    private CompositeDisposable Disposables { get; } = [];
    private LocalizedTexts Loc { get; } = ServiceLocator.GetRequiredService<LocalizedTexts>();
    public OpenAISettings()
    {
        InitializeComponent();

        ViewModel = ServiceLocator.GetRequiredService<OpenAISettingsViewModel>();

        this.Bind(ViewModel, x => x.IsSelected, v => v.IsEnableSwitch.IsOn)
            .DisposeWith(Disposables);

        this.Bind(ViewModel, x => x.Endpoint, v => v.EndpointBox.Text)
            .DisposeWith(Disposables);

        this.Bind(ViewModel, x => x.APIKey, v => v.ApiKeyBox.Password)
            .DisposeWith(Disposables);

        this.Bind(ViewModel, x => x.ModelId, v => v.ModelBox.Text)
            .DisposeWith(Disposables);

        this.Bind(ViewModel, x => x.Temperature, v => v.TemperatureSlider.Value)
            .DisposeWith(Disposables);

        //this.Bind(ViewModel, x => x.CompletionModel, v => v.CompletionModelSelector.SelectedItem)
        //    .DisposeWith(Disposables);

        this.Bind(ViewModel, x => x.MaxOutputTokens, v => v.MaxOutputTokensBox.Text)
            .DisposeWith(Disposables);

        this.Bind(ViewModel, x => x.TopP, v => v.TopPSlider.Value)
            .DisposeWith(Disposables);

        this.Bind(ViewModel, x => x.FrequencyPenalty, v => v.FrequencyPenaltySlider.Value)
            .DisposeWith(Disposables);

        this.Bind(ViewModel, x => x.PresencePenalty, v => v.PresencePenaltySlider.Value)
            .DisposeWith(Disposables);

        this.BindCommand(ViewModel, x => x.RestoreDefaultsCommand, v => v.RestoreDefaultsBtn)
            .DisposeWith(Disposables);
    }

    public void Dispose()
    {
        Disposables.Dispose();
    }
}
public partial class ReactiveOpenAISettings : ReactiveUserControl<OpenAISettingsViewModel>;
