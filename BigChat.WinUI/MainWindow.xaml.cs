using BigChat.AppCore;
using BigChat.AppCore.Notifications;
using BigChat.AppCore.Settings;
using BigChat.Main;
using BigChat.Utils;
using CommunityToolkit.WinUI.Behaviors;
using Microsoft.UI.Xaml;
using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using System.Reactive.Linq;
using Vanara.PInvoke;
using WinRT;
using WinRT.Interop;

namespace BigChat;

internal sealed partial class MainWindow : Window
{
    CompositeDisposable Disposables { get; } = [];
    private NotificationService NotificationService { get; } = ServiceLocator.GetRequiredService<NotificationService>();
    private ISettingsService Settings { get; } = ServiceLocator.GetRequiredService<ISettingsService>();

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


        Observable.FromEventPattern<object, WindowEventArgs>(this, nameof(Closed))
           .Subscribe(_ =>
           {
               SaveWindowState();
               Disposables.Dispose();
           })
           .DisposeWith(Disposables);
    }

    private void SaveWindowState()
    {
        // 1. Get the window's native handle (HWND)
        HWND hwnd = new(WindowNative.GetWindowHandle(this));

        // We'll populate these variables based on the window state
        int x, y, width, height;
        bool isMaximized = false;

        // 2. Get the placement to check if the window is maximized
        User32.WINDOWPLACEMENT placement = new();
        if (User32.GetWindowPlacement(hwnd, ref placement))
        {
            if (placement.showCmd == ShowWindowCommand.SW_SHOWMAXIMIZED)
            {
                // STATE: MAXIMIZED
                // Save the underlying "normal" position for when the user restores.
                isMaximized = true;
                RECT normalRect = placement.rcNormalPosition;
                x = normalRect.X;
                y = normalRect.Y;
                width = normalRect.Width;
                height = normalRect.Height;
            }
            else
            {
                // STATE: NORMAL or SNAPPED
                // Get the window's *actual current* screen coordinates.
                // This will correctly capture the size and position of a snapped window.
                if (User32.GetWindowRect(hwnd, out RECT currentRect))
                {
                    x = currentRect.X;
                    y = currentRect.Y;
                    width = currentRect.Width;
                    height = currentRect.Height;
                }
                else
                {
                    // Fallback or error handling if GetWindowRect fails
                    // For now, we can just return and not save.
                    return;
                }
            }

            // 3. Save the determined state
            WindowState state = new()
            {
                Height = height,
                Width = width,
                X = x,
                Y = y,
                IsMaximized = isMaximized,
            };
            Settings.SetWindowState(state);
        }
    }
}
