using Microsoft.UI.Xaml;
using Windows.ApplicationModel.DataTransfer;
using Windows.System;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace BigChat.Settings;

public sealed partial class About
{
    public string Version
    {
        get
        {
            Version version = ProcessInfoHelper.Version;
            return string.Format(null, "{0}.{1}.{2}.{3}", version.Major, version.Minor, version.Build, version.Revision);
        }
    }
    public About()
    {
        InitializeComponent();
    }

    private void toCloneRepoCard_Click(object sender, RoutedEventArgs e)
    {
        DataPackage package = new();
        package.SetText(GitCloneTextBlock.Text);
        Clipboard.SetContent(package);
    }

    private async void bugRequestCard_Click(object sender, RoutedEventArgs e)
    {
        await Launcher.LaunchUriAsync(new Uri("https://github.com/SamLS42/BigChat/issues/new"));
    }
}
