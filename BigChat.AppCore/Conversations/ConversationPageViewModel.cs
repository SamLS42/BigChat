using BigChat.AppCore.Conversations.EventMessages;
using BigChat.AppCore.Localization;
using BigChat.AppCore.Messages;
using BigChat.AppCore.Notifications;
using BigChat.AppCore.Settings;
using BigChat.AppCore.ViewModel;
using BigChat.Infrastructure.Data;
using BigChat.Infrastructure.Data.Models;
using CommunityToolkit.Mvvm.Messaging;
using DynamicData;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.AI;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using System.Globalization;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace BigChat.AppCore.Conversations;

public sealed partial class ConversationViewModel : ReactiveObject, IDisposable
{
    private CompositeDisposable Disposables { get; } = [];
    private SourceCache<MessageViewModel, int> MessageSource { get; } = new(vm => vm.Id);
    public IObservableCache<MessageViewModel, int> Messages => MessageSource.AsObservableCache();
    private IDbContextFactory<MyDbContext> DbContextFactory { get; } = ServiceLocator.GetRequiredService<IDbContextFactory<MyDbContext>>();

    public ConversationViewModel()
    {
        MessageSource.Connect()
            .SubscribeMany(m =>
            {
                return m.ObservableForProperty(x => x.Text)
                    .SkipWhile(m => m.Sender.Id == 0)
                    .DistinctUntilChanged()
                    .Subscribe(async vm => await UpdateMessageAsync(vm.Sender));
            })
            .Subscribe()
            .DisposeWith(Disposables);
    }

    [Reactive]
    public partial string Subject { get; set; } = string.Empty;
    public int Id { get; set; }

    [ReactiveCommand]
    private void Delete()
    {
        WeakReferenceMessenger.Default.Send<DeleteConversationConfirmation>(new(this));
    }

    [ReactiveCommand]
    public void Rename()
    {
        WeakReferenceMessenger.Default.Send<RenameConversationConfirmation>(new(this));
    }

    public override string ToString()
    {
        return Subject;
    }

    private readonly ILocalizedTexts localizedTexts = ServiceLocator.GetRequiredService<ILocalizedTexts>();
    private readonly SubjectResolver subjectResolver = ServiceLocator.GetRequiredService<SubjectResolver>();
    private readonly ConversationProcessor conversationProcessor = ServiceLocator.GetRequiredService<ConversationProcessor>();

    [Reactive]
    public partial string InputText { get; set; } = string.Empty;

    [Reactive]
    public int ConversationId { get; set; }

    [ReactiveCommand(CanExecute = nameof(CanAddMessages))]
    private async Task AddMessageAsync(CancellationToken cancellationToken)
    {
        if (ConversationId == 0)
        {
            ConversationId = await CreateConversationAsync(cancellationToken);
        }

        await AddUserMessageAsync(InputText.Trim(), cancellationToken);

        InputText = string.Empty;

        await ProcessConversationAsync(cancellationToken);
    }

    [ReactiveCommand]
    private async Task LoadHistoryAsync(CancellationToken cancellationToken = default)
    {
        await using MyDbContext db = await DbContextFactory.CreateDbContextAsync(cancellationToken);

        MessageSource.Edit(async updateAction =>
        {
            await foreach (Message message in db.GetRecentMessages(ConversationId, afterId: 0, count: 100))
            {
                MessageViewModel messageViewModel = message.ToMessageViewModel();

                updateAction.AddOrUpdate(messageViewModel);
            }
        });
    }

    private MessageViewModel AddResponse()
    {
        MessageViewModel message = new()
        {
            Role = ChatRole.Assistant,
            Text = string.Empty,
            ConversationId = ConversationId,
            CreatedAt = DateTime.Now,
        };

        MessageSource.AddOrUpdate(message);

        return message;
    }

    private async Task UpdateMessageAsync(MessageViewModel message, CancellationToken cancellationToken = default)
    {
        await using MyDbContext db = await DbContextFactory.CreateDbContextAsync(cancellationToken);

        await db.Messages.Where(m => m.Id == message.Id && m.ConversationId == message.ConversationId)
            .ExecuteUpdateAsync(m => m.SetProperty(m => m.Text, message.Text).SetProperty(m => m.ModifiedAt, DateTime.Now), cancellationToken: cancellationToken);

        // Delete messages after the one updated, the conversation is reset from here
        await db.Messages.Where(m => m.Id > message.Id && m.ConversationId == message.ConversationId)
            .ExecuteDeleteAsync(cancellationToken: cancellationToken);

        MessageSource.RemoveKeys(MessageSource.Keys.Where(k => k > message.Id));

        await ProcessConversationAsync(cancellationToken);
    }

    private async Task ProcessConversationAsync(CancellationToken cancellationToken)
    {
        try
        {
            MessageViewModel message = AddResponse();
            message.Text = await conversationProcessor.GetAIResponseAsync(ConversationId, cancellationToken);
            await CheckSubjectAsync(cancellationToken);
        }
        catch (MissingSettingsException ex)
        {
            ShowNotification message = new(Text: string.Format(CultureInfo.InvariantCulture, localizedTexts.MissingSettingsMessageText, ex.SettingName), Severity.Error);
            WeakReferenceMessenger.Default.Send(message);
        }
    }

    private async Task CheckSubjectAsync(CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(Subject) && Messages.Count >= 1)
        {
            string? subject = await subjectResolver.ResolveSubjectAsync(ConversationId, cancellationToken);
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
            ConversationId = ConversationId,
            CreatedAt = DateTime.UtcNow,
        };

        await db.Messages.AddAsync(message, cancellationToken);

        await db.SaveChangesAsync(cancellationToken);

        MessageViewModel messageViewModel = message.ToMessageViewModel();

        MessageSource.AddOrUpdate(messageViewModel);
    }

    private async Task<int> CreateConversationAsync(CancellationToken cancellationToken)
    {
        await using MyDbContext db = await DbContextFactory.CreateDbContextAsync(cancellationToken);

        Conversation newConversation = new()
        {
            CreatedAt = DateTime.Now,
            Subject = string.Empty,
        };

        await db.Conversations.AddAsync(newConversation, cancellationToken);

        await db.SaveChangesAsync(cancellationToken);

        return newConversation.Id;
    }

    private bool CanAddMessages()
    {
        return !string.IsNullOrWhiteSpace(InputText);
    }

    public void Dispose()
    {
        MessageSource.Dispose();
    }
}
