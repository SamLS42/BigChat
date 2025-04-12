namespace BigChat.AppCore.Localization;

public interface ILocalizedTexts
{
    string ApiEndpointText { get; }
    string APIKeyText { get; }
    string APIKeyToolTipText { get; }
    string AppThemeText { get; }
    string AppThemeToolTipText { get; }
    string CancelText { get; }
    string DeleteText { get; }
    string FrequencyPenaltyText { get; }
    string FrequencyPenaltyToolTipText { get; }
    string InputBoxPlaceholderText { get; }
    string MaxOutputTokensText { get; }
    string MaxOutputTokensToolTipText { get; }
    string ModelIdText { get; }
    string ModelIdToolTipText { get; }
    string NewChatText { get; }
    string PresencePenaltyText { get; }
    string PresencePenaltyToolTipText { get; }
    string RenameText { get; }
    string RestoreDefaultsText { get; }
    string SettingsText { get; }
    string TemperatureText { get; }
    string TemperatureToolTipText { get; }
    string TopPText { get; }
    string TopPToolTipText { get; }
    string MissingSettingsMessageText { get; }

    void Load();
}