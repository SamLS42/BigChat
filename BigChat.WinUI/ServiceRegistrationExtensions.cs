using BigChat.AppCore.Localization;
using BigChat.AppCore.Messages;
using BigChat.AppCore.Navigation;
using BigChat.Infrastructure.Settings;
using BigChat.Localization;
using BigChat.Main;
using BigChat.Messages;
using BigChat.Settings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

namespace BigChat;

internal static class ServiceRegistrationExtensions
{
    public static IServiceCollection AddPlatformServices(this IServiceCollection serviceCollection)
    {
        return serviceCollection.AddSingleton<ISettingsService, LocalSettingsService>()
            .AddSingleton<IStringLocalizer, StringLocalizer>()
            .AddSingleton<ILocalizedTexts, LocalizedTexts>()
            .AddSingleton<INavigationService, NavigationService>()
            .AddSingleton<DialogService>()
            .AddSingleton<IMessageControlSelector, MessageControlSelector>()
            .AddSingleton<IMessageControlProvider, MessageControlProvider>();
    }
}
