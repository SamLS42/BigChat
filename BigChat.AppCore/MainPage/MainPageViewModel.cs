using BigChat.AppCore.Conversations;
using BigChat.AppCore.Conversations.EventMessages;
using BigChat.Infrastructure.Data;
using CommunityToolkit.Mvvm.Messaging;
using DynamicData;
using Microsoft.EntityFrameworkCore;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace BigChat.AppCore.MainPage;

public sealed partial class MainPageViewModel : ReactiveObject,
    IRecipient<ConversationAdded>,
    IDisposable
{
    private readonly CompositeDisposable Disposables = [];
    private SourceList<ConversationViewModel> ConversationSource { get; } = new();
    public IObservableList<ConversationViewModel> Conversations => ConversationSource.AsObservableList();
    public ConversationViewModel? SelectedConversation { get; set; }

    private ObservableAsPropertyHelper<ConversationViewModel> _selectedConversationPageViewModel = null!;
    public ConversationViewModel SelectedConversationPageViewModel => _selectedConversationPageViewModel.Value;
    public BehaviorSubject<int> ConversationChangedSource { get; set; } = new(0);
    public IObservable<int> ConversationChanged => ConversationChangedSource.AsObservable();
    private IDbContextFactory<MyDbContext> DbContextFactory { get; } = ServiceLocator.GetRequiredService<IDbContextFactory<MyDbContext>>();

    private SourceList<int> BackStack { get; } = new();
    private SourceList<int> ForwardStack { get; } = new();

    [Reactive]
    public partial string AutoSuggestBoxText { get; set; } = string.Empty;

    [Reactive]
    public partial ReadOnlyCollection<ConversationViewModel> FilteredConversations { get; set; } = ReadOnlyCollection<ConversationViewModel>.Empty;

    public MainPageViewModel()
    {
        WeakReferenceMessenger.Default.Register(this);

        this.WhenAnyValue(x => x.SelectedConversation)
            .Subscribe(c =>
            {
                int id = c?.Id ?? 0;
                ConversationChangedSource.OnNext(id);
                BackStack.Add(id);
            })
            .DisposeWith(Disposables);

        _selectedConversationPageViewModel = ConversationChanged
            .Select(id => new ConversationViewModel() { ConversationId = id })
            .ToProperty(this, nameof(SelectedConversationPageViewModel));
    }

    [ReactiveCommand]
    private void OpenNewConversation()
    {
        SelectedConversation = null;
    }

    [ReactiveCommand]
    private async Task LoadConversations(CancellationToken cancellationToken = default)
    {
        await using MyDbContext db = await DbContextFactory.CreateDbContextAsync(cancellationToken);

        await foreach (var conversation in db.Conversations.OrderByDescending(c => c.CreatedAt).Select(c => new { c.Id, c.Subject })
            .AsAsyncEnumerable()
            .WithCancellation(cancellationToken))
        {
            ConversationSource.Add(new ConversationViewModel
            {
                Id = conversation.Id,
                Subject = conversation.Subject,
            });
        }
    }

    public void Receive(ConversationAdded message)
    {
        ArgumentNullException.ThrowIfNull(message);
        ConversationSource.Add(message.NewConversation);
    }

    public void Dispose()
    {
        WeakReferenceMessenger.Default.UnregisterAll(this);
        ConversationSource.Clear();
        Conversations.Dispose();
        Disposables.Dispose();
        _selectedConversationPageViewModel.Dispose();
    }

    [ReactiveCommand]
    private async Task UpdateConversationSubjectAsync(ConversationViewModel conversation, CancellationToken cancellationToken = default)
    {
        await using MyDbContext db = await DbContextFactory.CreateDbContextAsync(cancellationToken);

        await db.Conversations.Where(c => c.Id == conversation.Id)
            .ExecuteUpdateAsync(s => s.SetProperty(c => c.Subject, conversation.Subject), cancellationToken: cancellationToken);
    }

    [ReactiveCommand]
    private async Task DeleteConversationAsync(ConversationViewModel conversation, CancellationToken cancellationToken = default)
    {
        BackStack.Remove(conversation.Id);

        if (conversation == SelectedConversation)
        {
            SelectedConversation = null;
        }

        ConversationSource.Remove(conversation);

        await using MyDbContext db = await DbContextFactory.CreateDbContextAsync(cancellationToken);

        await db.Conversations.Where(c => c.Id == conversation.Id)
            .ExecuteDeleteAsync(cancellationToken: cancellationToken);
    }

    private IObservable<bool> CanGoBack => BackStack.CountChanged.Select(c => c != 0);

    [ReactiveCommand(CanExecute = nameof(CanGoBack))]
    private void GoBack()
    {
        BackStack.RemoveAt(-1);
        int id = BackStack.Items[BackStack.Items.Count - 1];
        SelectedConversation = Conversations.Items.SingleOrDefault(c => c.Id == id);
    }

    [ReactiveCommand]
    private void OpenEmptyConversation()
    {
        SelectedConversation = null;
    }

    [ReactiveCommand]
    private void SelectConversation((ConversationViewModel conversation, bool navigate) parameter)
    {
        BackStack.Add(SelectedConversation?.Id ?? 0);
        SelectedConversation = Conversations.Items.SingleOrDefault(c => c.Id == parameter.conversation.Id);
    }

    [ReactiveCommand]
    private void SelectSuggestedConversation(object chosenSuggestion)
    {
        if (chosenSuggestion is ConversationViewModel conversation)
        {
            SelectConversation((conversation, navigate: true));
            AutoSuggestBoxText = string.Empty;
            FilteredConversations = ReadOnlyCollection<ConversationViewModel>.Empty;
        }
    }

    [ReactiveCommand]
    private void UpdateAutoSuggestBoxText(object SelectedItem)
    {
        AutoSuggestBoxText = SelectedItem is ConversationViewModel conversation
            ? conversation.Subject
            : string.Empty;
    }

    [ReactiveCommand]
    private void FilterConversations()
    {
        FilteredConversations = new([.. Conversations.Items.Where(c => c.Subject.Contains(AutoSuggestBoxText, StringComparison.OrdinalIgnoreCase))]);
    }
}
