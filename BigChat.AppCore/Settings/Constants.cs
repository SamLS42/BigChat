namespace BigChat.AppCore.Settings;

public static class Constants
{
    public static double MinTemperature { get; } //=0
    public static double MinTopP { get; } //=0
    public static double MinFrequencyPenalty { get; } = -2;
    public static double MinPresencePenalty { get; } = -2;
    public static double MaxTemperature { get; } = 2;
    public static double MaxTopP { get; } = 2;
    public static double MaxFrequencyPenalty { get; } = 2;
    public static double MaxPresencePenalty { get; } = 2;
    public static double DefaultTemperature { get; } = 1;
    public static double DefaultMaxOutputTokens { get; } = 8192;
    public static double DefaultTopP { get; } = 1;
    public static double DefaultFrequencyPenalty { get; } //= 0;
    public static double DefaultPresencePenalty { get; } //= 0;
}