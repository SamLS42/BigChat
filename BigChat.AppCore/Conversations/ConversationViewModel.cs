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
using System.Reactive.Disposables.Fluent;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;

namespace BigChat.AppCore.Conversations;

public sealed partial class ConversationViewModel : ReactiveObject, IDisposable
{
    IChatClient ChatClient => ServiceLocator.GetRequiredService<IChatClient>();
    private CompositeDisposable Disposables { get; } = [];
    private SourceCache<MessageViewModel, int> MessageSource { get; } = new(vm => vm.Id);
    public IObservableCache<MessageViewModel, int> Messages => MessageSource.AsObservableCache();
    private IDbContextFactory<MyDbContext> DbContextFactory { get; } = ServiceLocator.GetRequiredService<IDbContextFactory<MyDbContext>>();
    private LocalizedTexts Loc { get; } = ServiceLocator.GetRequiredService<LocalizedTexts>();
    [Reactive]
    public partial bool AiIsResponding { get; set; }
    private CancellationTokenSource StopResponseCts { get; set; } = new();
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

        await AddAIResponseMessage();
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

        await AddAIResponseMessage();
    }

    private async Task AddAIResponseMessage()
    {
        Observable.FromAsync(CheckSubjectAsync).Subscribe();

        MessageViewModel vm = (await CreateAssistantMessageAsync()).ToMessageViewModel();

        MessageSource.AddOrUpdate(vm);

        await using MyDbContext db = await DbContextFactory.CreateDbContextAsync();

        ChatMessage[] messages = await db.Messages.Select(m => new ChatMessage(ChatRole.Parse(m.Role), m.Text))
            .ToArrayAsync();

        AiIsResponding = true;

        try
        {
            StringBuilder stringBuilder = new();

            await ChatClient.GetStreamingResponseAsync(messages, cancellationToken: StopResponseCts.Token)
                .ToObservable()
                .SubscribeOn(RxApp.TaskpoolScheduler)
                .Do(update =>
                {
                    stringBuilder.Append(update.Text);
                });

            vm.Text = stringBuilder.ToString();

            await db.Messages.Where(m => m.Id == vm.Id)
                .ExecuteUpdateAsync(m => m.SetProperty(m => m.Text, vm.Text).SetProperty(m => m.ModifiedAt, DateTime.UtcNow));
        }
        catch (HttpRequestException e)
        {
            //TODO: 
            vm.Text = $"The AI provider appears to not be correctly configured. Error message: {e.Message}";
            await db.Messages.Where(m => m.Id == vm.Id).ExecuteDeleteAsync();
        }
        catch (TaskCanceledException)
        {
        }
        finally
        {
            AiIsResponding = false;
        }
    }

    private async Task<Message> CreateAssistantMessageAsync(CancellationToken cancellationToken = default)
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

        return message;
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

    [ReactiveCommand]
    private async Task StopResponseAsync()
    {
        await StopResponseCts.CancelAsync();

        StopResponseCts = new CancellationTokenSource();
    }

    [Reactive]
    public string InputBoxText { get; set; } = string.Empty;
    private Subject<string> UserInputSource { get; } = new();
    public IObservable<string> UserInputs => UserInputSource.Where(s => !string.IsNullOrWhiteSpace(s)).AsObservable();

    public void Dispose()
    {
        MessageSource.Dispose();
    }
}
