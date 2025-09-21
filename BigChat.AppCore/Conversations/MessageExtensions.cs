using BigChat.AppCore.Messages;
using BigChat.AppCore.ViewModel;
using BigChat.Infrastructure.Data.Models;
using Microsoft.Extensions.AI;

namespace BigChat.AppCore.Conversations;

public static class MessageExtensions
{
    extension(Message message)
    {
        public MessageViewModel ToMessageViewModel()
        {
            return new MessageViewModel
            {
                Id = message.Id,
                ConversationId = message.ConversationId,
                Text = message.Text,
                Role = ChatRole.Parse(message.Role),
                CreatedAt = message.CreatedAt,
            };
        }
    }
}
