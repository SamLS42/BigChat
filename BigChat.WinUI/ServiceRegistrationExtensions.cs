using BigChat.AppCore.Localization;
using BigChat.AppCore.Messages;
using BigChat.AppCore.Settings;
using BigChat.Localization;
using BigChat.Main;
using BigChat.Settings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

namespace BigChat;

internal static partial class ServiceRegistrationExtensions
{
    public static IServiceCollection AddPlatformServices(this IServiceCollection serviceCollection)
    {
        return serviceCollection.AddSingleton<ISettingsService, SettingsService>()
            .AddSingleton<IStringLocalizer, StringLocalizer>()
            .AddSingleton<LocalizedTexts>()
            .AddSingleton<DialogService>();
    }
}
