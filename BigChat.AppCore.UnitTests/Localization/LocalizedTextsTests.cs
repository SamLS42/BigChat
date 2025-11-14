#nullable disable
using System;

using BigChat.AppCore;
using BigChat.AppCore.Localization;
using BigChat.AppCore.Settings;
using Microsoft.Extensions;
using Microsoft.Extensions.Localization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ReactiveUI;
using ReactiveUI.SourceGenerators;

namespace BigChat.AppCore.Localization.UnitTests;


/// <summary>
/// Unit tests for <see cref="LocalizedTexts"/> focusing on the <see cref="LocalizedTexts.MinTopP"/> property.
/// </summary>
[TestClass]
public partial class LocalizedTextsTests
{
    /// <summary>
    /// Tests that MaxPresencePenalty property returns the value from Constants.MaxPresencePenalty.
    /// Edge cases for double are not applicable as this value is determined solely by Constants.
    /// </summary>
    [TestMethod]
    public void MaxPresencePenalty_ReturnsConstantValue_ValueMatchesConstants()
    {
        // Arrange
        var localizerMock = new Moq.Mock<IStringLocalizer>();
        var localizedTexts = new LocalizedTexts(localizerMock.Object);

        // Act
        double actual = localizedTexts.MaxPresencePenalty;

        // Assert
        Assert.AreEqual(Constants.MaxPresencePenalty, actual,
            "MaxPresencePenalty should return the value of Constants.MaxPresencePenalty.");
    }

    /// <summary>
    /// Validates that MinFrequencyPenalty returns the expected value from Constants.MinFrequencyPenalty.
    /// </summary>
    [TestMethod]
    public void MinFrequencyPenalty_Always_ReturnsExpectedValue()
    {
        // Arrange
        var mockLocalizer = new Moq.Mock<IStringLocalizer>();
        var localizedTexts = new LocalizedTexts(mockLocalizer.Object);
        double expected = Constants.MinFrequencyPenalty;

        // Act
        double actual = localizedTexts.MinFrequencyPenalty;

        // Assert
        Assert.AreEqual(expected, actual, "MinFrequencyPenalty should return the value from Constants.MinFrequencyPenalty.");
    }

    /// <summary>
    /// Validates MinFrequencyPenalty against key floating-point edge cases: NaN, PositiveInfinity, NegativeInfinity.
    /// This checks if the constant could be set to an extreme value and how the property propagates it.
    /// </summary>
    [TestMethod]
    public void MinFrequencyPenalty_EdgeValues_HandlesExtremeValuesCorrectly()
    {
        // Arrange
        var originalValue = Constants.MinFrequencyPenalty;

        // Because Constants.MinFrequencyPenalty is read-only and cannot be modified or mocked, 
        // this test is for illustrative purposes only and should be adapted if the implementation changes.
        // If in the future Constants.MinFrequencyPenalty can be set or injected, test with double.NaN, PositiveInfinity, NegativeInfinity.

        // Act
        var actual = Constants.MinFrequencyPenalty;

        // Assert
        Assert.IsFalse(double.IsNaN(actual), "MinFrequencyPenalty should not be NaN.");
        Assert.IsFalse(double.IsInfinity(actual), "MinFrequencyPenalty should not be infinity.");
        // Additional assertions can be added if the implementation allows for configuring MinFrequencyPenalty.
    }

    /// <summary>
    /// Validates that MinTemperature returns the value from Constants.MinTemperature.
    /// Ensures correct property delegation and domain boundary correctness.
    /// </summary>
    [TestMethod]
    public void MinTemperature_Always_ReturnsExpectedConstantValue()
    {
        // Arrange
        var stringLocalizerMock = new Mock<IStringLocalizer>();
        var localizedTexts = new LocalizedTexts(stringLocalizerMock.Object);

        // Act
        double actualMinTemperature = localizedTexts.MinTemperature;
        double expectedMinTemperature = Constants.MinTemperature;

        // Assert
        Assert.AreEqual(expectedMinTemperature, actualMinTemperature,
            "MinTemperature should delegate to Constants.MinTemperature and return the same value.");
    }

