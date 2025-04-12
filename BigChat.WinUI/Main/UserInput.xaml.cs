using BigChat.AppCore.Localization;
using BigChat.AppCore.ViewModel;
using BigChat.Conversations;
using BigChat.Localization;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Windows.System;
using Windows.UI.Core;
using WinRT;

namespace BigChat.Main;
internal sealed partial class UserInput : UserControl, IRecipient<ConversationPageChanged>, IDisposable
{
    private LocalizedTexts Loc { get; }
    private ConversationPageViewModel Conversation { get; set; } = null!;

    public UserInput()
    {
        Loc = App.ServiceProvider.GetRequiredService<ILocalizedTexts>().As<LocalizedTexts>();
        InitializeComponent();
        WeakReferenceMessenger.Default.Register(this);
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

    public void Receive(ConversationPageChanged message)
    {
        Conversation = message.Value;
        Bindings.Update();
        FocusOnInputBox();
        WeakReferenceMessenger.Default.Cleanup();
    }
    private void InputBox_PreviewKeyDown(object sender, KeyRoutedEventArgs e)
    {
        if (e.Key == VirtualKey.Enter && !InputKeyboardSource.GetKeyStateForCurrentThread(VirtualKey.Shift).HasFlag(CoreVirtualKeyStates.Down))
        {
            if (Conversation.AddMessageCommand.CanExecute(parameter: null))
            {
                Conversation.AddMessageCommand.Execute(parameter: null);
                e.Handled = true;
            }
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
