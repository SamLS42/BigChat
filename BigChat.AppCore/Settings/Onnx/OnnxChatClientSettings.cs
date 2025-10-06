namespace BigChat.AppCore.Settings.Onnx;

public class OnnxChatClientSettings
{
    public string OnnxModelDir { get; set; } = string.Empty;
    public double Temperature { get; set; } = Constants.DefaultTemperature;
    public int MaxOutputTokens { get; set; } = Constants.DefaultMaxOutputTokens;
    public double TopP { get; set; } = Constants.DefaultTopP;
    public double FrequencyPenalty { get; set; } = Constants.DefaultFrequencyPenalty;
    public double PresencePenalty { get; set; } = Constants.DefaultPresencePenalty;
}