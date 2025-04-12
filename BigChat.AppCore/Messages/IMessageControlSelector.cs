using BigChat.AppCore.ViewModel;

namespace BigChat.AppCore.Messages;

public interface IMessageControlSelector
{
    IMessageControl GetControl(MessageViewModel message);
}