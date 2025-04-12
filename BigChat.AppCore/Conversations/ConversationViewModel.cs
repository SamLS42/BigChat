using BigChat.AppCore.Conversations.EventMessages;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace BigChat.AppCore.ViewModel;

public partial class ConversationViewModel : ObservableObject
{
    [ObservableProperty]
    public partial string Subject { get; set; } = string.Empty;
    public int Id { get; set; }

    [RelayCommand]
    private void Delete()
    {
        WeakReferenceMessenger.Default.Send<DeleteConversationConfirmation>(new(this));
    }

    [RelayCommand]
    public void Rename()
    {
        WeakReferenceMessenger.Default.Send<RenameConversationConfirmation>(new(this));
    }

    public override string ToString()
    {
        return Subject;
    }
}
