using BigChat.Infrastructure.Settings;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;

namespace BigChat.Infrastructure.ChatClient;
public class ChatClientProvider([FromKeyedServices(nameof(ConfiguredChatCompletionsClient))] IChatClient configuredChatCompletionsClient,
    [FromKeyedServices(nameof(ConfiguredOllamaChatClient))] IChatClient ConfiguredOllamaChatClient,
    ISettingsService settings
    )
{
    public IChatClient GetChatClient()
    {
        return settings.GetSelectedClient() switch
        {
            SupportedClients.Ollama => ConfiguredOllamaChatClient,
            SupportedClients.AzureAIInference => configuredChatCompletionsClient,
            _ => throw new NotSupportedException(nameof(GetChatClient))
        };
    }
}
