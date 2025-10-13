using BigChat.AppCore.Conversations;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace BigChat.AppCore.MainPage;

internal sealed class ConversationOperationsService
{
    private Subject<ConversationViewModel> DeletionRequestsSource { get; } = new();
    private Subject<ConversationViewModel> RenameRequestsSource { get; } = new();
    public IObservable<ConversationViewModel> DeletionRequests => DeletionRequestsSource.AsObservable();
    public IObservable<ConversationViewModel> RenameRequests => RenameRequestsSource.AsObservable();

    public void RequestDeletion(ConversationViewModel conversation)
    {
        DeletionRequestsSource.OnNext(conversation);
    }

    public void RequestRename(ConversationViewModel conversation)
    {
        RenameRequestsSource.OnNext(conversation);
    }
}
