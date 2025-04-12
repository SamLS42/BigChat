using BigChat.Infrastructure.Data;
using BigChat.Infrastructure.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;

namespace BigChat.Infrastructure.ChatClient;
public class AiTools(IDbContextFactory<MyDbContext> dbContextFactory)
{
    [Description("Gets the list of conversation in the application")]
    public async Task<Conversation[]> GetConversationList(CancellationToken cancellationToken)
    {
        await using MyDbContext db = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        return await db.Conversations.Select(c => new Conversation
        {
            Subject = c.Subject,
            CreatedAt = c.CreatedAt,
        }).ToArrayAsync(cancellationToken: cancellationToken);
    }
}
