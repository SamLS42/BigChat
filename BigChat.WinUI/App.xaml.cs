using BigChat.AppCore;
using BigChat.AppCore.ChatClient;
using BigChat.AppCore.Settings;
using BigChat.Infrastructure.Data;
using BigChat.Onnx;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml;
using System.Reactive;
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
            .SelectMany(db => Observable.FromAsync(async () =>
            {
                await db.Database.MigrateAsync();
                await db.DisposeAsync();
                return Unit.Default;
            }))
            .Subscribe(_ =>
            {
                m_window = ServiceLocator.GetRequiredService<MainWindow>();
                m_window.Activate();
            });


        ISettingsService settingsService = ServiceLocator.GetRequiredService<ISettingsService>();

        if (settingsService.GetSelectedClient() == SupportedClients.Onnx)
        {
            //Initialize in the background
            Task.Run(async () => await ServiceLocator.GetRequiredService<OnnxSetupService>().InitializeAsync());
        }
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
                .AddPooledDbContextFactory<MyDbContext>(optionsAction =>
                {
                    string cs = $"Data Source={Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MyDB.db")}";
                    optionsAction.UseSqlite(cs)
                                 .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
                })
                .AddLogging(builder =>
                {
                    builder.SetMinimumLevel(LogLevel.Error);
                    builder.AddEventLog();
                })
                .AddSingleton<OnnxSetupService>()
                .AddPlatformServices()
                .AddCoreServices()
                //.AddEmbeddingServices()
                ;
        }).Build();

        ServiceLocator.SetLocator(_host.Services);
    }
}