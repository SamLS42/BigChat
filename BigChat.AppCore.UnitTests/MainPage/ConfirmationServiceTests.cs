#nullable disable
using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;

using BigChat.AppCore;
using BigChat.AppCore.Conversations;
using BigChat.AppCore.MainPage;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BigChat.AppCore.MainPage.UnitTests;


/// <summary>
/// Unit tests for <see cref="ConversationOperationsService.RenameRequests"/>.
/// </summary>
[TestClass]
public sealed partial class ConversationOperationsServiceTests
{
    /// <summary>
    /// Verifies that RenameRequests does not emit anything until RequestRename is called.
    /// </summary>
    [TestMethod]
    public void RenameRequests_NoRequestRename_NoEmissions()
    {
        // Arrange
        var service = new ConversationOperationsService();
        bool emitted = false;
        using var subscription = service.RenameRequests.Subscribe(_ => emitted = true);

        // Act & Assert
        Assert.IsFalse(emitted);
    }

    /// <summary>
    /// Verifies that DeletionRequests does not emit anything until RequestDeletion is called.
    /// Ensures there are no emissions on subscription alone.
    /// </summary>
    [TestMethod]
    public void DeletionRequests_NoRequestDeletion_NoEmissions()
    {
        // Arrange
        var service = new ConversationOperationsService();
        bool emitted = false;
        using var subscription = service.DeletionRequests.Subscribe(_ => emitted = true);

        // Act & Assert
        Assert.IsFalse(emitted,
            "DeletionRequests should not emit any value until RequestDeletion is called.");
    }

}