using BigChat.AppCore.Conversations;
using BigChat.Infrastructure.Data;
using CommunityToolkit.Mvvm.Messaging;
using DynamicData;
using Microsoft.EntityFrameworkCore;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace BigChat.AppCore.MainPage;

public sealed partial class MainPageViewModel : ReactiveObject,
    IDisposable
{
    private readonly CompositeDisposable Disposables = [];
    private SourceCache<ConversationViewModel, int> ConversationSource { get; } = new(c => c.Id);
    public IObservableCache<ConversationViewModel, int> Conversations => ConversationSource.AsObservableCache();

    private IDbContextFactory<MyDbContext> DbContextFactory { get; } = ServiceLocator.GetRequiredService<IDbContextFactory<MyDbContext>>();

    [Reactive]
    public partial string AutoSuggestBoxText { get; set; } = string.Empty;

    [Reactive]
    public partial ReadOnlyCollection<ConversationViewModel> FilteredConversations { get; set; } = ReadOnlyCollection<ConversationViewModel>.Empty;

    public MainPageViewModel()
    {

        ConversationSource.Connect()
            .MergeMany(c => c.DeleteCommand.Select(_ => c))
            .Subscribe(async c => await DeleteConversationAsync(c))
            .DisposeWith(Disposables);

        ConversationSource.Connect()
            .MergeMany(c => c.RenameCommand.Select(_ => c))
            .Subscribe(async c => await UpdateConversationSubjectAsync(c))
            .DisposeWith(Disposables);
    }

    public void AddConversation(ConversationViewModel conversation)
    {
        ConversationSource.AddOrUpdate(conversation);
    }

    [ReactiveCommand]
    private async Task LoadConversations(CancellationToken cancellationToken = default)
    {
        await using MyDbContext db = await DbContextFactory.CreateDbContextAsync(cancellationToken);

        await foreach (var conversation in db.Conversations.OrderByDescending(c => c.CreatedAt).Select(c => new { c.Id, c.Subject })
            .AsAsyncEnumerable()
            .WithCancellation(cancellationToken))
        {
            ConversationSource.AddOrUpdate(new ConversationViewModel
            {
                Id = conversation.Id,
                Subject = conversation.Subject,
            });
        }
    }

    public void Dispose()
    {
        WeakReferenceMessenger.Default.UnregisterAll(this);
        ConversationSource.Clear();
        Conversations.Dispose();
        Disposables.Dispose();
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
        ConversationSource.Remove(conversation);

        await using MyDbContext db = await DbContextFactory.CreateDbContextAsync(cancellationToken);

        await db.Conversations.Where(c => c.Id == conversation.Id)
            .ExecuteDeleteAsync(cancellationToken: cancellationToken);

        conversation.Dispose();
    }

    [ReactiveCommand]
    private void SelectSuggestedConversation(object chosenSuggestion)
    {
        if (chosenSuggestion is ConversationViewModel conversation)
        {
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

    private static ConversationViewModel emptyConversation = new();

    public ConversationViewModel GetEmptyConversation()
    {
        if (ConversationSource.Items.Contains(emptyConversation))
        {
            emptyConversation = new();
        }

        return emptyConversation;
    }
}
