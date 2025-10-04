using BigChat.AppCore;
using BigChat.AppCore.Notifications;
using BigChat.Main;
using BigChat.Utils;
using CommunityToolkit.WinUI.Behaviors;
using Microsoft.UI.Xaml;
using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using WinRT;

namespace BigChat;

internal sealed partial class MainWindow : Window, IDisposable
{
    CompositeDisposable Disposables { get; } = [];
    private NotificationService NotificationService { get; } = ServiceLocator.GetRequiredService<NotificationService>();

    public MainWindow()
    {
        InitializeComponent();
        ExtendsContentIntoTitleBar = true;
        mainFrame.Navigate(typeof(MainPage));

        SetTitleBar(mainFrame.Content.As<MainPage>().PageTitleBar);

        NotificationService.Notifications
            .Subscribe(n =>
            {
                Notification notification = new()
                {
                    Message = n.Text,
                    Severity = n.Severity.ToInfoBarSeverity(),
                    Duration = TimeSpan.FromSeconds(5),
                };

                DispatcherQueue.TryEnqueue(() => NotificationQueue.Show(notification));
            })
            .DisposeWith(Disposables);
    }

    public void Dispose()
    {
        Disposables.Dispose();
    }
}
