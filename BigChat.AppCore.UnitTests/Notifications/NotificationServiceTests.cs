using BigChat.AppCore;
using BigChat.AppCore.Notifications;
using BigChat.Infrastructure.Data.Models;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace BigChat.AppCore.Notifications.UnitTests;
/// <summary>
/// Unit tests for <see cref = "NotificationService"/>.
/// </summary>
[TestClass]
public class NotificationServiceTests
{
    /// <summary>
    /// Tests that Send delivers a NotificationMessage with the correct message and severity to observers.
    /// Edge cases for message: empty string, whitespace, very long string, special/control characters.
    /// Covers all defined Severity enum values and also an out-of-range value.
    /// </summary>
    [TestMethod]
    public void Send_ValidInputs_NotificationIsDeliveredWithCorrectValues()
    {
        // Arrange
        var service = new NotificationService();
        var receivedNotifications = new List<NotificationMessage>();
        using var subscription = service.Notifications.Subscribe(n => receivedNotifications.Add(n));
        var testCases = new (Severity severity, string message)[]
        {
            (Severity.Informational, ""),
            (Severity.Success, " "),
            (Severity.Warning, new string ('x', 5000)),
            (Severity.Error, "Test\nMessage\t\u0001"),
            ((Severity)9999, "Unusual severity"),
        };
        foreach (var(severity, message)in testCases)
        {
            // Act
            service.Send(severity, message);
            // Assert
            var notification = receivedNotifications.LastOrDefault();
            Assert.IsNotNull(notification, "Notification was not received.");
            Assert.AreEqual(message, notification.Text, "NotificationMessage.Text does not match.");
            Assert.AreEqual(severity, notification.Severity, "NotificationMessage.Severity does not match.");
        }
    }

    /// <summary>
    /// Tests that Dispose_DisposeCalledTwice_NoExceptionThrown ensures idempotency and proper resource cleanup.
    /// </summary>
    [TestMethod]
    public void Dispose_DisposeCalledTwice_NoExceptionThrown()
    {
        // Arrange
        var service = new NotificationService();
        // Act
        service.Dispose();
        // Assert
        // Should not throw if called again; Dispose must be idempotent.
        service.Dispose();
    }

    /// <summary>
    /// Ensures Notifications is always non-null and returns a valid IObservable.
    /// </summary>
    [TestMethod]
    public void Notifications_Always_ReturnsNonNullObservable()
    {
        // Arrange
        var service = new NotificationService();
        // Act
        var notifications = service.Notifications;
        // Assert
        Assert.IsNotNull(notifications, "Notifications property must always return a non-null IObservable.");
    }

    /// <summary>
    /// Verifies that no messages are observed if Send is never called.
    /// </summary>
    [TestMethod]
    public void Notifications_NoSend_NoMessagesObserved()
    {
        // Arrange
        var service = new NotificationService();
        bool messageObserved = false;
        // Act
        using var subscription = service.Notifications.Subscribe(_ => messageObserved = true);
        // Assert
        Assert.IsFalse(messageObserved, "No messages should be observed if Send is not called.");
    }

}