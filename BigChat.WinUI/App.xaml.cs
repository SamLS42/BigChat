using BigChat.AppCore;
using BigChat.Infrastructure;
using BigChat.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml;
using System.Reactive.Linq;

namespace BigChat;

public partial class App : Application
{
    private IHost _host = null!;
    private Window m_window = null!;

    public App()
    {
        InitializeComponent();
        ConfigureServices();

        IDbContextFactory<MyDbContext> dbContextFactory = ServiceLocator.GetRequiredService<IDbContextFactory<MyDbContext>>();

        Observable.FromAsync(dbContextFactory.CreateDbContextAsync)
            .Subscribe(async db =>
            {
                await db.Database.MigrateAsync();
                await db.DisposeAsync();

                m_window = ServiceLocator.GetRequiredService<MainWindow>();
                m_window.Activate();
            });
    }

    private void ConfigureServices()
    {
        _host = Host.CreateDefaultBuilder().ConfigureServices((context, services) =>
        {
            // Logging
            services.AddLogging(builder =>
            {
                builder.AddDebug(); // shows in Debug output
                builder.SetMinimumLevel(LogLevel.Information);
            });

            services.AddTransient<MainWindow>()
                .AddServices()
                .AddLogging(builder =>
                {
                    builder.SetMinimumLevel(LogLevel.Error);
                    builder.AddEventLog();
                })
                .AddPlatformServices()
                .AddViewModels()
                .AddMemoryCache(setup =>
                {
                    setup.SizeLimit = 100;
                    setup.TrackLinkedCacheEntries = true;
                });
        }).Build();

        ServiceLocator.SetLocator(_host.Services);
    }
}