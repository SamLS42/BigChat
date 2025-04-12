using BigChat.Infrastructure.Settings;
using Microsoft.Extensions.AI;

namespace BigChat.Infrastructure.ChatClient;
internal sealed class ConfiguredOllamaChatClient : IChatClient
{
    private ISettingsService SettingsService { get; }
    private OllamaChatClient? ChatClient { get; set; }
    private ChatOptions ChatOptions { get; } = new ChatOptions();
    private string? Endpoint => SettingsService.GetOllamaChatSettings()?.Endpoint;
    private int EndpointHash { get; set; }

    public ConfiguredOllamaChatClient(ISettingsService settingsService)
    {
        SettingsService = settingsService;

        if (Endpoint is not null)
        {
            EndpointHash = StringComparer.Ordinal.GetHashCode(Endpoint);
        }
    }

    public void Dispose()
    {
        ChatClient?.Dispose();
    }

    private OllamaChatClient GetChatClient()
    {
        if (string.IsNullOrWhiteSpace(Endpoint))
        {
            throw new MissingSettingsException() { SettingName = nameof(Endpoint) };
        }

        if (EndpointHash != StringComparer.Ordinal.GetHashCode(Endpoint) || ChatClient is null)
        {
            ChatClient = new OllamaChatClient(Endpoint);

            EndpointHash = StringComparer.Ordinal.GetHashCode(Endpoint);
        }

        return ChatClient;
    }

    public Task<ChatResponse> GetResponseAsync(IEnumerable<ChatMessage> messages,
        ChatOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        return GetChatClient().GetResponseAsync(messages, GetChatOptions(), cancellationToken);
    }

    public object? GetService(Type serviceType, object? serviceKey = null)
    {
        if (ChatClient is IChatClient client)
        {
            return client.GetService(serviceType, serviceKey);
        }
        return null;
    }

    public IAsyncEnumerable<ChatResponseUpdate> GetStreamingResponseAsync(IEnumerable<ChatMessage> messages,
        ChatOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        return GetChatClient().GetStreamingResponseAsync(messages, GetChatOptions(), cancellationToken);
    }

    private ChatOptions GetChatOptions()
    {
        if (SettingsService.GetOllamaChatSettings() is OllamaChatClientSettings settings)
        {
            ChatOptions.ModelId = settings.ModelId;
            ChatOptions.Temperature = settings.Temperature;
            ChatOptions.MaxOutputTokens = settings.MaxOutputTokens;
            ChatOptions.TopP = settings.TopP;
            ChatOptions.FrequencyPenalty = settings.FrequencyPenalty;
            ChatOptions.PresencePenalty = settings.PresencePenalty;
        }

        return ChatOptions;
    }
}
