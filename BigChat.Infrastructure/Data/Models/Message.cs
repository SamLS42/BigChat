namespace BigChat.Infrastructure.Data.Models;

public class Message
{
    public int Id { get; set; }
    public string Content { get; set; } = null!;
    public string? ThinkContent { get; set; }
    public string Role { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime? ModifiedAt { get; set; }
    public int ConversationId { get; set; }
    public virtual Conversation Conversation { get; set; } = null!;
}