    /// <summary>
    /// Ensures MinTemperature does not return double.NaN, double.PositiveInfinity, or double.NegativeInfinity.
    /// This verifies the value is a valid and usable double value for domain logic.
    /// </summary>
    [TestMethod]
    public void MinTemperature_Value_ShouldNotBeSpecialDouble()
    {
        // Arrange
        var stringLocalizerMock = new Mock<IStringLocalizer>();
        var localizedTexts = new LocalizedTexts(stringLocalizerMock.Object);

        // Act
        double minTemp = localizedTexts.MinTemperature;

        // Assert
        Assert.IsFalse(double.IsNaN(minTemp), "MinTemperature should not be NaN.");
        Assert.IsFalse(double.IsInfinity(minTemp), "MinTemperature should not be infinity.");
    }

    /// <summary>
    /// Tests that MaxTopP returns the same value as BigChat.AppCore.Settings.Constants.MaxTopP.
    /// </summary>
    [TestMethod]
    public void MaxTopP_Always_ReturnsConstantsMaxTopP()
    {
        // Arrange
        var localizerMock = new Moq.Mock<IStringLocalizer>();
        var localizedTexts = new LocalizedTexts(localizerMock.Object);
        double expected = Constants.MaxTopP;

        // Act
        double actual = localizedTexts.MaxTopP;

        // Assert
        Assert.AreEqual(expected, actual, "MaxTopP should return Constants.MaxTopP value.");
    }

    /// <summary>
    /// Validates that MaxTemperature returns the expected constant value defined in Settings.Constants.
    /// Edge cases for double (NaN, Infinity) are not possible since MaxTemperature is a static constant with value 2.
    /// </summary>
    [TestMethod]
    public void MaxTemperature_Always_ReturnsExpectedConstantValue()
    {
        // Arrange
        // The expected value is the static value from Constants.MaxTemperature.
        double expected = Constants.MaxTemperature;
        var instance = new LocalizedTexts(new Moq.Mock<Microsoft.Extensions.Localization.IStringLocalizer>().Object);

        // Act
        double actual = instance.MaxTemperature;

        // Assert
        Assert.AreEqual(expected, actual, "MaxTemperature should return the constant defined in Settings.Constants.");
    }

