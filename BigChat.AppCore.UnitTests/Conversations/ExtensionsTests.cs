using System;

using BigChat.AppCore;
#nullable disable
using BigChat.AppCore.Conversations;
using BigChat.AppCore.Notifications;
using BigChat.Infrastructure.Data.Models;
using BigChat.Utils;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace BigChat.AppCore.Conversations.UnitTests;


/// <summary>
/// Tests for the ToConversationViewModel extension method to ensure correct mapping and error handling.
/// </summary>
public partial class ExtensionsTests
{
    /// <summary>
    /// Verifies ToConversationViewModel correctly maps Id, Subject, and CreatedAt (ToLocalTime)
    /// for diverse edge cases: int boundaries, Subject variations, DateTime boundaries.
    /// </summary>
    [TestMethod]
    public void ToConversationViewModel_ValidConversation_PropertiesMappedCorrectly()
    {
        // Arrange
        var idCases = new[] { int.MinValue, int.MaxValue, 0, -1, 1 };
        var subjectCases = new[]
        {
                null,
                "",
                "   ",
                new string('A', 5000),
                "SpecialChars_\n\t\r!@#$%^&*()"
            };
        var createdAtCases = new[]
        {
                DateTime.MinValue,
                DateTime.MaxValue,
                new DateTime(2000, 1, 1, 12, 0, 0, DateTimeKind.Utc),
                new DateTime(2020, 5, 5, 23, 59, 59, DateTimeKind.Local),
                DateTime.UtcNow,
                DateTime.Now
            };

        foreach (var id in idCases)
        {
            foreach (var subject in subjectCases)
            {
                foreach (var createdAt in createdAtCases)
                {
                    var conv = new Conversation
                    {
                        Id = id,
                        Subject = subject,
                        CreatedAt = createdAt
                    };

                    // Act
                    var result = conv.ToConversationViewModel();

                    // Assert
                    Assert.AreEqual(id, result.Id, "Id should be mapped correctly.");
                    Assert.AreEqual(subject ?? string.Empty, result.Subject, "Subject should be mapped or default to empty string.");
                    Assert.AreEqual(conv.CreatedAt.ToLocalTime(), result.CreatedAt, "CreatedAt should be mapped and converted to local time.");
                }
            }
        }
    }

    /// <summary>
    /// Verifies ToInfoBarSeverity correctly maps each Severity value to its corresponding InfoBarSeverity value.
    /// </summary>
    [TestMethod]
    public void ToInfoBarSeverity_ValidSeverityValues_ReturnsCorrespondingInfoBarSeverity()
    {
        // Arrange & Act & Assert
        Assert.AreEqual(InfoBarSeverity.Informational, Severity.Informational.ToInfoBarSeverity(), "Severity.Informational should map to InfoBarSeverity.Informational.");
        Assert.AreEqual(InfoBarSeverity.Success, Severity.Success.ToInfoBarSeverity(), "Severity.Success should map to InfoBarSeverity.Success.");
        Assert.AreEqual(InfoBarSeverity.Warning, Severity.Warning.ToInfoBarSeverity(), "Severity.Warning should map to InfoBarSeverity.Warning.");
        Assert.AreEqual(InfoBarSeverity.Error, Severity.Error.ToInfoBarSeverity(), "Severity.Error should map to InfoBarSeverity.Error.");
    }

}