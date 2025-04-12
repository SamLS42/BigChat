using BigChat.AppCore.ViewModel;
using CommunityToolkit.Mvvm.Messaging.Messages;

namespace BigChat.Conversations;
internal sealed class ConversationPageChanged(ConversationPageViewModel value) : ValueChangedMessage<ConversationPageViewModel>(value)
{
}