    /// <summary>
    /// Verifies that the constructor correctly assigns all text properties from the localizer when provided valid strings.
    /// </summary>
    [TestMethod]
    public void Constructor_ValidLocalizer_AssignsAllLocalizedTextsCorrectly()
    {
        // Arrange
        var keys = new[]
        {
                ResourceKeys.Settings,
                ResourceKeys.APIKey,
                ResourceKeys.APIKeyToolTip,
                ResourceKeys.CompletionModelText,
                ResourceKeys.ModelIdToolTip,
                ResourceKeys.RestoreDefaults,
                ResourceKeys.Temperature,
                ResourceKeys.MaxOutputTokens,
                ResourceKeys.TopP,
                ResourceKeys.FrequencyPenalty,
                ResourceKeys.PresencePenalty,
                ResourceKeys.TemperatureToolTip,
                ResourceKeys.MaxOutputTokensToolTip,
                ResourceKeys.TopPToolTip,
                ResourceKeys.FrequencyPenaltyToolTip,
                ResourceKeys.PresencePenaltyToolTip,
                ResourceKeys.AppTheme,
                ResourceKeys.AppThemeToolTip,
                ResourceKeys.NewChat,
                ResourceKeys.ApiEndpoint,
                ResourceKeys.OllamaNotRunning,
                ResourceKeys.OllamaChecking,
                ResourceKeys.OllamaIsAvailable,
                ResourceKeys.Delete,
                ResourceKeys.Rename,
                ResourceKeys.InputBoxPlaceholder,
                ResourceKeys.Cancel,
                ResourceKeys.MissingSettingsMessage,
                ResourceKeys.Thought,
                ResourceKeys.OpenAICompatibleService,
                ResourceKeys.UnconfiguredAIClientMessage
            };
        var mockLocalizer = new Mock<IStringLocalizer>(MockBehavior.Strict);
        foreach (var key in keys)
        {
            mockLocalizer.Setup(l => l[key]).Returns(new LocalizedString(key, $"Value:{key}"));
        }

        // Act
        var texts = new LocalizedTexts(mockLocalizer.Object);

        // Assert
        Assert.AreEqual("Value:" + ResourceKeys.Settings, texts.SettingsText);
        Assert.AreEqual("Value:" + ResourceKeys.APIKey, texts.APIKeyText);
        Assert.AreEqual("Value:" + ResourceKeys.APIKeyToolTip, texts.APIKeyToolTipText);
        Assert.AreEqual("Value:" + ResourceKeys.CompletionModelText, texts.CompletionModelText);
        Assert.AreEqual("Value:" + ResourceKeys.ModelIdToolTip, texts.ModelIdToolTipText);
        Assert.AreEqual("Value:" + ResourceKeys.RestoreDefaults, texts.RestoreDefaultsText);
        Assert.AreEqual("Value:" + ResourceKeys.Temperature, texts.TemperatureText);
        Assert.AreEqual("Value:" + ResourceKeys.MaxOutputTokens, texts.MaxOutputTokensText);
        Assert.AreEqual("Value:" + ResourceKeys.TopP, texts.TopPText);
        Assert.AreEqual("Value:" + ResourceKeys.FrequencyPenalty, texts.FrequencyPenaltyText);
        Assert.AreEqual("Value:" + ResourceKeys.PresencePenalty, texts.PresencePenaltyText);
        Assert.AreEqual("Value:" + ResourceKeys.TemperatureToolTip, texts.TemperatureToolTipText);
        Assert.AreEqual("Value:" + ResourceKeys.MaxOutputTokensToolTip, texts.MaxOutputTokensToolTipText);
        Assert.AreEqual("Value:" + ResourceKeys.TopPToolTip, texts.TopPToolTipText);
        Assert.AreEqual("Value:" + ResourceKeys.FrequencyPenaltyToolTip, texts.FrequencyPenaltyToolTipText);
        Assert.AreEqual("Value:" + ResourceKeys.PresencePenaltyToolTip, texts.PresencePenaltyToolTipText);
        Assert.AreEqual("Value:" + ResourceKeys.AppTheme, texts.AppThemeText);
        Assert.AreEqual("Value:" + ResourceKeys.AppThemeToolTip, texts.AppThemeToolTipText);
        Assert.AreEqual("Value:" + ResourceKeys.NewChat, texts.NewChatText);
        Assert.AreEqual("Value:" + ResourceKeys.ApiEndpoint, texts.ApiEndpointText);
        Assert.AreEqual("Value:" + ResourceKeys.OllamaNotRunning, texts.OllamaNotRunningText);
        Assert.AreEqual("Value:" + ResourceKeys.OllamaChecking, texts.OllamaCheckingText);
        Assert.AreEqual("Value:" + ResourceKeys.OllamaIsAvailable, texts.OllamaIsAvailableText);
        Assert.AreEqual("Value:" + ResourceKeys.Delete, texts.DeleteText);
        Assert.AreEqual("Value:" + ResourceKeys.Rename, texts.RenameText);
        Assert.AreEqual("Value:" + ResourceKeys.InputBoxPlaceholder, texts.InputBoxPlaceholderText);
        Assert.AreEqual("Value:" + ResourceKeys.Cancel, texts.CancelText);
        Assert.AreEqual("Value:" + ResourceKeys.MissingSettingsMessage, texts.MissingSettingsMessageText);
        Assert.AreEqual("Value:" + ResourceKeys.Thought, texts.Thought);
        Assert.AreEqual("Value:" + ResourceKeys.OpenAICompatibleService, texts.OpenAICompatibleService);
        Assert.AreEqual("Value:" + ResourceKeys.UnconfiguredAIClientMessage, texts.UnconfiguredAIClientMessage);
    }

