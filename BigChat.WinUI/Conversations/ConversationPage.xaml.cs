using BigChat.AppCore.Conversations;
using BigChat.AppCore.ViewModel;
using DynamicData;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace BigChat.Conversations;

internal class ReactiveConversationPage : ReactivePage<ConversationViewModel>;
internal sealed partial class ConversationPage : ReactiveConversationPage
{
    private ReadOnlyObservableCollection<MessageViewModel>? Messages;

    public ConversationPage()
    {
        InitializeComponent();

        this.WhenActivated(d =>
        {
            this.WhenAnyValue(x => x.ViewModel)
                .WhereNotNull()
                .Subscribe(vm => vm!.LoadHistoryCommand.Execute().Subscribe())
                .DisposeWith(d);

            this.WhenAnyValue(x => x.ViewModel!.Messages)
                .WhereNotNull()
                .SelectMany(messages => messages.Connect()
                    .SortBy(m => m.Id)
                    .Bind(out Messages))
                .Subscribe()
                .DisposeWith(d);
        });
    }
}
