namespace BigChat.Infrastructure.Data.Models;

public class Conversation
{
    public int Id { get; set; }
    public string Subject { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime? ModifiedAt { get; set; }
    public virtual ICollection<Message> Messages { get; init; } = [];
}
