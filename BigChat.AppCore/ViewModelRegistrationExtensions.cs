using BigChat.AppCore.ChatClient;
using BigChat.AppCore.Conversations;
using BigChat.AppCore.MainPage;
using BigChat.AppCore.Notifications;
using BigChat.AppCore.Settings;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;

namespace BigChat.AppCore;

public static class ViewModelRegistrationExtensions
{
    public static IServiceCollection AddCoreServices(this IServiceCollection serviceCollection)
    {
        return serviceCollection
            .AddKeyedSingleton<IChatClient, ConfiguredAzureAIInferenceClient>(nameof(ConfiguredAzureAIInferenceClient))
            .AddKeyedSingleton<IChatClient, ConfiguredOllamaChatClient>(nameof(ConfiguredOllamaChatClient))
            .AddTransient(services =>
            {
                ISettingsService settings = services.GetRequiredService<ISettingsService>();

                return settings.GetSelectedClient() switch
                {
                    SupportedClients.AzureAIInference => services.GetKeyedService<IChatClient>(nameof(ConfiguredAzureAIInferenceClient))!,
                    SupportedClients.Ollama => services.GetKeyedService<IChatClient>(nameof(ConfiguredOllamaChatClient))!,
                    _ => services.GetKeyedService<IChatClient>(nameof(SupportedClients.Onnx))!
                };
            })
            .AddSingleton<SubjectResolver>()
            .AddSingleton<NotificationService>()
            .AddTransient<MainPageViewModel>()
            .AddTransient<ConversationViewModel>()
            .AddSingleton<SettingsViewModel>()
            .AddTransient<AzureAIInferenceSettingsViewModel>()
            .AddTransient<OllamaChatSettingsViewModel>();
    }
}
