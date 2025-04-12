using BigChat.AppCore.ViewModel;

namespace BigChat.AppCore.Conversations.EventMessages;

public sealed record DeleteConversationConfirmation(ConversationViewModel Conversation);
