using BigChat.AppCore.ViewModel;

namespace BigChat.AppCore.Navigation;

public interface INavigationService
{
    void RemoveFromBackStack(ConversationViewModel conversation);
    void UpdateBackStackOnNavigation();
    void NavigateOnSelection(object SelectedItem);
    void GoBack();
    void LeaveConversation(ConversationViewModel conversation);
    void OpenEmptyConversation();
    void SelectConversation(ConversationViewModel conversation, bool navigate);
    ConversationViewModel? SelectedConversation { get; set; }
}
