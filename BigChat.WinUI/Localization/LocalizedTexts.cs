using BigChat.AppCore.Localization;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Localization;

namespace BigChat.Localization;
internal sealed partial class LocalizedTexts : ObservableObject, ILocalizedTexts
{
    private IStringLocalizer StringLocalizer { get; set; }
    [ObservableProperty] public partial string SettingsText { get; private set; } = null!;
    [ObservableProperty] public partial string ApiEndpointText { get; private set; } = null!;
    [ObservableProperty] public partial string APIKeyText { get; private set; } = null!;
    [ObservableProperty] public partial string APIKeyToolTipText { get; private set; } = null!;
    [ObservableProperty] public partial string ModelIdText { get; private set; } = null!;
    [ObservableProperty] public partial string ModelIdToolTipText { get; private set; } = null!;
    [ObservableProperty] public partial string RestoreDefaultsText { get; private set; } = null!;
    [ObservableProperty] public partial string TemperatureText { get; private set; } = null!;
    [ObservableProperty] public partial string MaxOutputTokensText { get; private set; } = null!;
    [ObservableProperty] public partial string TopPText { get; private set; } = null!;
    [ObservableProperty] public partial string FrequencyPenaltyText { get; private set; } = null!;
    [ObservableProperty] public partial string PresencePenaltyText { get; private set; } = null!;
    [ObservableProperty] public partial string TemperatureToolTipText { get; private set; } = null!;
    [ObservableProperty] public partial string MaxOutputTokensToolTipText { get; private set; } = null!;
    [ObservableProperty] public partial string TopPToolTipText { get; private set; } = null!;
    [ObservableProperty] public partial string FrequencyPenaltyToolTipText { get; private set; } = null!;
    [ObservableProperty] public partial string PresencePenaltyToolTipText { get; private set; } = null!;
    [ObservableProperty] public partial string AppThemeText { get; private set; } = null!;
    [ObservableProperty] public partial string AppThemeToolTipText { get; private set; } = null!;
    [ObservableProperty] public partial string NewChatText { get; private set; } = null!;
    [ObservableProperty] public partial string DeleteText { get; private set; } = null!;
    [ObservableProperty] public partial string RenameText { get; private set; } = null!;
    [ObservableProperty] public partial string InputBoxPlaceholderText { get; private set; } = null!;
    [ObservableProperty] public partial string CancelText { get; internal set; } = null!;
    [ObservableProperty] public partial string MissingSettingsMessageText { get; internal set; } = null!;

    public LocalizedTexts(IStringLocalizer stringLocalizer)
    {
        StringLocalizer = stringLocalizer;
        Load();
    }

    public void Load()
    {
        SettingsText = StringLocalizer[ResourceKeys.Settings];
        APIKeyText = StringLocalizer[ResourceKeys.APIKey];
        APIKeyToolTipText = StringLocalizer[ResourceKeys.APIKeyToolTip];
        ModelIdText = StringLocalizer[ResourceKeys.ModelId];
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
        DeleteText = StringLocalizer[ResourceKeys.Delete];
        RenameText = StringLocalizer[ResourceKeys.Rename];
        InputBoxPlaceholderText = StringLocalizer[ResourceKeys.InputBoxPlaceholder];
        CancelText = StringLocalizer[ResourceKeys.Cancel];
        MissingSettingsMessageText = StringLocalizer[ResourceKeys.MissingSettingsMessage];
    }
}
