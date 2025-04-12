using BigChat.AppCore.Localization;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace BigChat.Main;

internal sealed class DialogService(ILocalizedTexts loc)
{
    public ContentDialog GetConfirmationDialog(XamlRoot xamlRoot, object title, string primaryButtonText, object content)
    {
        return new()
        {
            Title = title,
            PrimaryButtonText = primaryButtonText,
            Content = content,
            XamlRoot = xamlRoot,
            Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style,
            CloseButtonText = loc.CancelText,
            DefaultButton = ContentDialogButton.Primary,
        };
    }
}