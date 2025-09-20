using BigChat.AppCore;
using BigChat.AppCore.Localization;
using BigChat.AppCore.Settings;
using BigChat.Localization;
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
    private ChatCompletionsSettingsViewModel ChatCompletionsSettings { get; set; } = ServiceLocator.GetRequiredService<ChatCompletionsSettingsViewModel>();
    private OllamaChatSettingsViewModel OllamaChatSettings { get; set; } = ServiceLocator.GetRequiredService<OllamaChatSettingsViewModel>();
    private LocalizedTexts Loc { get; } = ServiceLocator.GetRequiredService<ILocalizedTexts>().As<LocalizedTexts>();
    private SettingsViewModel ViewModel { get; } = ServiceLocator.GetRequiredService<SettingsViewModel>();

    public SettingsPage()
    {
        InitializeComponent();
        ChatCompletionsSettings.PropertyChanged += SaveChatCompletionsSettings;
        OllamaChatSettings.PropertyChanged += SaveOllamaChatSettings;
    }

    private void SaveChatCompletionsSettings(object? sender, PropertyChangedEventArgs e)
    {
        sender.As<ChatCompletionsSettingsViewModel>().Save();
    }

    private void SaveOllamaChatSettings(object? sender, PropertyChangedEventArgs e)
    {
        sender.As<OllamaChatSettingsViewModel>().Save();
    }
}