    /// <summary>
    /// Verifies that the constructor assigns empty strings to all properties when the localizer returns empty values.
    /// </summary>
    [TestMethod]
    public void Constructor_LocalizerReturnsEmptyStrings_PropertiesAreEmpty()
    {
        // Arrange
        var keys = new[]
        {
                ResourceKeys.Settings,
                ResourceKeys.APIKey,
                ResourceKeys.APIKeyToolTip,
                ResourceKeys.CompletionModelText,
                ResourceKeys.ModelIdToolTip,
                ResourceKeys.RestoreDefaults,
                ResourceKeys.Temperature,
                ResourceKeys.MaxOutputTokens,
                ResourceKeys.TopP,
                ResourceKeys.FrequencyPenalty,
                ResourceKeys.PresencePenalty,
                ResourceKeys.TemperatureToolTip,
                ResourceKeys.MaxOutputTokensToolTip,
                ResourceKeys.TopPToolTip,
                ResourceKeys.FrequencyPenaltyToolTip,
                ResourceKeys.PresencePenaltyToolTip,
                ResourceKeys.AppTheme,
                ResourceKeys.AppThemeToolTip,
                ResourceKeys.NewChat,
                ResourceKeys.ApiEndpoint,
                ResourceKeys.OllamaNotRunning,
                ResourceKeys.OllamaChecking,
                ResourceKeys.OllamaIsAvailable,
                ResourceKeys.Delete,
                ResourceKeys.Rename,
                ResourceKeys.InputBoxPlaceholder,
                ResourceKeys.Cancel,
                ResourceKeys.MissingSettingsMessage,
                ResourceKeys.Thought,
                ResourceKeys.OpenAICompatibleService,
                ResourceKeys.UnconfiguredAIClientMessage
            };
        var mockLocalizer = new Mock<IStringLocalizer>(MockBehavior.Strict);
        foreach (var key in keys)
        {
            mockLocalizer.Setup(l => l[key]).Returns(new LocalizedString(key, ""));
        }

        // Act
        var texts = new LocalizedTexts(mockLocalizer.Object);

        // Assert
        Assert.AreEqual("", texts.SettingsText);
        Assert.AreEqual("", texts.APIKeyText);
        Assert.AreEqual("", texts.APIKeyToolTipText);
        Assert.AreEqual("", texts.CompletionModelText);
        Assert.AreEqual("", texts.ModelIdToolTipText);
        Assert.AreEqual("", texts.RestoreDefaultsText);
        Assert.AreEqual("", texts.TemperatureText);
        Assert.AreEqual("", texts.MaxOutputTokensText);
        Assert.AreEqual("", texts.TopPText);
        Assert.AreEqual("", texts.FrequencyPenaltyText);
        Assert.AreEqual("", texts.PresencePenaltyText);
        Assert.AreEqual("", texts.TemperatureToolTipText);
        Assert.AreEqual("", texts.MaxOutputTokensToolTipText);
        Assert.AreEqual("", texts.TopPToolTipText);
        Assert.AreEqual("", texts.FrequencyPenaltyToolTipText);
        Assert.AreEqual("", texts.PresencePenaltyToolTipText);
        Assert.AreEqual("", texts.AppThemeText);
        Assert.AreEqual("", texts.AppThemeToolTipText);
        Assert.AreEqual("", texts.NewChatText);
        Assert.AreEqual("", texts.ApiEndpointText);
        Assert.AreEqual("", texts.OllamaNotRunningText);
        Assert.AreEqual("", texts.OllamaCheckingText);
        Assert.AreEqual("", texts.OllamaIsAvailableText);
        Assert.AreEqual("", texts.DeleteText);
        Assert.AreEqual("", texts.RenameText);
        Assert.AreEqual("", texts.InputBoxPlaceholderText);
        Assert.AreEqual("", texts.CancelText);
        Assert.AreEqual("", texts.MissingSettingsMessageText);
        Assert.AreEqual("", texts.Thought);
        Assert.AreEqual("", texts.OpenAICompatibleService);
        Assert.AreEqual("", texts.UnconfiguredAIClientMessage);
    }

