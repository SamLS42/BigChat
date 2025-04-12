using BigChat.Infrastructure.ChatClient;
using BigChat.Infrastructure.Conversations;
using BigChat.Infrastructure.Data;
using BigChat.Infrastructure.Embeddings;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;

namespace BigChat.Infrastructure;

public static class ServiceRegistration
{
    public static IServiceCollection AddServices(this IServiceCollection serviceCollection)
    {
        return serviceCollection
            .AddKeyedSingleton<IChatClient, ConfiguredChatCompletionsClient>(nameof(ConfiguredChatCompletionsClient))
            .AddKeyedSingleton<IChatClient, ConfiguredOllamaChatClient>(nameof(ConfiguredOllamaChatClient))
            .AddSingleton<ChatClientProvider>()
            .AddSingleton<SubjectResolver>()
            .AddSingleton<ConversationProcessor>()
            .AddSingleton<IEmbeddingGenerator<string, Embedding<float>>, SKEmbeddingGenerator>()
            .AddSingleton(services =>
            {
                SqliteConnection connection = new($"Data Source={AppDomain.CurrentDomain.BaseDirectory}\\MyDB.db;");
                connection.EnableExtensions(true);
                connection.LoadExtension($"{AppDomain.CurrentDomain.BaseDirectory}\\vec0.dll");

                return connection;
            })
            .AddDbContextPool<MyDbContext>(SetDbContextOptions)
            .AddPooledDbContextFactory<MyDbContext>(SetDbContextOptions);
    }

    private static void SetDbContextOptions(IServiceProvider services, DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite(services.GetRequiredService<SqliteConnection>(), contextOwnsConnection: true)
        .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
    }
}
