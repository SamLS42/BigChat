using BigChat.AppCore.Localization;
using BigChat.AppCore.MainPage;
using BigChat.AppCore.Messages;
using BigChat.AppCore.Services;
using BigChat.AppCore.ViewModel;
using BigChat.Infrastructure.Data;
using BigChat.Infrastructure.Data.Models;
using DynamicData;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.AI;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using System.ClientModel;
using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace BigChat.AppCore.Conversations;

public sealed partial class ConversationViewModel : ReactiveObject, IDisposable
{
    // Private fields
    IChatClient ChatClient => ServiceLocator.GetRequiredService<IChatClient>();
    private CompositeDisposable Disposables { get; } = [];
    private SourceCache<MessageViewModel, int> MessageSource { get; } = new(vm => vm.Id);
    private IDbContextFactory<MyDbContext> DbContextFactory { get; } = ServiceLocator.GetRequiredService<IDbContextFactory<MyDbContext>>();
    private ConversationOperationsService ConversationOperations { get; } = ServiceLocator.GetRequiredService<ConversationOperationsService>();
    private LocalizedTexts Loc { get; } = ServiceLocator.GetRequiredService<LocalizedTexts>();
    private SubjectResolver SubjectResolver { get; } = ServiceLocator.GetRequiredService<SubjectResolver>();
    private DataService DataService { get; } = ServiceLocator.GetRequiredService<DataService>();
    private CancellationTokenSource StopResponseCts { get; set; } = new();
    private Subject<string> UserInputSource { get; } = new();

    // Public observables
    public IObservableCache<MessageViewModel, int> Messages => MessageSource.AsObservableCache();
    public IObservable<string> UserInputs => UserInputSource.Where(s => !string.IsNullOrWhiteSpace(s)).AsObservable();

    // Reactive properties
    [Reactive]
    public partial string Subject { get; set; } = string.Empty;

    [Reactive]
    public partial int Id { get; set; }

    [Reactive]
    public partial DateTime CreatedAt { get; set; }

    [Reactive]
    public partial bool AiIsResponding { get; set; }

    [Reactive]
    public partial string InputBoxText { get; set; } = string.Empty;

    // Constructor
    public ConversationViewModel(int id)
    {
        Id = id;

        MessageSource.Connect()
            .MergeMany(m => m.MessageUpdated.Select(_ => m))
            .Subscribe(async m => await UpdateMessageAsync(m))
            .DisposeWith(Disposables);
    }

    // Commands
    [ReactiveCommand]
    private void Delete()
    {
        ConversationOperations.RequestDeletion(this);
    }

    [ReactiveCommand]
    public void Rename()
    {
        ConversationOperations.RequestRename(this);
    }

    // Public methods
    public override string ToString()
    {
        return Subject;
    }

    // Private methods
    [ReactiveCommand]
    private async Task LoadHistoryAsync(CancellationToken cancellationToken = default)
    {
        await using MyDbContext db = await DbContextFactory.CreateDbContextAsync(cancellationToken);

        MessageViewModel[] messages = [.. db.Messages.Where(m => m.ConversationId == Id).Select(m => m.ToMessageViewModel())];

        MessageSource.EditDiff(messages, areItemsEqual: (m1, m2) => m1.Id == m2.Id);
    }

    [ReactiveCommand]
    private async Task AddMessageAsync(string inputText, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(inputText))
        {
            return;
        }

        Message message = await DataService.AddMessageAsync(conversationId: Id, chatRole: ChatRole.User, content: inputText, cancellationToken);

        MessageViewModel messageViewModel = message.ToMessageViewModel();

        MessageSource.AddOrUpdate(messageViewModel);

        InputBoxText = string.Empty;

        await AddAIResponseMessageAsync();
    }

    [ReactiveCommand]
    private async Task StopResponseAsync()
    {
        await StopResponseCts.CancelAsync();
        StopResponseCts = new CancellationTokenSource();
    }

    private async Task UpdateMessageAsync(MessageViewModel message, CancellationToken cancellationToken = default)
    {
        await using MyDbContext db = await DbContextFactory.CreateDbContextAsync(cancellationToken);

        await DataService.UpdateMessageAsync(message.Id, message.Content, cancellationToken: cancellationToken);

        // Delete messages after the one updated, the conversation is reset from here
        await db.Messages.Where(m => m.Id > message.Id)
            .ExecuteDeleteAsync(cancellationToken: cancellationToken);

        MessageSource.RemoveKeys(MessageSource.Keys.Where(k => k > message.Id));

        await AddAIResponseMessageAsync();
    }

    private async Task AddAIResponseMessageAsync()
    {
        Observable.FromAsync(CheckSubjectAsync).Subscribe();

        await using MyDbContext db = await DbContextFactory.CreateDbContextAsync();

        ChatMessage[] messages = await db.Messages.Where(m => m.ConversationId == Id)
            .Select(m => new ChatMessage(ChatRole.Parse(m.Role), m.Content))
            .ToArrayAsync();

        Message message = await DataService.AddMessageAsync(conversationId: Id, ChatRole.Assistant);
        MessageViewModel responseMessage = message.ToMessageViewModel();

        responseMessage.IsPending = true;

        MessageSource.AddOrUpdate(responseMessage);

        AiIsResponding = true;

        try
        {
            ChatResponse res = await ChatClient.GetResponseAsync(messages, cancellationToken: StopResponseCts.Token);

            await ChatClient.GetStreamingResponseAsync(messages, cancellationToken: StopResponseCts.Token)
                .ToObservable()
                .ObserveOn(RxApp.MainThreadScheduler)
                .ForEachAsync(update =>
                {
                    if (responseMessage.IsPending)
                    {
                        responseMessage.IsPending = false;
                    }

                    if (!string.IsNullOrEmpty(update.Text))
                    {
                        responseMessage.Content += update.Text;
                    }

                    foreach (TextReasoningContent reasoningContent in update.Contents.OfType<TextReasoningContent>().Where(c => !string.IsNullOrEmpty(c.Text)))
                    {
                        responseMessage.ThinkContent += reasoningContent.Text;
                    }
                });

            await DataService.UpdateMessageAsync(responseMessage.Id, responseMessage.Content, responseMessage.ThinkContent);
        }
        catch (ClientResultException exception)
        {
            responseMessage.IsPending = false;
            responseMessage.Content = exception.Message;
            await db.Messages.Where(m => m.Id == responseMessage.Id).ExecuteDeleteAsync();
        }
        catch (Exception e)
        {
            responseMessage.IsPending = false;
            responseMessage.Content = $"Please check if the Settings are configured, we are getting this error message:\n\n`{e.Message}`";
            await db.Messages.Where(m => m.Id == responseMessage.Id).ExecuteDeleteAsync();
        }
        finally
        {
            AiIsResponding = false;
        }
    }

    private async Task CheckSubjectAsync(CancellationToken cancellationToken = default)
    {
        if ((string.IsNullOrWhiteSpace(Subject) || Subject == Loc.NewChatText) && Messages.Count >= 1)
        {
            string? subject = await SubjectResolver.ResolveSubjectAsync(Id, cancellationToken);
            Subject = subject ?? Subject;
        }
    }

    // Dispose
    public void Dispose()
    {
        MessageSource.Dispose();
        Disposables.Dispose();
    }
}
