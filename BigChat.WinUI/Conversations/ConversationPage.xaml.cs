using BigChat.AppCore.Conversations;
using BigChat.AppCore.ViewModel;
using DynamicData;
using Microsoft.UI.Xaml.Navigation;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using System.Reactive.Linq;
using WinRT;

namespace BigChat.Conversations;

internal class ReactiveConversationPage : ReactivePage<ConversationViewModel>;
internal sealed partial class ConversationPage : ReactiveConversationPage, IDisposable
{
    private ReadOnlyObservableCollection<MessageViewModel>? Messages;
    private CompositeDisposable Disposables { get; } = [];

    public ConversationPage()
    {
        InitializeComponent();

        this.WhenAnyValue(x => x.ViewModel)
            .Select(vm =>
            {
                if (vm is null)
                {
                    return Observable.Empty<IChangeSet<MessageViewModel, int>>();
                }

                return Observable.Defer(() =>
                {
                    vm.LoadHistoryCommand.Execute().Subscribe();
                    return vm.Messages!.Connect().SortBy(x => x.Id);
                });
            })
            .Switch()
            .Bind(out Messages)
            .Subscribe()
            .DisposeWith(Disposables);
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        ViewModel = e.Parameter.As<ConversationViewModel>();
        base.OnNavigatedTo(e);
    }

    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
        Dispose();
        base.OnNavigatedFrom(e);
    }

    public void Dispose()
    {
        Disposables.Dispose();
    }
}