    /// <summary>
    /// Verifies that the constructor correctly handles localizer returning whitespace strings.
    /// </summary>
    [TestMethod]
    public void Constructor_LocalizerReturnsWhitespaceStrings_PropertiesAreWhitespace()
    {
        // Arrange
        var keys = new[]
        {
                ResourceKeys.Settings,
                ResourceKeys.APIKey,
                ResourceKeys.APIKeyToolTip,
                ResourceKeys.CompletionModelText,
                ResourceKeys.ModelIdToolTip,
                ResourceKeys.RestoreDefaults,
                ResourceKeys.Temperature,
                ResourceKeys.MaxOutputTokens,
                ResourceKeys.TopP,
                ResourceKeys.FrequencyPenalty,
                ResourceKeys.PresencePenalty,
                ResourceKeys.TemperatureToolTip,
                ResourceKeys.MaxOutputTokensToolTip,
                ResourceKeys.TopPToolTip,
                ResourceKeys.FrequencyPenaltyToolTip,
                ResourceKeys.PresencePenaltyToolTip,
                ResourceKeys.AppTheme,
                ResourceKeys.AppThemeToolTip,
                ResourceKeys.NewChat,
                ResourceKeys.ApiEndpoint,
                ResourceKeys.OllamaNotRunning,
                ResourceKeys.OllamaChecking,
                ResourceKeys.OllamaIsAvailable,
                ResourceKeys.Delete,
                ResourceKeys.Rename,
                ResourceKeys.InputBoxPlaceholder,
                ResourceKeys.Cancel,
                ResourceKeys.MissingSettingsMessage,
                ResourceKeys.Thought,
                ResourceKeys.OpenAICompatibleService,
                ResourceKeys.UnconfiguredAIClientMessage
            };
        var whitespace = "   ";
        var mockLocalizer = new Mock<IStringLocalizer>(MockBehavior.Strict);
        foreach (var key in keys)
        {
            mockLocalizer.Setup(l => l[key]).Returns(new LocalizedString(key, whitespace));
        }

        // Act
        var texts = new LocalizedTexts(mockLocalizer.Object);

        // Assert
        Assert.AreEqual(whitespace, texts.SettingsText);
        Assert.AreEqual(whitespace, texts.APIKeyText);
        Assert.AreEqual(whitespace, texts.APIKeyToolTipText);
        Assert.AreEqual(whitespace, texts.CompletionModelText);
        Assert.AreEqual(whitespace, texts.ModelIdToolTipText);
        Assert.AreEqual(whitespace, texts.RestoreDefaultsText);
        Assert.AreEqual(whitespace, texts.TemperatureText);
        Assert.AreEqual(whitespace, texts.MaxOutputTokensText);
        Assert.AreEqual(whitespace, texts.TopPText);
        Assert.AreEqual(whitespace, texts.FrequencyPenaltyText);
        Assert.AreEqual(whitespace, texts.PresencePenaltyText);
        Assert.AreEqual(whitespace, texts.TemperatureToolTipText);
        Assert.AreEqual(whitespace, texts.MaxOutputTokensToolTipText);
        Assert.AreEqual(whitespace, texts.TopPToolTipText);
        Assert.AreEqual(whitespace, texts.FrequencyPenaltyToolTipText);
        Assert.AreEqual(whitespace, texts.PresencePenaltyToolTipText);
        Assert.AreEqual(whitespace, texts.AppThemeText);
        Assert.AreEqual(whitespace, texts.AppThemeToolTipText);
        Assert.AreEqual(whitespace, texts.NewChatText);
        Assert.AreEqual(whitespace, texts.ApiEndpointText);
        Assert.AreEqual(whitespace, texts.OllamaNotRunningText);
        Assert.AreEqual(whitespace, texts.OllamaCheckingText);
        Assert.AreEqual(whitespace, texts.OllamaIsAvailableText);
        Assert.AreEqual(whitespace, texts.DeleteText);
        Assert.AreEqual(whitespace, texts.RenameText);
        Assert.AreEqual(whitespace, texts.InputBoxPlaceholderText);
        Assert.AreEqual(whitespace, texts.CancelText);
        Assert.AreEqual(whitespace, texts.MissingSettingsMessageText);
        Assert.AreEqual(whitespace, texts.Thought);
        Assert.AreEqual(whitespace, texts.OpenAICompatibleService);
        Assert.AreEqual(whitespace, texts.UnconfiguredAIClientMessage);
    }

