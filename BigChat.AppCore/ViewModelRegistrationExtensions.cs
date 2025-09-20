using BigChat.AppCore.ChatClient;
using BigChat.AppCore.Conversations;
using BigChat.AppCore.MainPage;
using BigChat.AppCore.Settings;
using BigChat.AppCore.ViewModel;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;

namespace BigChat.AppCore;

public static class ViewModelRegistrationExtensions
{
    public static IServiceCollection AddViewModels(this IServiceCollection serviceCollection)
    {
        return serviceCollection
            .AddKeyedSingleton<IChatClient, ConfiguredChatCompletionsClient>(nameof(ConfiguredChatCompletionsClient))
            .AddKeyedSingleton<IChatClient, ConfiguredOllamaChatClient>(nameof(ConfiguredOllamaChatClient))
            .AddSingleton<ChatClientProvider>()
            .AddSingleton<SubjectResolver>()
            .AddSingleton<ConversationProcessor>()
            .AddTransient<MainPageViewModel>()
            .AddTransient<ConversationViewModel>()
            .AddTransient<SettingsViewModel>()
            .AddTransient<ChatCompletionsSettingsViewModel>()
            .AddTransient<OllamaChatSettingsViewModel>();
    }
}
