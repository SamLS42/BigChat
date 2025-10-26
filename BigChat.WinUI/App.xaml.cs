using BigChat.AppCore;
using BigChat.AppCore.Settings;
using BigChat.AppCore.Settings.Ollama;
using BigChat.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using OllamaSharp;
using System.Reactive;
using System.Reactive.Linq;
using Windows.Graphics;

namespace BigChat;

public partial class App : Application
{
    private IHost _host = null!;

    public App()
    {
        InitializeComponent();
        ConfigureServices();


        //ISettingsService settingsService = ServiceLocator.GetRequiredService<ISettingsService>();
        //if (settingsService.GetSelectedClient() == SupportedClients.Onnx)
        //{
        //    //Initialize in the background
        //    Task.Run(async () => await ServiceLocator.GetRequiredService<OnnxSetupService>().InitializeAsync());
        //}
    }

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {

        IDbContextFactory<MyDbContext> dbContextFactory = ServiceLocator.GetRequiredService<IDbContextFactory<MyDbContext>>();

        Observable.FromAsync(dbContextFactory.CreateDbContextAsync)
            .SelectMany(db => Observable.FromAsync(async () =>
            {
                await db.Database.MigrateAsync();
                await db.DisposeAsync();
                return Unit.Default;
            }))
            .Subscribe(_ => LaunchWindow());
    }

    private void ConfigureServices()
    {
        _host = Host.CreateDefaultBuilder().ConfigureServices((_, services) =>
        {
            // Logging
            services.AddLogging(builder =>
            {
                builder.AddDebug(); // shows in Debug output
                builder.SetMinimumLevel(LogLevel.Information);
            });

            services.AddEmbeddingGenerator(services =>
            {
                ISettingsService settings = services.GetRequiredService<ISettingsService>();
                OllamaChatClientSettings ollamaSettings = settings.GetOllamaChatSettings();
                return new OllamaApiClient(new Uri(ollamaSettings.Endpoint), "embeddinggemma:latest");
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
                //.AddSingleton<OnnxSetupService>()
                .AddPlatformServices()
                .AddCoreServices();
        }).Build();

        ServiceLocator.SetLocator(_host.Services);
    }
    private void LaunchWindow()
    {
        ISettingsService localSetting = ServiceLocator.GetRequiredService<ISettingsService>();
        WindowState windowState = localSetting.GetWindowState();

        MainWindow? window = new();

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