namespace BigChat.AppCore.Settings;

public static class Constants
{
    public static double MinTemperature { get; }
    public static double MinTopP { get; }
    public static double MinFrequencyPenalty { get; } = -2;
    public static double MinPresencePenalty { get; } = -2;
    public static double MaxTemperature { get; } = 2;
    public static double MaxTopP { get; } = 2;
    public static double MaxFrequencyPenalty { get; } = 2;
    public static double MaxPresencePenalty { get; } = 2;
    public static double DefaultTemperature { get; } = 1;
    public static int DefaultMaxOutputTokens { get; } = 8192;
    public static double DefaultTopP { get; } = 1;
    public static double DefaultFrequencyPenalty { get; }
    public static double DefaultPresencePenalty { get; }
    public static string CapabilityCompletion { get; } = "completion";
    public static string CapabilityTools { get; } = "tools";
    public static string CapabilityInsert { get; } = "insert";
    public static string CapabilityVision { get; } = "vision";
    public static string CapabilityEmbedding { get; } = "embedding";
    public static string CapabilityThinking { get; } = "thinking";
}