using BigChat.AppCore;
using BigChat.AppCore.Notifications;
using BigChat.AppCore.Settings;
using BigChat.Infrastructure.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using System.Diagnostics;
using Windows.Graphics;
using Windows.Storage;

namespace BigChat;

public partial class App : Application
{
    private IHost _host = null!;
    private readonly string _dbPath;
    private readonly string _connectionString;

    public App()
    {
        InitializeComponent();

        _dbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "MyDB.db");

        _connectionString = new SqliteConnectionStringBuilder
        {
            DataSource = _dbPath,
            Mode = SqliteOpenMode.ReadWriteCreate,
            Cache = SqliteCacheMode.Private
        }.ToString();

        // ensure DB file can be created using the exact connection string we'll register
        try
        {
            using SqliteConnection conn = new(_connectionString);
            conn.Open();
            conn.Close();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Failed creating/opening DB at '{_dbPath}': {ex}");
            throw;
        }

        ConfigureServices();

        ConfigureExceptionHandler();
    }

    private void ConfigureExceptionHandler()
    {
        AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
        {
            Exception ex = (Exception)e.ExceptionObject;
            ServiceLocator.GetRequiredService<NotificationService>()
                .Send(Severity.Error, ex.Message);
            Debug.WriteLine($"Unhandled exception: {ex}");
        };
    }

    protected override async void OnLaunched(LaunchActivatedEventArgs args)
    {
        IDbContextFactory<MyDbContext> dbFactory = ServiceLocator.GetRequiredService<IDbContextFactory<MyDbContext>>();
        await using MyDbContext db = await dbFactory.CreateDbContextAsync();
        await db.Database.MigrateAsync();
        await db.DisposeAsync();

        LaunchWindow();
    }

    private void ConfigureServices()
    {
        _host = Host.CreateDefaultBuilder().ConfigureServices((_, services) =>
        {
            services.AddLogging(builder =>
            {
                builder.AddDebug();
                builder.SetMinimumLevel(LogLevel.Information);
            });

            //services.AddEmbeddingGenerator(svc =>
            //{
            //    ISettingsService settings = svc.GetRequiredService<ISettingsService>();
            //    OllamaChatClientSettings ollamaSettings = settings.GetOllamaChatSettings();
            //    return new OllamaApiClient(new Uri(ollamaSettings.Endpoint), "embeddinggemma:latest");
            //});

            services.AddTransient<MainWindow>();

            // reuse the single connection string
            services.AddPooledDbContextFactory<MyDbContext>(options =>
            {
                options.UseSqlite(_connectionString)
                       .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            });

            services.AddPlatformServices()
                    .AddCoreServices();
        }).Build();

        ServiceLocator.SetLocator(_host.Services);
    }

    private void LaunchWindow()
    {
        ISettingsService localSetting = ServiceLocator.GetRequiredService<ISettingsService>();
        WindowState windowState = localSetting.GetWindowState();

        MainWindow window = new();
        AppWindow appWindow = window.AppWindow;

        appWindow.Move(new PointInt32(windowState.X, windowState.Y));
        appWindow.Resize(new SizeInt32(windowState.Width, windowState.Height));

        if (windowState.IsMaximized && appWindow.Presenter is OverlappedPresenter presenter)
        {
            presenter.Maximize();
        }

        window.Activate();
    }
}
