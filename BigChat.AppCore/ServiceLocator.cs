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
            ? ServiceProvider.GetRequiredService<T>() ?? throw new InvalidOperationException($"There is no service of type {typeof(T)}.")
            : ServiceProvider.GetRequiredKeyedService<T>(key) ?? throw new InvalidOperationException($"There is no service of type {typeof(T)} and key {key}.");
    }
}
