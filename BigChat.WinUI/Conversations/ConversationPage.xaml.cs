using BigChat.AppCore;
using BigChat.AppCore.Conversations;
using BigChat.AppCore.Localization;
using BigChat.AppCore.ViewModel;
using DynamicData;
using Microsoft.Extensions.AI;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Navigation;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using System.Reactive.Linq;
using Windows.System;
using Windows.UI.Core;
using WinRT;

namespace BigChat.Conversations;

internal partial class ReactiveConversationPage : ReactivePage<ConversationViewModel>;
internal sealed partial class ConversationPage : ReactiveConversationPage, IDisposable
{
    private bool IsInitialiazing { get; set; } = true;
    private readonly ReadOnlyObservableCollection<MessageViewModel> Messages;
    private CompositeDisposable Disposables { get; } = [];
    private LocalizedTexts Loc { get; } = ServiceLocator.GetRequiredService<LocalizedTexts>();

    public ConversationPage()
    {
        InitializeComponent();

        this.WhenActivated(d => DispatcherQueue.TryEnqueue(() => InputBox.Focus(Microsoft.UI.Xaml.FocusState.Programmatic)));

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
            .ObserveOn(RxApp.MainThreadScheduler)
            .OnItemAdded(async lastAdd =>
            {
                await Task.Yield();
                MessageListView.UpdateLayout();

                if (lastAdd.Role == ChatRole.User && await ViewModel!.LoadHistoryCommand.CanExecute.FirstAsync())
                {
                    MessageListView.ScrollIntoView(lastAdd, ScrollIntoViewAlignment.Leading);
                }
                else
                {
                    MessageListView.ScrollIntoView(lastAdd, ScrollIntoViewAlignment.Default);
                }
            })
            .Subscribe()
            .DisposeWith(Disposables);

        this.WhenAnyValue(x => x.ViewModel)
            .Select(vm =>
                Observable.Using(() => new CompositeDisposable(),
                    disposables =>
                    {
                        if (vm is null)
                        {
                            return Observable.Empty<Unit>();
                        }

                        this.BindCommand(ViewModel, vm => vm.AddMessageCommand, v => v.SendBtn)
                            .DisposeWith(disposables);
                        this.BindCommand(ViewModel, vm => vm.StopResponseCommand, v => v.StopBtn)
                            .DisposeWith(disposables);

                        this.Bind(ViewModel, vm => vm.InputBoxText, v => v.InputBox.Text)
                            .DisposeWith(disposables);

                        this.OneWayBind(ViewModel, vm => vm.AiIsResponding, v => v.SendBtn.Visibility, v => v ? Microsoft.UI.Xaml.Visibility.Collapsed : Microsoft.UI.Xaml.Visibility.Visible)
                            .DisposeWith(disposables);
                        this.OneWayBind(ViewModel, vm => vm.AiIsResponding, v => v.StopBtn.Visibility, v => v ? Microsoft.UI.Xaml.Visibility.Visible : Microsoft.UI.Xaml.Visibility.Collapsed)
                            .DisposeWith(disposables);

                        return Observable.Never<Unit>();
                    }))
            .Switch()
            .Subscribe()
            .DisposeWith(Disposables);

        IsInitialiazing = false;
    }

    private void InputBox_PreviewKeyDown(object sender, KeyRoutedEventArgs e)
    {
        if (e.Key == VirtualKey.Enter && !InputKeyboardSource.GetKeyStateForCurrentThread(VirtualKey.Shift).HasFlag(CoreVirtualKeyStates.Down))
        {
            e.Handled = true;

            if (ViewModel!.AiIsResponding)
            {
                return;
            }

            ViewModel!.AddMessageCommand.Execute(InputBox.Text).Subscribe();
        }
    }

    public void Focus()
    {
        if (InputBox.IsLoaded)
        {
            DispatcherQueue.TryEnqueue(() => InputBox.Focus(Microsoft.UI.Xaml.FocusState.Programmatic));
            return;
        }

        Observable.FromEventPattern(InputBox, nameof(InputBox.Loaded))
            .Subscribe(_ => DispatcherQueue.TryEnqueue(() => InputBox.Focus(Microsoft.UI.Xaml.FocusState.Programmatic)));
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
