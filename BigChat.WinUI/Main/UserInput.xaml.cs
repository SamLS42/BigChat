using BigChat.AppCore;
using BigChat.AppCore.Localization;
using BigChat.AppCore.MainPage;
using BigChat.Localization;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using ReactiveUI;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Windows.System;
using Windows.UI.Core;
using WinRT;

namespace BigChat.Main;

internal class ReactiveUserInput : ReactiveUserControl<UserInputViewModel>;
internal sealed partial class UserInput : ReactiveUserInput, IDisposable
{
    private LocalizedTexts Loc { get; } = ServiceLocator.GetRequiredService<ILocalizedTexts>().As<LocalizedTexts>();

    private Subject<string> UserInputSource { get; } = new();
    public IObservable<string> UserInputs => UserInputSource.Where(s => !string.IsNullOrWhiteSpace(s)).AsObservable();

    public UserInput()
    {
        InitializeComponent();
    }

    public Visibility IsShowingConversation
    {
        get => (Visibility)GetValue(IsShowingConversationProperty);
        set => SetValue(IsShowingConversationProperty, value);
    }

    public static readonly DependencyProperty IsShowingConversationProperty = DependencyProperty.Register(
        name: nameof(IsShowingConversation),
        propertyType: typeof(Visibility),
        ownerType: typeof(UserInput),
        typeMetadata: new PropertyMetadata(defaultValue: Visibility.Visible));


    private void InputBox_PreviewKeyDown(object sender, KeyRoutedEventArgs e)
    {
        if (e.Key == VirtualKey.Enter && !InputKeyboardSource.GetKeyStateForCurrentThread(VirtualKey.Shift).HasFlag(CoreVirtualKeyStates.Down))
        {
            AddMessage();
            e.Handled = true;
        }
    }

    private void InputBox_Loaded(object _, RoutedEventArgs _2)
    {
        FocusOnInputBox();
    }

    public void FocusOnInputBox()
    {
        DispatcherQueue.TryEnqueue(() => InputBox.Focus(FocusState.Programmatic));
    }

    public void Dispose()
    {
        WeakReferenceMessenger.Default.UnregisterAll(this);
    }

    private void AddMessage(object sender, RoutedEventArgs e)
    {
        AddMessage();
    }

    private void AddMessage()
    {
        UserInputSource.OnNext(InputBox.Text);

        InputBox.Text = string.Empty;
    }
}
