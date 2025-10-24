using BigChat.AppCore.ChatClient;
using BigChat.AppCore.Conversations;
using BigChat.AppCore.MainPage;
using BigChat.AppCore.Notifications;
using BigChat.AppCore.Services;
using BigChat.AppCore.Settings;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;

namespace BigChat.AppCore;

public static class RegistrationExtensions
{
    public static IServiceCollection AddCoreServices(this IServiceCollection serviceCollection)
    {
        return serviceCollection
            .AddKeyedSingleton<IChatClient, ConfiguredAzureAIInferenceClient>(nameof(ConfiguredAzureAIInferenceClient))
            .AddKeyedSingleton<IChatClient, ConfiguredOllamaChatClient>(nameof(ConfiguredOllamaChatClient))
            .AddTransient(services =>
            {
                ISettingsService settings = services.GetRequiredService<ISettingsService>();

                return services.GetKeyedService<IChatClient>(nameof(ConfiguredOllamaChatClient))!;

                //return settings.GetSelectedClient() switch
                //{
                //    SupportedClients.AzureAIInference => services.GetKeyedService<IChatClient>(nameof(ConfiguredAzureAIInferenceClient))!,
                //    SupportedClients.Ollama => services.GetKeyedService<IChatClient>(nameof(ConfiguredOllamaChatClient))!,
                //    _ => services.GetKeyedService<IChatClient>(nameof(SupportedClients.Onnx))!
                //};
            })
            .AddSingleton<SubjectResolver>()
            .AddSingleton<NotificationService>()
            .AddSingleton<SettingsViewModel>()
            .AddSingleton<DataService>()
            .AddSingleton<ConversationOperationsService>()
            .AddTransient<MainPageViewModel>()
            .AddTransient<ConversationViewModel>()
            .AddTransient<AzureAIInferenceSettingsViewModel>()
            .AddTransient<OllamaSettingsViewModel>();
    }
}
