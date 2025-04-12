using BigChat.AppCore.Notifications;
using BigChat.Main;
using BigChat.Utils;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.WinUI.Behaviors;
using Microsoft.UI.Xaml;

namespace BigChat;

internal sealed partial class MainWindow : Window, IRecipient<ShowNotification>, IDisposable
{
    public MainWindow()
    {
        InitializeComponent();
        ExtendsContentIntoTitleBar = true;
        mainFrame.Navigate(typeof(MainPage));
        WeakReferenceMessenger.Default.Register(this);
    }

    public void Receive(ShowNotification message)
    {
        Notification notification = new()
        {
            Message = message.Text,
            Severity = message.Severity.ToInfoBarSeverity(),
            Duration = TimeSpan.FromSeconds(5),
        };

        NotificationQueue.Show(notification);
    }

    public void Dispose()
    {
        WeakReferenceMessenger.Default.UnregisterAll(this);
    }
}
