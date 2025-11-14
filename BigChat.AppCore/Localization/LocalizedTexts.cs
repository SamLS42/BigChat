using BigChat.AppCore.Settings;
using Microsoft.Extensions.Localization;
using ReactiveUI;
using ReactiveUI.SourceGenerators;

namespace BigChat.AppCore.Localization;

public sealed partial class LocalizedTexts : ReactiveObject
{
    private IStringLocalizer StringLocalizer { get; set; }
    [Reactive] public partial string SettingsText { get; private set; }
    [Reactive] public partial string ApiEndpointText { get; private set; }
    [Reactive] public partial string OllamaNotRunningText { get; private set; }
    [Reactive] public partial string OllamaCheckingText { get; private set; }
    [Reactive] public partial string OllamaIsAvailableText { get; private set; }
    [Reactive] public partial string APIKeyText { get; private set; }
    [Reactive] public partial string APIKeyToolTipText { get; private set; }
    [Reactive] public partial string CompletionModelText { get; private set; }
    [Reactive] public partial string ModelIdToolTipText { get; private set; }
    [Reactive] public partial string RestoreDefaultsText { get; private set; }
    [Reactive] public partial string TemperatureText { get; private set; }
    [Reactive] public partial string MaxOutputTokensText { get; private set; }
    [Reactive] public partial string TopPText { get; private set; }
    [Reactive] public partial string FrequencyPenaltyText { get; private set; }
    [Reactive] public partial string PresencePenaltyText { get; private set; }
    [Reactive] public partial string TemperatureToolTipText { get; private set; }
    [Reactive] public partial string MaxOutputTokensToolTipText { get; private set; }
    [Reactive] public partial string TopPToolTipText { get; private set; }
    [Reactive] public partial string FrequencyPenaltyToolTipText { get; private set; }
    [Reactive] public partial string PresencePenaltyToolTipText { get; private set; }
    [Reactive] public partial string AppThemeText { get; private set; }
    [Reactive] public partial string AppThemeToolTipText { get; private set; }
    [Reactive] public partial string NewChatText { get; private set; }
    [Reactive] public partial string DeleteText { get; private set; }
    [Reactive] public partial string RenameText { get; private set; }
    [Reactive] public partial string InputBoxPlaceholderText { get; private set; }
    [Reactive] public partial string CancelText { get; internal set; }
    [Reactive] public partial string MissingSettingsMessageText { get; internal set; }
    [Reactive] public partial string Thought { get; internal set; }
    [Reactive] public partial string OpenAICompatibleService { get; internal set; }
    [Reactive] public partial string UnconfiguredAIClientMessage { get; internal set; }
    public double MaxTemperature => Constants.MaxTemperature;
    public double MinTemperature => Constants.MinTemperature;
    public double MaxTopP => Constants.MaxTopP;
    public double MinTopP => Constants.MinTopP;
    public double MaxFrequencyPenalty => Constants.MaxFrequencyPenalty;
    public double MinFrequencyPenalty => Constants.MinFrequencyPenalty;
    public double MaxPresencePenalty => Constants.MaxPresencePenalty;
    public double MinPresencePenalty => Constants.MinPresencePenalty;

    public LocalizedTexts(IStringLocalizer stringLocalizer)
    {
        StringLocalizer = stringLocalizer;

        SettingsText = StringLocalizer[ResourceKeys.Settings];
        APIKeyText = StringLocalizer[ResourceKeys.APIKey];
        APIKeyToolTipText = StringLocalizer[ResourceKeys.APIKeyToolTip];
        CompletionModelText = StringLocalizer[ResourceKeys.CompletionModelText];
        ModelIdToolTipText = StringLocalizer[ResourceKeys.ModelIdToolTip];
        RestoreDefaultsText = StringLocalizer[ResourceKeys.RestoreDefaults];
        TemperatureText = StringLocalizer[ResourceKeys.Temperature];
        MaxOutputTokensText = StringLocalizer[ResourceKeys.MaxOutputTokens];
        TopPText = StringLocalizer[ResourceKeys.TopP];
        FrequencyPenaltyText = StringLocalizer[ResourceKeys.FrequencyPenalty];
        PresencePenaltyText = StringLocalizer[ResourceKeys.PresencePenalty];
        TemperatureToolTipText = StringLocalizer[ResourceKeys.TemperatureToolTip];
        MaxOutputTokensToolTipText = StringLocalizer[ResourceKeys.MaxOutputTokensToolTip];
        TopPToolTipText = StringLocalizer[ResourceKeys.TopPToolTip];
        FrequencyPenaltyToolTipText = StringLocalizer[ResourceKeys.FrequencyPenaltyToolTip];
        PresencePenaltyToolTipText = StringLocalizer[ResourceKeys.PresencePenaltyToolTip];
        AppThemeText = StringLocalizer[ResourceKeys.AppTheme];
        AppThemeToolTipText = StringLocalizer[ResourceKeys.AppThemeToolTip];
        NewChatText = StringLocalizer[ResourceKeys.NewChat];
        ApiEndpointText = StringLocalizer[ResourceKeys.ApiEndpoint];
        OllamaNotRunningText = StringLocalizer[ResourceKeys.OllamaNotRunning];
        OllamaCheckingText = StringLocalizer[ResourceKeys.OllamaChecking];
        OllamaIsAvailableText = StringLocalizer[ResourceKeys.OllamaIsAvailable];
        DeleteText = StringLocalizer[ResourceKeys.Delete];
        RenameText = StringLocalizer[ResourceKeys.Rename];
        InputBoxPlaceholderText = StringLocalizer[ResourceKeys.InputBoxPlaceholder];
        CancelText = StringLocalizer[ResourceKeys.Cancel];
        MissingSettingsMessageText = StringLocalizer[ResourceKeys.MissingSettingsMessage];
        Thought = StringLocalizer[ResourceKeys.Thought];
        OpenAICompatibleService = StringLocalizer[ResourceKeys.OpenAICompatibleService];
        UnconfiguredAIClientMessage = StringLocalizer[ResourceKeys.UnconfiguredAIClientMessage];
    }
}
