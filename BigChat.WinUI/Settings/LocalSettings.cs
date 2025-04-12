using BigChat.Infrastructure.Settings;

namespace BigChat.Settings;

internal sealed class LocalSettings
{
    public string? AppTheme { get; set; }
    public string? SelectedClient { get; set; }
    public ChatCompletionsClientSettings? ChatCompletionsClientSettings { get; set; }
    public OllamaChatClientSettings? OllamaChatClientSettings { get; set; }
}
