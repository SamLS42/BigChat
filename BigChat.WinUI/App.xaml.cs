using BigChat.AppCore;
using BigChat.Infrastructure;
using BigChat.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using System.Diagnostics.CodeAnalysis;

namespace BigChat;

#pragma warning disable CA1515 // Consider making public types internal
public partial class App : Application
#pragma warning restore CA1515 // Consider making public types internal
{
    public static ServiceProvider ServiceProvider { get; private set; } = null!;
    private Window m_window = null!;

    public App()
    {
        InitializeComponent();
        ConfigureServices();
    }

    public T GetService<T>() where T : notnull
    {
        return ServiceProvider.GetRequiredService<T>();
    }

    [UnconditionalSuppressMessage("AOT", "IL3050:Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.", Justification = "<Pending>")]
    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        //ServiceProvider.GetRequiredService<MyDbContext>().Database.EnsureDeleted();
        ServiceProvider.GetRequiredService<MyDbContext>().Database.EnsureCreated();

        m_window = ServiceProvider.GetRequiredService<MainWindow>();
        m_window.Activate();
    }

    private static void ConfigureServices()
    {
        IServiceCollection serviceCollection = new ServiceCollection()
               .AddTransient<MainWindow>()
               .AddServices()
               .AddPlatformServices()
               .AddViewModels()
               .AddMemoryCache(setup =>
               {
                   setup.SizeLimit = 100;
                   setup.TrackLinkedCacheEntries = true;
               })
               ;

        ServiceProvider = serviceCollection.BuildServiceProvider();
    }
}
