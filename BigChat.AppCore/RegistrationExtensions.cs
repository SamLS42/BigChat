using BigChat.AppCore.ChatClients;
using BigChat.AppCore.Conversations;
using BigChat.AppCore.MainPage;
using BigChat.AppCore.Notifications;
using BigChat.AppCore.Services;
using BigChat.AppCore.Settings;
using BigChat.AppCore.Settings.OpenAI;
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
            .AddKeyedSingleton<IChatClient, ConfiguredOpenAIClient>(nameof(ConfiguredOpenAIClient))
            .AddTransient(services =>
            {
                ISettingsService settings = services.GetRequiredService<ISettingsService>();

                return settings.SelectedClient switch
                {
                    SupportedClients.AzureAIInference => services.GetKeyedService<IChatClient>(nameof(ConfiguredAzureAIInferenceClient))!,
                    SupportedClients.Ollama => services.GetKeyedService<IChatClient>(nameof(ConfiguredOllamaChatClient))!,
                    SupportedClients.OpenAI => services.GetKeyedService<IChatClient>(nameof(ConfiguredOpenAIClient))!,
                    _ => services.GetKeyedService<IChatClient>(nameof(SupportedClients.None))!
                };
            })
            .AddSingleton<SubjectResolver>()
            .AddSingleton<NotificationService>()
            .AddSingleton<DataService>()
            .AddSingleton<ConversationOperationsService>()
            .AddTransient<MainPageViewModel>()
            .AddTransient<ConversationViewModel>()
            .AddTransient<AzureAIInferenceSettingsViewModel>()
            .AddTransient<OpenAISettingsViewModel>()
            .AddScoped<OllamaSettingsViewModel>();
    }
}
