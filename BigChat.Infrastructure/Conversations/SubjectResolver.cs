using BigChat.Infrastructure.ChatClient;
using BigChat.Infrastructure.Data;
using BigChat.Infrastructure.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.AI;
using System.Text.RegularExpressions;

namespace BigChat.Infrastructure.Conversations;

public partial class SubjectResolver(ChatClientProvider chatClientProvider, MyDbContext db)
{
    private const string DetermineSubjectOrder = """
            Based on our conversation so far, please determine the primary subject of our discussion. Once identified, output the subject in exactly the following format (and nothing else):

            SUBJECT: <subject>

            For example, if the subject is "Travel to California", you should output:

            SUBJECT: Travel to California

            Remember: no additional text, commentary, or formatting—exactly one line following the format above is required.
            """;
    private const int ContextLength = 4;

    public async Task<string?> ResolveSubjectAsync(int conversationId, CancellationToken cancellationToken = default)
    {
        List<Message> tempResults = new(ContextLength);

        await foreach (Message item in db.GetRecentMessages(conversationId, afterId: 0, count: ContextLength).WithCancellation(cancellationToken))
        {
            tempResults.Add(item);
        }

        ChatMessage[] LastestMessages = [
            .. tempResults.Select(x => new ChatMessage(role: new ChatRole(x.Role), content: x.Text)),
            new ChatMessage (role : ChatRole.User, content : DetermineSubjectOrder),
        ];

        ChatResponse response = await chatClientProvider.GetChatClient().GetResponseAsync(LastestMessages, cancellationToken: cancellationToken);

        string? subject = SubjectMatcher()
            .Matches(response.Text ?? string.Empty)
            .LastOrDefault()?.Groups["subject"].Value
            .Trim();

        if (subject is not null)
        {
            int modfiedRows = await db.Conversations.Where(c => c.Id == conversationId)
                .ExecuteUpdateAsync(s => s.SetProperty(c => c.Subject, subject), cancellationToken: cancellationToken);

            return subject;
        }

        return null;
    }

    [GeneratedRegex(@"SUBJECT:\s*(?<subject>.+)$", RegexOptions.ExplicitCapture, 500)]
    private static partial Regex SubjectMatcher();
}
