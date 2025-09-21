using BigChat.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BigChat.Infrastructure;

public static class ServiceRegistration
{
    public static IServiceCollection AddServices(this IServiceCollection serviceCollection)
    {
        return serviceCollection.AddPooledDbContextFactory<MyDbContext>(optionsAction =>
            {
                string cs = $"Data Source={Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MyDB.db")}";
                optionsAction.UseSqlite(cs)
                             .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            });
    }
}
