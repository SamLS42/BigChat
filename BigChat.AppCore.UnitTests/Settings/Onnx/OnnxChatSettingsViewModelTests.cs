#nullable disable
using System;

using BigChat.AppCore.Settings;
using BigChat.AppCore.Settings.Onnx;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ReactiveUI;
using ReactiveUI.SourceGenerators;

namespace BigChat.AppCore.Settings.UnitTests;

/// <summary>
/// Unit tests for <see cref="OnnxChatSettingsViewModel"/> constructor.
/// </summary>
[TestClass]
public partial class OnnxChatSettingsViewModelTests
{
    /// <summary>
    /// Verifies that Save does not throw exceptions for typical values.
    /// </summary>
    [TestMethod]
    public void Save_ValidTypicalValues_DoesNotThrow()
    {
        // Arrange
        var settingsMock = new Mock<ISettingsService>(MockBehavior.Strict);
        var chatSettings = new OnnxChatClientSettings();
        settingsMock.Setup(s => s.GetOnnxChatSettings()).Returns(chatSettings);
        settingsMock.Setup(s => s.SetOnnxChatClientSettings(It.IsAny<OnnxChatClientSettings>()));

        var vm = new OnnxChatSettingsViewModel(settingsMock.Object)
        {
            OnnxModelDir = "C:\\models",
            Temperature = 1.0,
            MaxOutputTokens = 128,
            TopP = 0.9,
            FrequencyPenalty = 0.2,
            PresencePenalty = 0.3
        };

        // Act & Assert
        vm.Save();
        settingsMock.Verify(s => s.SetOnnxChatClientSettings(chatSettings), Times.Once);
    }

}