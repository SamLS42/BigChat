using BigChat.Main;
using Microsoft.UI.Xaml;

namespace BigChat;

internal sealed partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        ExtendsContentIntoTitleBar = true;
        mainFrame.Navigate(typeof(MainPage));
    }
}
