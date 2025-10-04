using BigChat.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace BigChat.Infrastructure;

internal sealed class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<MyDbContext>
{
    public MyDbContext CreateDbContext(string[] args)
    {
        DbContextOptionsBuilder<MyDbContext> optionsBuilder = new();

        string cs = $"Data Source={Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MyDB.db")}";
        optionsBuilder.UseSqlite(cs);

        return new MyDbContext(optionsBuilder.Options);
    }
}
