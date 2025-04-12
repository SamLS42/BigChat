using BigChat.AppCore.Messages;
using BigChat.AppCore.ViewModel;
using Microsoft.Extensions.AI;

namespace BigChat.Messages;
internal sealed class MessageControlSelector : IMessageControlSelector
{
    public IMessageControl GetControl(MessageViewModel message)
    {
        return string.Equals(message.Role, ChatRole.User.Value, StringComparison.OrdinalIgnoreCase)
            ? new UserMessage() { Message = message }
            : new AssistantMessage() { Message = message };
    }
}