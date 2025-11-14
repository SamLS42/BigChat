namespace BigChat.AppCore.Settings.OpenAI;

public class OpenAIClientSettings : BaseAIClientSettings
{
    public string ModelId { get; set; } = string.Empty;
    public string Endpoint { get; set; } = string.Empty;
    public string APIKey { get; set; } = string.Empty;
}
