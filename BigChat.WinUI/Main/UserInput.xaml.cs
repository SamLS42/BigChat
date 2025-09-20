using BigChat.AppCore;
using BigChat.AppCore.Conversations;
using BigChat.AppCore.Localization;
using BigChat.Localization;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Windows.System;
using Windows.UI.Core;
using WinRT;

namespace BigChat.Main;

internal sealed partial class UserInput : UserControl, IDisposable
{
    private LocalizedTexts Loc { get; } = ServiceLocator.GetRequiredService<ILocalizedTexts>().As<LocalizedTexts>();
    private ConversationViewModel Conversation { get; set; } = null!;

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
            Conversation.AddMessageCommand.Execute().Subscribe();
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
}
