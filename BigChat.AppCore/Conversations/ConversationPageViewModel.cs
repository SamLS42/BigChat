using BigChat.AppCore.Conversations.EventMessages;
using BigChat.AppCore.Localization;
using BigChat.AppCore.Messages;
using BigChat.AppCore.Notifications;
using BigChat.Infrastructure.Conversations;
using BigChat.Infrastructure.Data;
using BigChat.Infrastructure.Data.Models;
using BigChat.Infrastructure.Settings;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.AI;
using System.Collections.ObjectModel;
using System.Globalization;

namespace BigChat.AppCore.ViewModel;

public sealed partial class ConversationPageViewModel(MyDbContext db,
    IMessageControlSelector messageControlSelector,
    ILocalizedTexts localizedTexts,
    SubjectResolver subjectResolver,
    ConversationProcessor conversationProcessor) : ObservableObject, IDisposable
{
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(AddMessageCommand))]
    public partial string InputText { get; set; } = string.Empty;

    public ConversationViewModel Conversation { get; set; } = null!;

    public ObservableCollection<IMessageControl> Messages { get; } = [];

    public Task<IMessageControl> GetOrCreateMessageControl(MessageViewModel message)
    {
        return Task.FromResult(messageControlSelector.GetControl(message));

        //messageControl.Message = message;

        //return messageControl;
    }

    [RelayCommand(CanExecute = nameof(CanAddMessages), FlowExceptionsToTaskScheduler = true)]
    private async Task AddMessageAsync(CancellationToken cancellationToken)
    {
        if (Conversation.Id == 0)
        {
            Conversation.Id = await CreateConversationAsync(cancellationToken);
            WeakReferenceMessenger.Default.Send(new ConversationAdded(Conversation));
        }

        await AddUserMessageAsync(InputText.Trim(), cancellationToken);

        InputText = string.Empty;

        await ProcessConversationAsync(cancellationToken);
    }

    [RelayCommand]
    private async Task LoadHistoryAsync(CancellationToken cancellationToken = default)
    {
        List<Task<IMessageControl>> messageControls = [];

        await foreach (Message message in db.GetRecentMessages(Conversation.Id, afterId: 0, count: 420).WithCancellation(cancellationToken))
        {
            MessageViewModel messageViewModel = new(message);

            messageViewModel.EditConfirmed += MessageViewModel_EditConfirmed;

            messageControls.Add(GetOrCreateMessageControl(messageViewModel));
        }

        await Task.WhenAll(messageControls);

        foreach (IMessageControl item in messageControls.Select(t => t.Result).OrderBy(m => m.Message.Id))
        {
            Messages.Add(item);
        }
    }

    private void MessageViewModel_EditConfirmed(object? sender, EventArgs e)
    {
        if (sender is MessageViewModel message)
        {
            UpdateMessageCommand.Execute(message);
        }
    }

    private IMessageControl AddResponse()
    {
        NotifyAddCommand();

        MessageViewModel message = new(new()
        {
            Role = ChatRole.Assistant.Value,
            Text = string.Empty,
            ConversationId = Conversation.Id,
            CreatedAt = DateTime.Now,
        });

        IMessageControl control = messageControlSelector.GetControl(message);

        Messages.Add(control);

        return control;
    }

    [RelayCommand(FlowExceptionsToTaskScheduler = true)]
    private async Task UpdateMessageAsync(MessageViewModel message, CancellationToken cancellationToken)
    {
        await db.Messages.Where(m => m.Id == message.Id && m.ConversationId == message.ConversationId)
            .ExecuteUpdateAsync(m => m.SetProperty(m => m.Text, message.Text).SetProperty(m => m.ModifiedAt, DateTime.Now), cancellationToken: cancellationToken);

        // Delete messages after the one updated, the conversation is reset from here
        await db.Messages.Where(m => m.Id > message.Id && m.ConversationId == message.ConversationId)
            .ExecuteDeleteAsync(cancellationToken: cancellationToken);


        foreach (IMessageControl item in (IMessageControl[])[.. Messages.Where(m => m.Message.Id > message.Id)])
        {
            item.Message.EditConfirmed -= MessageViewModel_EditConfirmed;
            Messages.Remove(item);
        }

        await ProcessConversationAsync(cancellationToken);
    }

    private async Task ProcessConversationAsync(CancellationToken cancellationToken)
    {
        try
        {
            IMessageControl control = AddResponse();
            control.Message.Text = await conversationProcessor.GetAIResponseAsync(Conversation.Id, cancellationToken);
            await CheckSubjectAsync(cancellationToken);
        }
        catch (MissingSettingsException ex)
        {
            ShowNotification message = new(Text: string.Format(CultureInfo.InvariantCulture, localizedTexts.MissingSettingsMessageText, ex.SettingName), Severity.Error);
            WeakReferenceMessenger.Default.Send(message);
        }
    }

    private void NotifyAddCommand()
    {
        AddMessageCommand.NotifyCanExecuteChanged();
    }

    private async Task CheckSubjectAsync(CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(Conversation.Subject) && Messages.Count >= 1)
        {
            string? subject = await subjectResolver.ResolveSubjectAsync(Conversation.Id, cancellationToken);
            Conversation.Subject = subject ?? Conversation.Subject;
        }
    }

    private async Task AddUserMessageAsync(string text, CancellationToken cancellationToken)
    {
        Message message = new()
        {
            Text = text,
            Role = ChatRole.User.Value,
            ConversationId = Conversation.Id,
            CreatedAt = DateTime.Now,
        };

        await db.Messages.AddAsync(message, cancellationToken);

        await db.SaveChangesAsync(cancellationToken);

        MessageViewModel messageViewModel = new(message);

        messageViewModel.EditConfirmed += MessageViewModel_EditConfirmed;

        Messages.Add(await GetOrCreateMessageControl(messageViewModel));
    }

    private async Task<int> CreateConversationAsync(CancellationToken cancellationToken)
    {
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
        foreach (IMessageControl message in Messages)
        {
            message.Message.EditConfirmed -= MessageViewModel_EditConfirmed;
        }

        Messages.Clear();
    }
}
