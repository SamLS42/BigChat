using BigChat.AppCore.ViewModel;

namespace BigChat.AppCore.Messages;

public interface IMessageControlProvider
{
    IMessageControl GetOrCreate(MessageViewModel message);
    void CreateEntry(IMessageControl messageControl);
}
