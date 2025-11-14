#nullable disable
using System;

using BigChat.AppCore;
using BigChat.AppCore.Messages;
using BigChat.AppCore.ViewModel;
using BigChat.Infrastructure.Data;
using BigChat.Infrastructure.Data.Models;
using Microsoft.Extensions;
using Microsoft.Extensions.AI;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BigChat.AppCore.Messages.UnitTests;


/// <summary>
/// Unit tests for ChatRoleExtensions.
/// </summary>
[TestClass]
public class ChatRoleExtensionsTests
{
    /// <summary>
    /// Verifies that Parse returns the correct ChatRole for each valid string value ("assistant", "system", "tool", "user").
    /// </summary>
    [TestMethod]
    public void Parse_ValidRoleStrings_ReturnsExpectedChatRole()
    {
        // Arrange
        var cases = new[]
        {
                new { Input = ChatRole.Assistant.Value, Expected = ChatRole.Assistant },
                new { Input = ChatRole.System.Value, Expected = ChatRole.System },
                new { Input = ChatRole.Tool.Value, Expected = ChatRole.Tool },
                new { Input = ChatRole.User.Value, Expected = ChatRole.User }
            };

        foreach (var testCase in cases)
        {
            // Act
            var result = ChatRoleExtensions.Parse(testCase.Input);

            // Assert
            Assert.AreEqual(testCase.Expected, result, $"Parse should return correct ChatRole for input '{testCase.Input}'.");
        }
    }

}



/// <summary>
/// Unit tests for MessageExtensions.
/// </summary>
[TestClass]
public class MessageExtensionsTests
{
    /// <summary>
    /// Verifies ToMessageViewModel handles ThinkContent nullability per domain requirements.
    /// - If ThinkContent is null, the ViewModel's ThinkContent is string.Empty.
    /// </summary>
    [TestMethod]
    public void ToMessageViewModel_ThinkContentNull_MapsToEmptyString()
    {
        // Arrange
        var message = new Message
        {
            Id = 10,
            Content = "SampleContent",
            ThinkContent = null,
            Role = ChatRole.User.Value,
            ConversationId = 99
        };

        // Act
        var viewModel = message.ToMessageViewModel();

        // Assert
        Assert.AreEqual(string.Empty, viewModel.ThinkContent, "ThinkContent should be string.Empty if Message.ThinkContent is null.");
    }
}