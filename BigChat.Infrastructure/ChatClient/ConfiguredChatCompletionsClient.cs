using Azure;
using Azure.AI.Inference;
using BigChat.Infrastructure.Settings;
using Microsoft.Extensions.AI;

namespace BigChat.Infrastructure.ChatClient;

internal sealed class ConfiguredChatCompletionsClient : IChatClient
{
    private ISettingsService SettingsService { get; }
    private int EndpointHash { get; set; }
    private int ApiKeyHash { get; set; }
    private string? Endpoint => SettingsService.GetChatCompletionsSettings()?.Endpoint;
    private string? Key => SettingsService.GetChatCompletionsSettings()?.APIKey;
    private IChatClient? ChatClient { get; set; }
    private ChatOptions ChatOptions { get; } = new ChatOptions();

    public ConfiguredChatCompletionsClient(ISettingsService settingsService)
    {
        SettingsService = settingsService;

        if (Endpoint is not null)
        {
            EndpointHash = StringComparer.Ordinal.GetHashCode(Endpoint);
        }

        if (Key is not null)
        {
            ApiKeyHash = StringComparer.Ordinal.GetHashCode(Key);
        }
    }

    public void Dispose()
    {
        ChatClient?.Dispose();
    }

    private IChatClient GetChatClient()
    {
        if (string.IsNullOrWhiteSpace(Endpoint))
        {
            throw new MissingSettingsException() { SettingName = nameof(Endpoint) };
        }

        if (string.IsNullOrWhiteSpace(Key))
        {
            throw new MissingSettingsException() { SettingName = nameof(Key) };
        }

        if (EndpointHash != StringComparer.Ordinal.GetHashCode(Endpoint) || ApiKeyHash != StringComparer.Ordinal.GetHashCode(Key) || ChatClient is null)
        {
            ChatClient = new ChatClientBuilder(new ChatCompletionsClient(endpoint: new Uri(Endpoint), credential: new AzureKeyCredential(Key)).AsIChatClient())
                .Build();

            EndpointHash = StringComparer.Ordinal.GetHashCode(Endpoint);
            ApiKeyHash = StringComparer.Ordinal.GetHashCode(Key);
        }

        return ChatClient;
    }

    public Task<ChatResponse> GetResponseAsync(IEnumerable<ChatMessage> messages,
        ChatOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        return GetChatClient().GetResponseAsync(messages, options ?? GetChatOptions(), cancellationToken);
    }

    public object? GetService(Type serviceType, object? serviceKey = null)
    {
        return ChatClient?.GetService(serviceType, serviceKey);
    }

    public IAsyncEnumerable<ChatResponseUpdate> GetStreamingResponseAsync(IEnumerable<ChatMessage> messages,
        ChatOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        return GetChatClient().GetStreamingResponseAsync(messages, options ?? GetChatOptions(), cancellationToken);
    }

    private ChatOptions GetChatOptions()
    {
        if (SettingsService.GetChatCompletionsSettings() is ChatCompletionsClientSettings settings)
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
