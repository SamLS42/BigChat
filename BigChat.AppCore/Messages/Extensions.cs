using BigChat.AppCore.ViewModel;
using BigChat.Infrastructure.Data.Models;
using Microsoft.Extensions.AI;

namespace BigChat.AppCore.Messages;

public static class MessageExtensions
{
    extension(Message message)
    {
        public MessageViewModel ToMessageViewModel()
        {
            return new MessageViewModel
            {
                Id = message.Id,
                Content = message.Content,
                Role = ChatRole.Parse(message.Role),
                ConversationId = message.ConversationId,
                CreatedAt = message.CreatedAt.ToLocalTime(),
            };
        }
    }
}

public static class ChatRoleExtensions
{
    extension(ChatRole message)
    {
        public static ChatRole Parse(string value)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(value, nameof(value));

            if (value.Equals(ChatRole.Assistant.Value, StringComparison.Ordinal))
                return ChatRole.Assistant;
            if (value.Equals(ChatRole.System.Value, StringComparison.Ordinal))
                return ChatRole.System;
            if (value.Equals(ChatRole.Tool.Value, StringComparison.Ordinal))
                return ChatRole.Tool;
            if (value.Equals(ChatRole.User.Value, StringComparison.Ordinal))
                return ChatRole.User;

            throw new ArgumentOutOfRangeException(value);
        }
    }
}