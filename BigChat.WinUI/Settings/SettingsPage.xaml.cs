using BigChat.AppCore.Localization;
using BigChat.AppCore.Settings;
using BigChat.Localization;
using Microsoft.Extensions.DependencyInjection;
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
    private ChatCompletionsSettingsViewModel ChatCompletionsSettings { get; set; } = App.ServiceProvider.GetRequiredService<ChatCompletionsSettingsViewModel>();
    private OllamaChatSettingsViewModel OllamaChatSettings { get; set; } = App.ServiceProvider.GetRequiredService<OllamaChatSettingsViewModel>();
    private LocalizedTexts Loc { get; } = App.ServiceProvider.GetRequiredService<ILocalizedTexts>().As<LocalizedTexts>();
    private SettingsViewModel Settings { get; } = App.ServiceProvider.GetRequiredService<SettingsViewModel>();
    public double MaxTemperature => Constants.MaxTemperature;
    public double MinTemperature => Constants.MinTemperature;
    public double MaxTopP => Constants.MaxTopP;
    public double MinTopP => Constants.MinTopP;
    public double MaxFrequencyPenalty => Constants.MaxFrequencyPenalty;
    public double MinFrequencyPenalty => Constants.MinFrequencyPenalty;
    public double MaxPresencePenalty => Constants.MaxPresencePenalty;
    public double MinPresencePenalty => Constants.MinPresencePenalty;

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