using BigChat.AppCore.Localization;
using BigChat.AppCore.Messages;
using BigChat.AppCore.ViewModel;
using BigChat.Infrastructure.Data;
using BigChat.Infrastructure.Data.Models;
using DynamicData;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.AI;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace BigChat.AppCore.Conversations;

public sealed partial class ConversationViewModel : ReactiveObject, IDisposable
{
    IChatClient ChatClient => ServiceLocator.GetRequiredService<IChatClient>();
    private CompositeDisposable Disposables { get; } = [];
    private SourceCache<MessageViewModel, int> MessageSource { get; } = new(vm => vm.Id);
    public IObservableCache<MessageViewModel, int> Messages => MessageSource.AsObservableCache();
    private IDbContextFactory<MyDbContext> DbContextFactory { get; } = ServiceLocator.GetRequiredService<IDbContextFactory<MyDbContext>>();
    private ILocalizedTexts Loc { get; } = ServiceLocator.GetRequiredService<ILocalizedTexts>();

    public ConversationViewModel()
    {
        MessageSource.Connect()
            .MergeMany(m => m.MessageUpdated.Select(_ => m))
            .Subscribe(async m => await UpdateMessageAsync(m))
            .DisposeWith(Disposables);
    }

    [Reactive]
    public partial string Subject { get; set; } = string.Empty;

    [Reactive]
    public partial int Id { get; set; }
    public DateTime CreatedAt { get; set; }

    [ReactiveCommand]
    private void Delete() { }

    [ReactiveCommand]
    public void Rename() { }

    public override string ToString()
    {
        return Subject;
    }

    private readonly SubjectResolver subjectResolver = ServiceLocator.GetRequiredService<SubjectResolver>();


    [ReactiveCommand]
    private async Task AddMessageAsync(string inputText, CancellationToken cancellationToken)
    {
        await AddUserMessageAsync(inputText.Trim(), cancellationToken);

        await AddAIResponseMessage(cancellationToken);
    }

    [ReactiveCommand]
    private async Task LoadHistoryAsync(CancellationToken cancellationToken = default)
    {
        await using MyDbContext db = await DbContextFactory.CreateDbContextAsync(cancellationToken);

        MessageViewModel[] messages = [.. db.Messages.Where(m => m.ConversationId == Id).Select(m => m.ToMessageViewModel())];

        MessageSource.EditDiff(messages, areItemsEqual: (m1, m2) => m1.Id == m2.Id);
    }

    private async Task UpdateMessageAsync(MessageViewModel message, CancellationToken cancellationToken = default)
    {
        await using MyDbContext db = await DbContextFactory.CreateDbContextAsync(cancellationToken);

        await db.Messages.Where(m => m.Id == message.Id)
            .ExecuteUpdateAsync(m => m.SetProperty(m => m.Text, message.Text).SetProperty(m => m.ModifiedAt, DateTime.Now), cancellationToken: cancellationToken);

        // Delete messages after the one updated, the conversation is reset from here
        await db.Messages.Where(m => m.Id > message.Id)
            .ExecuteDeleteAsync(cancellationToken: cancellationToken);

        MessageSource.RemoveKeys(MessageSource.Keys.Where(k => k > message.Id));

        await AddAIResponseMessage(cancellationToken);
    }

    private async Task AddAIResponseMessage(CancellationToken cancellationToken)
    {
        Observable.FromAsync(CheckSubjectAsync).Subscribe();

        MessageViewModel vm = await CreateAssistantMessageAsync(cancellationToken);

        MessageSource.AddOrUpdate(vm);

        await using MyDbContext db = await DbContextFactory.CreateDbContextAsync(cancellationToken);

        ChatMessage[] messages = await db.Messages.Select(m => new ChatMessage(ChatRole.Parse(m.Role), m.Text))
            .ToArrayAsync(cancellationToken);

        //The UI gets bloked when using streaming response
        vm.Text = (await ChatClient.GetResponseAsync(messages, cancellationToken: cancellationToken)).Text;

        await db.Messages.Where(m => m.Id == vm.Id && m.Id == vm.Id)
            .ExecuteUpdateAsync(m => m.SetProperty(m => m.Text, vm.Text).SetProperty(m => m.ModifiedAt, DateTime.Now), cancellationToken: cancellationToken);
    }

    private async Task<MessageViewModel> CreateAssistantMessageAsync(CancellationToken cancellationToken)
    {
        await using MyDbContext db = await DbContextFactory.CreateDbContextAsync(cancellationToken);

        Message message = new()
        {
            ConversationId = Id,
            CreatedAt = DateTime.UtcNow,
            Role = ChatRole.Assistant.Value,
            Text = string.Empty
        };

        await db.Messages.AddAsync(message, cancellationToken);

        await db.SaveChangesAsync(cancellationToken);

        MessageViewModel vm = message.ToMessageViewModel();
        return vm;
    }

    private async Task CheckSubjectAsync(CancellationToken cancellationToken = default)
    {
        if ((string.IsNullOrWhiteSpace(Subject) || Subject == Loc.NewChatText) && Messages.Count >= 1)
        {
            string? subject = await subjectResolver.ResolveSubjectAsync(Id, cancellationToken);
            Subject = subject ?? Subject;
        }
    }

    private async Task AddUserMessageAsync(string text, CancellationToken cancellationToken)
    {
        await using MyDbContext db = await DbContextFactory.CreateDbContextAsync(cancellationToken);

        Message message = new()
        {
            Text = text,
            Role = ChatRole.User.Value,
            ConversationId = Id,
            CreatedAt = DateTime.UtcNow,
        };

        await db.Messages.AddAsync(message, cancellationToken);

        await db.SaveChangesAsync(cancellationToken);

        MessageViewModel messageViewModel = message.ToMessageViewModel();

        MessageSource.AddOrUpdate(messageViewModel);
    }

    public void Dispose()
    {
        MessageSource.Dispose();
    }
}
