using BigChat.AppCore.ViewModel;

namespace BigChat.AppCore.Messages;

public interface IMessageControl
{
    MessageViewModel Message { get; set; }
}
