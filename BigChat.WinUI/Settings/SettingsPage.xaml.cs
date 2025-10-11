using BigChat.AppCore;
using BigChat.AppCore.Localization;
using BigChat.AppCore.Settings;
using Microsoft.UI.Xaml.Controls;
using System.ComponentModel;
using WinRT;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace BigChat.Settings;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
internal sealed partial class SettingsPage : Page
{
    private LocalizedTexts Loc { get; } = ServiceLocator.GetRequiredService<LocalizedTexts>();
    private SettingsViewModel ViewModel { get; } = ServiceLocator.GetRequiredService<SettingsViewModel>();

    public SettingsPage()
    {
        InitializeComponent();
    }

    private void SaveChatCompletionsSettings(object? sender, PropertyChangedEventArgs e)
    {
        sender.As<AzureAIInferenceSettingsViewModel>().Save();
    }
}