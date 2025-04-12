using BigChat.AppCore.Settings;
using BigChat.AppCore.ViewModel;
using Microsoft.Extensions.DependencyInjection;

namespace BigChat.AppCore;

public static class ViewModelRegistrationExtensions
{
    public static IServiceCollection AddViewModels(this IServiceCollection serviceCollection)
    {
        return serviceCollection
            .AddTransient<MainPageViewModel>()
            .AddTransient<ConversationPageViewModel>()
            .AddTransient<SettingsViewModel>()
            .AddTransient<ChatCompletionsSettingsViewModel>()
            .AddTransient<OllamaChatSettingsViewModel>();
    }
}
