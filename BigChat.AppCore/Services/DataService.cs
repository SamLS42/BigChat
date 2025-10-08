using BigChat.AppCore.Localization;
using BigChat.Infrastructure.Data;
using BigChat.Infrastructure.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.AI;

namespace BigChat.AppCore.Services;

public class DataService
{
    private IDbContextFactory<MyDbContext> DbContextFactory { get; } = ServiceLocator.GetRequiredService<IDbContextFactory<MyDbContext>>();
    private LocalizedTexts Loc { get; } = ServiceLocator.GetRequiredService<LocalizedTexts>();

    public async Task<Message> AddMessageAsync(int conversationId, ChatRole chatRole, string? content = null, CancellationToken cancellationToken = default)
    {
        await using MyDbContext db = await DbContextFactory.CreateDbContextAsync(cancellationToken);

        Message message = new()
        {
            ConversationId = conversationId,
            CreatedAt = DateTime.UtcNow,
            Role = chatRole.Value,
            Content = content ?? string.Empty,
        };

        await db.Messages.AddAsync(message, cancellationToken);

        await db.SaveChangesAsync(cancellationToken);

        return message;
    }

    public async Task UpdateMessageAsync(int id, string? content = null, string? thinkContent = null, CancellationToken cancellationToken = default)
    {
        await using MyDbContext db = await DbContextFactory.CreateDbContextAsync(cancellationToken);

        await db.Messages
            .Where(t => t.Id == id)
            .ExecuteUpdateAsync(u =>
            {
                if (content is not null)
                {
                    u.SetProperty(t => t.Content, content);
                }
                if (thinkContent is not null)
                {
                    u.SetProperty(t => t.ThinkContent, thinkContent);
                }
                u.SetProperty(t => t.ModifiedAt, DateTime.UtcNow);
            }, cancellationToken: cancellationToken);
    }

    public async Task<Conversation> CreateConversationAsync(CancellationToken cancellationToken = default)
    {
        await using MyDbContext db = await DbContextFactory.CreateDbContextAsync(cancellationToken);

        Conversation newConversation = new()
        {
            CreatedAt = DateTime.UtcNow,
            Subject = Loc.NewChatText,
        };

        await db.Conversations.AddAsync(newConversation, cancellationToken);

        await db.SaveChangesAsync(cancellationToken);

        return newConversation;
    }
}
