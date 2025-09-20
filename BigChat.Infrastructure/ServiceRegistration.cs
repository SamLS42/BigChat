using BigChat.Infrastructure.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BigChat.Infrastructure;

public static class ServiceRegistration
{
    public static IServiceCollection AddServices(this IServiceCollection serviceCollection)
    {
        return serviceCollection
            .AddSingleton(services =>
            {
                SqliteConnection connection = new($"Data Source={AppDomain.CurrentDomain.BaseDirectory}\\MyDB.db;");
                connection.EnableExtensions(true);
                connection.LoadExtension($"{AppDomain.CurrentDomain.BaseDirectory}\\vec0.dll");

                return connection;
            })
            //.AddDbContextPool<MyDbContext>(SetDbContextOptions)
            .AddPooledDbContextFactory<MyDbContext>(SetDbContextOptions);
    }

    private static void SetDbContextOptions(IServiceProvider services, DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite(services.GetRequiredService<SqliteConnection>(), contextOwnsConnection: true)
        .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
    }
}
