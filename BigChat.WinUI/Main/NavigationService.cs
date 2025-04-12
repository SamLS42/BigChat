using BigChat.AppCore.Navigation;
using BigChat.AppCore.ViewModel;
using BigChat.Conversations;
using BigChat.Settings;
using DynamicData;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Navigation;
using System.Collections.ObjectModel;

namespace BigChat.Main;
internal sealed class NavigationService : INavigationService
{
    private Frame Frame { get; set; } = null!;
    private NavigationView NavigationView { get; set; } = null!;
    private NavigationViewItem EmptyConversation { get; set; } = null!;
    private Func<ReadOnlyObservableCollection<ChatNavigationViewItem>> GetItems { get; set; } = null!;

    public void Setup(
        Frame frame,
        NavigationView navigationView,
        NavigationViewItem emptyConversation,
        Func<ReadOnlyObservableCollection<ChatNavigationViewItem>> getItems
        )
    {
        Frame = frame;
        NavigationView = navigationView;
        EmptyConversation = emptyConversation;
        GetItems = getItems;
    }

    private object? SelectedMenuItem { get; set; }
    public ConversationViewModel? SelectedConversation { get; set; }
    public void RemoveFromBackStack(ConversationViewModel conversation)
    {
        Frame.BackStack.RemoveMany(Frame.BackStack.Where(entry => ReferenceEquals(entry.Parameter, conversation)));
    }

    public void UpdateBackStackOnNavigation()
    {
        if (Frame.CanGoBack)
        {
            PageStackEntry entry = Frame.BackStack[^1];

            if (entry.Parameter is ConversationViewModel conversation && conversation.Id == 0)
            {
                Frame.BackStack.Remove(entry);
                return;
            }

            if (Frame.BackStack.Count > 1 &&
                ReferenceEquals(entry.Parameter, Frame.BackStack[^2].Parameter))
            {
                Frame.BackStack.Remove(entry);
                return;
            }
        }
    }

    public void NavigateOnSelection(object SelectedItem)
    {
        if (ReferenceEquals(SelectedMenuItem, SelectedItem))
        {
            return;
        }

        if (SelectedItem is not ConversationViewModel)
        {
            SelectedConversation = null;
        }

        SelectedMenuItem = SelectedItem;

        if (ReferenceEquals(SelectedItem, NavigationView.SettingsItem))
        {
            Frame.Navigate(typeof(SettingsPage), parameter: null, new EntranceNavigationTransitionInfo());
            return;
        }

        if (ReferenceEquals(SelectedItem, EmptyConversation) || SelectedItem is null)
        {
            SelectedConversation = new();
        }

        if (SelectedItem is ChatNavigationViewItem conversation)
        {
            if (ReferenceEquals(conversation.Conversation, SelectedConversation))
            {
                return;
            }
            SelectedConversation = conversation.Conversation;
        }

        if (SelectedConversation is not null)
        {
            Frame.Navigate(typeof(ConversationPage), parameter: SelectedConversation, new SuppressNavigationTransitionInfo());
            return;
        }

        throw new NotSupportedException($"Can't navigate to {nameof(SelectedItem)}");
    }

    public void GoBack()
    {
        if (Frame.CanGoBack)
        {
            PageStackEntry entry = Frame.BackStack[^1];

            Frame.GoBack();

            if (entry.SourcePageType.Name.Equals(nameof(SettingsPage), StringComparison.Ordinal))
            {
                SelectedMenuItem = NavigationView.SettingsItem;
                NavigationView.SelectedItem = SelectedMenuItem;
            }
            else if (entry.Parameter is ConversationViewModel conversation)
            {
                SelectConversation(conversation, navigate: false);
            }
        }
    }

    public void LeaveConversation(ConversationViewModel conversation)
    {
        if (ReferenceEquals(conversation, (SelectedMenuItem as ChatNavigationViewItem)?.Conversation))
        {
            OpenEmptyConversation();
        }
    }

    public void OpenEmptyConversation()
    {
        NavigationView.DispatcherQueue.TryEnqueue(() => NavigationView.SelectedItem = EmptyConversation);
    }

    public void SelectConversation(ConversationViewModel conversation, bool navigate)
    {
        ChatNavigationViewItem? selectedConversation = GetItems.Invoke().SingleOrDefault(c => c.Conversation == conversation);

        if (!navigate)
        {
            SelectedMenuItem = selectedConversation;
        }

        NavigationView.DispatcherQueue.TryEnqueue(() => NavigationView.SelectedItem = selectedConversation);
    }
}
