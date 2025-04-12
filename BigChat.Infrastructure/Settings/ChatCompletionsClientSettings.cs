namespace BigChat.Infrastructure.Settings;

public class ChatCompletionsClientSettings
{
    public string? ModelId { get; set; }
    public string? Endpoint { get; set; }
    public string? APIKey { get; set; }
    public float? Temperature { get; set; }
    public int? MaxOutputTokens { get; set; }
    public float? TopP { get; set; }
    public float? FrequencyPenalty { get; set; }
    public float? PresencePenalty { get; set; }
}