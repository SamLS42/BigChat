using Microsoft.Extensions.DependencyInjection;

namespace BigChat.AppCore;

public static class ServiceLocator
{
    private static IServiceProvider ServiceProvider { get; set; } = null!;

    public static void SetLocator(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;
    }

    public static T GetRequiredService<T>(string? key = null) where T : notnull
    {
        return key == null
            ? ServiceProvider.GetRequiredService<T>()
            : ServiceProvider.GetRequiredKeyedService<T>(key);
    }
}