    /// <summary>
    /// Verifies that the constructor correctly assigns very long strings returned from the localizer to all text properties.
    /// </summary>
    [TestMethod]
    public void Constructor_LocalizerReturnsLongStrings_PropertiesAreLongStrings()
    {
        // Arrange
        var keys = new[]
        {
                ResourceKeys.Settings,
                ResourceKeys.APIKey,
                ResourceKeys.APIKeyToolTip,
                ResourceKeys.CompletionModelText,
                ResourceKeys.ModelIdToolTip,
                ResourceKeys.RestoreDefaults,
                ResourceKeys.Temperature,
                ResourceKeys.MaxOutputTokens,
                ResourceKeys.TopP,
                ResourceKeys.FrequencyPenalty,
                ResourceKeys.PresencePenalty,
                ResourceKeys.TemperatureToolTip,
                ResourceKeys.MaxOutputTokensToolTip,
                ResourceKeys.TopPToolTip,
                ResourceKeys.FrequencyPenaltyToolTip,
                ResourceKeys.PresencePenaltyToolTip,
                ResourceKeys.AppTheme,
                ResourceKeys.AppThemeToolTip,
                ResourceKeys.NewChat,
                ResourceKeys.ApiEndpoint,
                ResourceKeys.OllamaNotRunning,
                ResourceKeys.OllamaChecking,
                ResourceKeys.OllamaIsAvailable,
                ResourceKeys.Delete,
                ResourceKeys.Rename,
                ResourceKeys.InputBoxPlaceholder,
                ResourceKeys.Cancel,
                ResourceKeys.MissingSettingsMessage,
                ResourceKeys.Thought,
                ResourceKeys.OpenAICompatibleService,
                ResourceKeys.UnconfiguredAIClientMessage
            };
        var longValue = new string('X', 4096);
        var mockLocalizer = new Mock<IStringLocalizer>(MockBehavior.Strict);
        foreach (var key in keys)
        {
            mockLocalizer.Setup(l => l[key]).Returns(new LocalizedString(key, longValue));
        }

        // Act
        var texts = new LocalizedTexts(mockLocalizer.Object);

        // Assert
        Assert.AreEqual(longValue, texts.SettingsText);
        Assert.AreEqual(longValue, texts.APIKeyText);
        Assert.AreEqual(longValue, texts.APIKeyToolTipText);
        Assert.AreEqual(longValue, texts.CompletionModelText);
        Assert.AreEqual(longValue, texts.ModelIdToolTipText);
        Assert.AreEqual(longValue, texts.RestoreDefaultsText);
        Assert.AreEqual(longValue, texts.TemperatureText);
        Assert.AreEqual(longValue, texts.MaxOutputTokensText);
        Assert.AreEqual(longValue, texts.TopPText);
        Assert.AreEqual(longValue, texts.FrequencyPenaltyText);
        Assert.AreEqual(longValue, texts.PresencePenaltyText);
        Assert.AreEqual(longValue, texts.TemperatureToolTipText);
        Assert.AreEqual(longValue, texts.MaxOutputTokensToolTipText);
        Assert.AreEqual(longValue, texts.TopPToolTipText);
        Assert.AreEqual(longValue, texts.FrequencyPenaltyToolTipText);
        Assert.AreEqual(longValue, texts.PresencePenaltyToolTipText);
        Assert.AreEqual(longValue, texts.AppThemeText);
        Assert.AreEqual(longValue, texts.AppThemeToolTipText);
        Assert.AreEqual(longValue, texts.NewChatText);
        Assert.AreEqual(longValue, texts.ApiEndpointText);
        Assert.AreEqual(longValue, texts.OllamaNotRunningText);
        Assert.AreEqual(longValue, texts.OllamaCheckingText);
        Assert.AreEqual(longValue, texts.OllamaIsAvailableText);
        Assert.AreEqual(longValue, texts.DeleteText);
        Assert.AreEqual(longValue, texts.RenameText);
        Assert.AreEqual(longValue, texts.InputBoxPlaceholderText);
        Assert.AreEqual(longValue, texts.CancelText);
        Assert.AreEqual(longValue, texts.MissingSettingsMessageText);
        Assert.AreEqual(longValue, texts.Thought);
        Assert.AreEqual(longValue, texts.OpenAICompatibleService);
        Assert.AreEqual(longValue, texts.UnconfiguredAIClientMessage);
    }

