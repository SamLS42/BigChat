using BigChat.AppCore;
using BigChat.AppCore.Localization;
using BigChat.AppCore.Settings;
using ReactiveUI;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace BigChat.Settings;

public sealed partial class AzureAIInferenceSettings : ReactiveAzureAIInferenceSettings
{
    private LocalizedTexts Loc { get; } = ServiceLocator.GetRequiredService<LocalizedTexts>();
    public AzureAIInferenceSettings()
    {
        InitializeComponent();

        ViewModel = ServiceLocator.GetRequiredService<AzureAIInferenceSettingsViewModel>();
    }
}
public partial class ReactiveAzureAIInferenceSettings : ReactiveUserControl<AzureAIInferenceSettingsViewModel>;
