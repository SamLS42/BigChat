using BigChat.Infrastructure.Data.Models;

namespace BigChat.AppCore.Conversations;

internal static class Extensions
{
    extension(Conversation conversation)
    {
        internal ConversationViewModel ToConversationViewModel()
        {
            return new(conversation.Id)
            {
                Subject = conversation.Subject,
                CreatedAt = conversation.CreatedAt.ToLocalTime(),
            };
        }
    }
}
