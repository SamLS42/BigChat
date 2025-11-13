using BigChat.AppCore.Settings;
using BigChat.AppCore.Settings.OpenAI;
using Microsoft.Extensions.AI;
using System.ClientModel;

namespace BigChat.AppCore.ChatClients;

internal sealed class ConfiguredOpenAIClient : IChatClient
{
    private ISettingsService SettingsService { get; } = ServiceLocator.GetRequiredService<ISettingsService>();
    private string CurrentEndpoint { get; set; } = string.Empty;
    private string CurrentApiKey { get; set; } = string.Empty;
    private string CurrentModelId { get; set; } = string.Empty;
    private string? Endpoint => SettingsService.GetOpenAISettings()?.Endpoint;
    private string? Key => SettingsService.GetOpenAISettings()?.APIKey;
    private string? ModelId => SettingsService.GetOpenAISettings()?.ModelId;
    private IChatClient? ChatClient { get; set; }
    private ChatOptions ChatOptions { get; } = new ChatOptions();

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

        if (string.IsNullOrWhiteSpace(ModelId))
        {
            throw new MissingSettingsException() { SettingName = nameof(ModelId) };
        }

        if (CurrentEndpoint != Endpoint || CurrentApiKey != Key || CurrentModelId != ModelId || ChatClient is null)
        {
            ChatClient?.Dispose();

            CurrentEndpoint = Endpoint;
            CurrentApiKey = Key;
            CurrentModelId = ModelId;

            ChatClient = new OpenAI.Chat.ChatClient(
                credential: new ApiKeyCredential(CurrentApiKey),
                model: CurrentModelId,
                options: new OpenAI.OpenAIClientOptions()
                {
                    Endpoint = new($"{CurrentEndpoint}"),
                })
                .AsIChatClient();
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
        if (SettingsService.GetOpenAISettings() is OpenAIClientSettings settings)
        {
            ChatOptions.ModelId = settings.ModelId;
            ChatOptions.Temperature = (float?)settings.Temperature;
            ChatOptions.MaxOutputTokens = settings.MaxOutputTokens;
            ChatOptions.TopP = (float?)settings.TopP;
            ChatOptions.FrequencyPenalty = (float?)settings.FrequencyPenalty;
            ChatOptions.PresencePenalty = (float?)settings.PresencePenalty;
            //#pragma warning disable OPENAI001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
            //            ChatOptions.RawRepresentationFactory = _ => new OpenAI.Chat.ChatCompletionOptions
            //            {
            //                ReasoningEffortLevel = OpenAI.Chat.ChatReasoningEffortLevel.Minimal
            //            };
            //#pragma warning restore OPENAI001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
        }

        return ChatOptions;
    }
}
