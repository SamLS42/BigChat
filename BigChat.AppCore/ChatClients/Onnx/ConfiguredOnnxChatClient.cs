using BigChat.AppCore.Settings;
using BigChat.AppCore.Settings.Onnx;
using Microsoft.Extensions.AI;
using Microsoft.ML.OnnxRuntimeGenAI;

namespace BigChat.AppCore.ChatClients.Onnx;

public sealed partial class ConfiguredOnnxChatClient : IChatClient
{
    private ISettingsService SettingsService { get; } = ServiceLocator.GetRequiredService<ISettingsService>();
    private OnnxRuntimeGenAIChatClient? ChatClient { get; set; }
    private ChatOptions ChatOptions { get; } = new ChatOptions();
    private string OnnxModelDir => SettingsService.GetOnnxChatSettings().OnnxModelDir;
    private string CurrentOnnxModelDir { get; set; } = string.Empty;

    public ConfiguredOnnxChatClient()
    {
        CurrentOnnxModelDir = OnnxModelDir;
    }

    public void Dispose()
    {
        ChatClient?.Dispose();
    }

    private OnnxRuntimeGenAIChatClient GetChatClient()
    {
        if (string.IsNullOrWhiteSpace(OnnxModelDir))
        {
            throw new MissingSettingsException() { SettingName = nameof(OnnxModelDir) };
        }

        if (CurrentOnnxModelDir != OnnxModelDir || ChatClient is null)
        {
            ChatClient?.Dispose();

            CurrentOnnxModelDir = OnnxModelDir;

            OnnxRuntimeGenAIChatClientOptions options = new()
            {
                StopSequences = ConfiguredOnnxChatClientHelpers.template?.Stop ?? [],
                PromptFormatter = (chatMessages, chatOptions) => ConfiguredOnnxChatClientHelpers.GetPrompt(ConfiguredOnnxChatClientHelpers.template, [.. chatMessages], chatOptions),
            };

            ChatClient = new OnnxRuntimeGenAIChatClient(config: new(CurrentOnnxModelDir), ownsConfig: true, options);
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
        return ChatClient is IChatClient client ? client.GetService(serviceType, serviceKey) : null;
    }

    public IAsyncEnumerable<ChatResponseUpdate> GetStreamingResponseAsync(IEnumerable<ChatMessage> messages,
        ChatOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        return GetChatClient().GetStreamingResponseAsync(messages, options ?? GetChatOptions(), cancellationToken);
    }

    private ChatOptions GetChatOptions()
    {
        if (SettingsService.GetOnnxChatSettings() is OnnxChatClientSettings settings)
        {
            ChatOptions.Temperature = (float?)settings.Temperature;
            ChatOptions.MaxOutputTokens = settings.MaxOutputTokens;
            ChatOptions.TopP = (float?)settings.TopP;
            ChatOptions.FrequencyPenalty = (float?)settings.FrequencyPenalty;
            ChatOptions.PresencePenalty = (float?)settings.PresencePenalty;
        }

        return ChatOptions;
    }
}
