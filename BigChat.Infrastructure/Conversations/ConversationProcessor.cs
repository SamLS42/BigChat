using BigChat.Infrastructure.ChatClient;
using BigChat.Infrastructure.Data;
using BigChat.Infrastructure.Data.Models;
using Microsoft.Extensions.AI;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

namespace BigChat.Infrastructure.Conversations;

public class ConversationProcessor(MyDbContext db, ChatClientProvider chatClientProvider)
{
    private ConcurrentDictionary<int, (ResponseMessage response, CancellationTokenSource cts)> RunningJobs { get; } = [];

    public bool TryGetStreamingMessage(int conversationId, [MaybeNullWhen(false)] out ResponseMessage value)
    {
        if (RunningJobs.TryGetValue(conversationId, out (ResponseMessage response, CancellationTokenSource cts) job))
        {
            value = job.response;
            return true;
        }

        value = null;
        return false;
    }

    public ResponseMessage GetStreamingResponse(int conversationId)
    {
        CancellationTokenSource cts = new();

        ResponseMessage message = new() { Text = string.Empty };

        RunningJobs.TryAdd(conversationId, (message, cts));

        return message;
    }

    public async Task ProcessConversationAsync(int conversationId)
    {
        if (RunningJobs.TryGetValue(conversationId, out (ResponseMessage response, CancellationTokenSource cts) job))
        {
            (ResponseMessage response, CancellationToken cancellationToken) = (job.response, job.cts.Token);

            List<ChatMessage> messages = [];

            await foreach (Message m in db.GetRecentMessages(conversationId, 0, 50).WithCancellation(cancellationToken))
            {
                messages.Add(new ChatMessage(new ChatRole(m.Role), m.Text));
            }

            try
            {
                await foreach (ChatResponseUpdate messagePart in chatClientProvider.GetChatClient().GetStreamingResponseAsync(messages, cancellationToken: cancellationToken))
                {
                    response.Text += messagePart;
                }
            }
            finally
            {
                // Even if the processing is cancelled, the text already received will be added to the conversation.
                // Hence the "cancellationToken: default", it won't be cancelled
                if (!string.IsNullOrWhiteSpace(response.Text))
                {
                    Message message = (await db.Messages.AddAsync(new Message
                    {
                        ConversationId = conversationId,
                        CreatedAt = DateTime.UtcNow,
                        Role = ChatRole.Assistant.Value,
                        Text = response.Text,
                    }, cancellationToken: default)).Entity;

                    await db.SaveChangesAsync(cancellationToken: default);

                    response.Id = message.Id;
                }

                response.Completed = true;
                RunningJobs.TryRemove(conversationId, out _);
            }
        }
    }

    public async Task CancelProcessConversationAsync(int conversationId)
    {
        if (RunningJobs.TryRemove(conversationId, out (ResponseMessage response, CancellationTokenSource cts) job))
        {
            await job.cts.CancelAsync();
        }
    }
}
