using BigChat.AppCore;
using BigChat.AppCore.Localization;
using BigChat.AppCore.Settings;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace BigChat.Settings;

public sealed partial class OllamaSettings : UserControl
{
    private OllamaSettingsViewModel ViewModel { get; } = ServiceLocator.GetRequiredService<OllamaSettingsViewModel>();
    private LocalizedTexts Loc { get; } = ServiceLocator.GetRequiredService<LocalizedTexts>();
    public OllamaSettings()
    {
        InitializeComponent();
    }

}
