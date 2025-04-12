using BigChat.Infrastructure.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace BigChat.Infrastructure.Data;

public class MyDbContext : DbContext
{
    public float VecDistanceCosine(IList<float> a, IList<float> b)
    {
        throw new NotSupportedException(nameof(VecDistanceCosine));
    }

    public static readonly Func<MyDbContext, int, int, int, IAsyncEnumerable<Message>> RecentMessagesQuery =
        EF.CompileAsyncQuery((MyDbContext db, int conversationId, int after, int count) => db.Messages
        .Where(m => m.ConversationId == conversationId && m.Id > after)
        .OrderBy(m => m.Id)
        .Take(count));

    public IAsyncEnumerable<Message> GetRecentMessages(int conversationId, int afterId, int count)
    {
        return RecentMessagesQuery(this, conversationId, afterId, count);
    }

    public MyDbContext()
    {
    }

    public MyDbContext(DbContextOptions<MyDbContext> options) : base(options)
    {
    }

    public virtual DbSet<Conversation> Conversations { get; set; }

    public virtual DbSet<Message> Messages { get; set; }

    public virtual DbSet<UserFile> UserFiles { get; set; }

    public virtual DbSet<FileEmbedding> FileEmbeddings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
#pragma warning disable CA1062 // Validate arguments of public methods
        modelBuilder.Entity<Conversation>(entity =>
        {
            entity.Property(e => e.CreatedAt).HasColumnType("DateTime2");
            entity.Property(e => e.ModifiedAt).HasColumnType("DateTime2");
            entity.Property(e => e.Subject).HasColumnType("NVarChar(255)");
        });
#pragma warning restore CA1062 // Validate arguments of public methods

        modelBuilder.Entity<Message>(entity =>
        {
            entity.Property(e => e.CreatedAt).HasColumnType("DateTime2");
            entity.Property(e => e.ModifiedAt).HasColumnType("DateTime2");
            entity.Property(e => e.Role).HasColumnType("NVarChar(255)");
            entity.Property(e => e.Text).HasColumnType("TEXT");

            entity.HasOne(d => d.Conversation).WithMany(p => p.Messages)
                .HasForeignKey(d => d.ConversationId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<FileEmbedding>(entity =>
        {
            entity.Property(e => e.CreatedAt).HasColumnType("DateTime2");
            entity.Property(e => e.Contents).HasColumnType("TEXT");
            entity.Property(e => e.ContentsEmbedding).HasColumnType("Blob")
                .HasMaxLength(384);

            entity.HasOne(d => d.UserFile).WithMany(p => p.FileEmbeddings)
                .HasForeignKey(d => d.UserFileId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<UserFile>(entity =>
        {
            entity.Property(e => e.CreatedAt).HasColumnType("DateTime2");
            entity.Property(e => e.ModifiedAt).HasColumnType("DateTime2");
            entity.Property(e => e.Name).HasColumnType("NVarChar(255)");
        });

        modelBuilder.HasDbFunction(typeof(MyDbContext).GetMethod(nameof(VecDistanceCosine), [typeof(IList<float>), typeof(IList<float>)])!)
            .HasName("vec_distance_cosine");
    }
}
