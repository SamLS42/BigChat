using BigChat.Infrastructure.ChatClient;
using BigChat.Infrastructure.Data;
using BigChat.Infrastructure.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.AI;

namespace BigChat.Infrastructure.Conversations;

public class ConversationProcessor(IDbContextFactory<MyDbContext> dbContextFactory, ChatClientProvider chatClientProvider)
{
    public async Task<string> GetAIResponseAsync(int conversationId, CancellationToken cancellationToken)
    {
        List<ChatMessage> messages = [];

        await using MyDbContext db = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        await foreach (Message m in db.GetRecentMessages(conversationId, 0, 50).WithCancellation(cancellationToken))
        {
            messages.Add(new ChatMessage(new ChatRole(m.Role), m.Text));
        }

        string response = (await chatClientProvider.GetChatClient().GetResponseAsync(messages, cancellationToken: cancellationToken)).Text;

        if (!string.IsNullOrWhiteSpace(response))
        {
            Message message = (await db.Messages.AddAsync(new Message
            {
                ConversationId = conversationId,
                CreatedAt = DateTime.UtcNow,
                Role = ChatRole.Assistant.Value,
                Text = response,
            }, cancellationToken: default)).Entity;

            await db.SaveChangesAsync(cancellationToken: default);
        }

        return response;
    }
}
