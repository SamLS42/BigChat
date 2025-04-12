using System.Collections.ObjectModel;

namespace BigChat.Infrastructure.Data.Models;

public class FileEmbedding
{
    public int Id { get; set; }
    public string Contents { get; set; } = null!;
    public required ReadOnlyCollection<float> ContentsEmbedding { get; set; }
    public DateTime CreatedAt { get; set; }
    public int UserFileId { get; set; }
    public virtual UserFile UserFile { get; set; } = null!;
}
