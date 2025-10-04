using BigChat.AppCore;
using BigChat.AppCore.Localization;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using ReactiveUI;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Windows.System;
using Windows.UI.Core;

namespace BigChat.Conversations;

internal sealed partial class Empty : Page, IActivatableView
{
    private LocalizedTexts Loc { get; } = ServiceLocator.GetRequiredService<LocalizedTexts>();
    private Subject<string> UserInputSource { get; set; } = new();
    public IObservable<string> UserInputs => UserInputSource.AsObservable();
    public Empty()
    {
        InitializeComponent();

        this.WhenActivated(d => DispatcherQueue.TryEnqueue(() => InputBox.Focus(Microsoft.UI.Xaml.FocusState.Programmatic)));
    }

    private void InputBox_PreviewKeyDown(object sender, KeyRoutedEventArgs e)
    {
        if (e.Key == VirtualKey.Enter && !InputKeyboardSource.GetKeyStateForCurrentThread(VirtualKey.Shift).HasFlag(CoreVirtualKeyStates.Down))
        {
            SendInput();
            e.Handled = true;
        }
    }

    private void SendBtn_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        SendInput();
    }

    private void SendInput()
    {
        UserInputSource.OnNext(InputBox.Text);
        InputBox.Text = string.Empty;
    }
}
