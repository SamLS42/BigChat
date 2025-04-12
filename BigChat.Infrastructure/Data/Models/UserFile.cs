namespace BigChat.Infrastructure.Data.Models;

public class UserFile
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ModifiedAt { get; set; }
    public virtual ICollection<FileEmbedding> FileEmbeddings { get; init; } = [];
}