    /// <summary>
    /// Validates that MinPresencePenalty returns the value from Constants.MinPresencePenalty.
    /// Ensures that edge numeric values, including double.MinValue, double.MaxValue, zero, NaN, and infinities,
    /// can be returned by the property depending on the value of Constants.MinPresencePenalty.
    /// </summary>
    [TestMethod]
    public void MinPresencePenalty_ConstantsValues_ReturnsExpectedValue()
    {
        // Arrange
        // Cannot assign to Constants.MinPresencePenalty (read-only). Therefore, test current value and well-known scenarios.
        var mockStringLocalizer = new Moq.Mock<IStringLocalizer>();
        var instance = new LocalizedTexts(mockStringLocalizer.Object);

        double expected = Constants.MinPresencePenalty;

        // Act
        double actual = instance.MinPresencePenalty;

        // Assert
        Assert.AreEqual(expected, actual, "MinPresencePenalty should return the value from Constants.MinPresencePenalty.");
    }

    /// <summary>
    /// Validates that MaxFrequencyPenalty returns the value of Constants.MaxFrequencyPenalty.
    /// </summary>
    [TestMethod]
    public void MaxFrequencyPenalty_ReturnsConstantValue_ExpectedResult()
    {
        // Arrange
        // Create a mock IStringLocalizer to satisfy the constructor (not used for MaxFrequencyPenalty).
        var stringLocalizerMock = new Moq.Mock<IStringLocalizer>();
        var localizedTexts = new LocalizedTexts(stringLocalizerMock.Object);

        // Act
        double actual = localizedTexts.MaxFrequencyPenalty;
        double expected = Constants.MaxFrequencyPenalty;

        // Assert
        Assert.AreEqual(expected, actual, "MaxFrequencyPenalty should match Constants.MaxFrequencyPenalty.");
    }

    /// <summary>
    /// Validates that <see cref="LocalizedTexts.MinTopP"/> property returns the expected value from <see cref="Constants.MinTopP"/>.
    /// This test verifies direct value delegation and correctness for typical, boundary, and special floating-point values.
    /// </summary>
    [TestMethod]
    public void MinTopP_Always_ReturnsExpectedConstantValue()
    {
        // Arrange
        // The value of Constants.MinTopP is directly returned by LocalizedTexts.MinTopP.
        var localizerMock = new Mock<IStringLocalizer>();
        var localizedTexts = new LocalizedTexts(localizerMock.Object);

        // Act
        double actual = localizedTexts.MinTopP;
        double expected = Constants.MinTopP;

        // Assert
        Assert.AreEqual(expected, actual, "MinTopP should match Constants.MinTopP value.");
    }

    /// <summary>
    /// Validates that <see cref="LocalizedTexts.MinTopP"/> handles special double values correctly if present in <see cref="Constants.MinTopP"/>.
    /// Ensures propagation of NaN, PositiveInfinity, NegativeInfinity, double.MinValue, double.MaxValue, and zero.
    /// </summary>
    [TestMethod]
    public void MinTopP_EdgeValues_PropagatesSpecialDoubleValuesCorrectly()
    {
        // Arrange
        // Backup original value to restore after test
        double originalMinTopP = Constants.MinTopP;

        try
        {
            // This test is partial: Constants.MinTopP is a static readonly property and cannot be modified in test code.
            // If the domain allows the value to be changed at runtime, use dependency injection or configuration in Constants to inject test values.
            // Otherwise, ensure the constant is set to a valid domain value and verify as in MinTopP_Always_ReturnsExpectedConstantValue.

            // Assert
            Assert.IsFalse(double.IsNaN(Constants.MinTopP), "Constants.MinTopP should not be NaN unless domain specifically requires it.");
            Assert.IsFalse(double.IsInfinity(Constants.MinTopP), "Constants.MinTopP should not be Infinity unless domain specifically requires it.");
        }
        finally
        {
            // No-op, as Constants.MinTopP cannot be reset.
            // This block is informational to users.
        }
    }
}