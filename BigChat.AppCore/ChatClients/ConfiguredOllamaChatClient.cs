using BigChat.AppCore.Settings;
using BigChat.AppCore.Settings.Ollama;
using Microsoft.Extensions.AI;
using OllamaSharp;

namespace BigChat.AppCore.ChatClients;

internal sealed class ConfiguredOllamaChatClient : IChatClient
{
    private ISettingsService SettingsService { get; } = ServiceLocator.GetRequiredService<ISettingsService>();
    private OllamaApiClient? ChatClient { get; set; }
    private ChatOptions ChatOptions { get; } = new ChatOptions();
    private string Endpoint => SettingsService.GetOllamaChatSettings().Endpoint;
    private string CurrentEndpoint { get; set; } = string.Empty;

    public void Dispose()
    {
        ChatClient?.Dispose();
    }

    private OllamaApiClient GetChatClient()
    {
        if (string.IsNullOrWhiteSpace(Endpoint))
        {
            throw new MissingSettingsException() { SettingName = nameof(Endpoint) };
        }

        if (CurrentEndpoint != Endpoint || ChatClient is null)
        {
            ChatClient?.Dispose();

            ChatClient = new OllamaApiClient(Endpoint);

            CurrentEndpoint = Endpoint;
        }

        return ChatClient;
    }

    public Task<ChatResponse> GetResponseAsync(IEnumerable<ChatMessage> messages,
        ChatOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        return ((IChatClient)GetChatClient()).GetResponseAsync(messages, GetChatOptions(), cancellationToken);
    }

    public object? GetService(Type serviceType, object? serviceKey = null)
    {
        return ChatClient is IChatClient client ? client.GetService(serviceType, serviceKey) : null;
    }

    public IAsyncEnumerable<ChatResponseUpdate> GetStreamingResponseAsync(IEnumerable<ChatMessage> messages,
        ChatOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        return ((IChatClient)GetChatClient()).GetStreamingResponseAsync(messages, options ?? GetChatOptions(), cancellationToken);
    }

    private ChatOptions GetChatOptions()
    {
        if (SettingsService.GetOllamaChatSettings() is OllamaChatClientSettings settings)
        {
            ChatOptions.ModelId = settings.CompletionModel;
            ChatOptions.Temperature = (float?)settings.Temperature;
            ChatOptions.MaxOutputTokens = settings.MaxOutputTokens;
            ChatOptions.TopP = (float?)settings.TopP;
            ChatOptions.FrequencyPenalty = (float?)settings.FrequencyPenalty;
            ChatOptions.PresencePenalty = (float?)settings.PresencePenalty;
        }

        return ChatOptions;
    }
}
