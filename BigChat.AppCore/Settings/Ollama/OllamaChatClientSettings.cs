namespace BigChat.AppCore.Settings.Ollama;

public class OllamaChatClientSettings
{
    public string ModelId { get; set; } = string.Empty;
    public string Endpoint { get; set; } = "http://localhost:11434";
    public double Temperature { get; set; } = Constants.DefaultTemperature;
    public int MaxOutputTokens { get; set; } = Constants.DefaultMaxOutputTokens;
    public double TopP { get; set; } = Constants.DefaultTopP;
    public double FrequencyPenalty { get; set; } = Constants.DefaultFrequencyPenalty;
    public double PresencePenalty { get; set; } = Constants.DefaultPresencePenalty;
}
