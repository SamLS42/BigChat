#nullable disable
using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;

using BigChat.AppCore;
using BigChat.AppCore.ViewModel;
using Microsoft.Extensions;
using Microsoft.Extensions.AI;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ReactiveUI;
using ReactiveUI.SourceGenerators;

namespace BigChat.AppCore.ViewModel.UnitTests;


/// <summary>
/// Unit tests for MessageViewModel.IsThinking property.
/// </summary>
[TestClass]
public partial class MessageViewModelTests
{
    /// <summary>
    /// Tests IsThinking with extremely long strings in ThinkContent and Content properties.
    /// Ensures correct evaluation and performance with large input values.
    /// </summary>
    [TestMethod]
    public void IsThinking_LongStrings_HandledCorrectly()
    {
        // Arrange
        var longString = new string('a', 10000);
        var vm = new MessageViewModel
        {
            ThinkContent = longString,
            Content = longString,
            IsPending = false
        };

        // Act
        var actual = vm.IsThinking;

        // Assert
        // Since HasThink is true, but Content is NOT empty, and IsPending is false: should be false
        Assert.IsFalse(actual);
    }

    /// <summary>
    /// Tests IsThinking with special characters and whitespace in Content and ThinkContent.
    /// </summary>
    [TestMethod]
    public void IsThinking_SpecialCharactersAndWhitespace_HandledCorrectly()
    {
        // Arrange
        var vm = new MessageViewModel
        {
            ThinkContent = "\r\n\t", // whitespace
            Content = "\u0000\u263A", // null char, unicode smiley
            IsPending = true
        };

        // Act
        var actual = vm.IsThinking;

        // Assert
        // IsPending is true so result must be true regardless of other properties.
        Assert.IsTrue(actual);
    }

    /// <summary>
    /// Verifies HasThink returns true when ThinkContent is a non-empty, non-whitespace string.
    /// Expected: HasThink is true for any string with visible characters.
    /// </summary>
    [TestMethod]
    public void HasThink_NonEmptyThinkContent_ReturnsTrue()
    {
        // Arrange
        var vm = new MessageViewModel();

        var testInputs = new[] { "test", "1", "a", "你好", "Hello\nWorld", "    .", new string('X', 10000), "!@#$%^&*()" };
        foreach (var input in testInputs)
        {
            vm.ThinkContent = input;

            // Act
            var result = vm.HasThink;

            // Assert
            Assert.IsTrue(result, $"Expected HasThink to be true for ThinkContent '{input}'");
        }
    }

    /// <summary>
    /// Verifies DisplayContent updates reactively when ThinkContent, Content, or IsPending change at runtime.
    /// This ensures dynamic changes trigger correct DisplayContent value.
    /// </summary>
    [TestMethod]
    public void DisplayContent_DynamicPropertyChanges_ReflectsLatestState()
    {
        // Arrange
        var vm = new MessageViewModel();
        vm.ThinkContent = "a";
        vm.Content = "";
        vm.IsPending = false;

        // Act & Assert
        // Should be "<Thinking...>" because HasThink = true and Content = ""
        Assert.AreEqual("<Thinking...>", vm.DisplayContent);

        // Change Content to non-empty, IsThinking = false, should show Content
        vm.Content = "msg";
        Assert.AreEqual("msg", vm.DisplayContent);

        // Set IsPending true, should show "<Thinking...>"
        vm.IsPending = true;
        Assert.AreEqual("<Thinking...>", vm.DisplayContent);

        // Clear ThinkContent and IsPending, Content not empty; should show Content
        vm.ThinkContent = "";
        vm.IsPending = false;
        Assert.AreEqual("msg", vm.DisplayContent);
    }

    /// <summary>
    /// Verifies DisplayContent is initialized correctly for default property values.
    /// </summary>
    [TestMethod]
    public void DisplayContent_DefaultValues_ReturnsExpected()
    {
        // Arrange
        var vm = new MessageViewModel();

        // Act
        string value = vm.DisplayContent;

        // Assert
        Assert.AreEqual(string.Empty, value);
    }

    /// <summary>
    /// Ensures that a newly created MessageViewModel has correct default values
    /// and computed properties reflect the expected initial state.
    /// </summary>
    [TestMethod]
    public void Constructor_DefaultInitialization_ComputedPropertiesReflectDefaultState()
    {
        // Arrange & Act
        var vm = new MessageViewModel();

        // Assert
        Assert.AreEqual(string.Empty, vm.Content, "Content should be initialized to empty string.");
        Assert.AreEqual(string.Empty, vm.ThinkContent, "ThinkContent should be initialized to empty string.");
        Assert.AreEqual(string.Empty, vm.EditText, "EditText should be initialized to empty string.");
        Assert.IsFalse(vm.IsPending, "IsPending should be false by default.");
        Assert.IsFalse(vm.HasThink, "HasThink should be false when ThinkContent is empty.");
        Assert.IsFalse(vm.IsThinking, "IsThinking should be false when HasThink is false and IsPending is false.");
        Assert.AreEqual(string.Empty, vm.DisplayContent, "DisplayContent should equal Content when IsThinking is false.");
    }

    /// <summary>
    /// Verifies HasThink is true for ThinkContent set to a non-empty, non-whitespace string.
    /// </summary>
    [TestMethod]
    public void Constructor_ThinkContentNonWhitespace_HasThinkTrue()
    {
        // Arrange
        var vm = new MessageViewModel();
        vm.ThinkContent = "Some thinking...";

        // Act
        var result = vm.HasThink;

        // Assert
        Assert.IsTrue(result, "HasThink should be true for non-empty, non-whitespace ThinkContent.");
    }

    /// <summary>
    /// Ensures HasThink is false for ThinkContent values: null, empty, whitespace.
    /// </summary>
    [TestMethod]
    public void Constructor_ThinkContentNullOrWhitespace_HasThinkFalse()
    {
        // Arrange
        var vm = new MessageViewModel();

        // Test for empty string
        vm.ThinkContent = "";
        Assert.IsFalse(vm.HasThink, "HasThink should be false for empty ThinkContent.");

        // Test for whitespace string
        vm.ThinkContent = "    ";
        Assert.IsFalse(vm.HasThink, "HasThink should be false for whitespace ThinkContent.");

        // Test for null
        vm.ThinkContent = null!;
        Assert.IsFalse(vm.HasThink, "HasThink should be false for null ThinkContent.");
    }

    /// <summary>
    /// Ensures IsThinking is true when HasThink is true and Content is null or empty.
    /// </summary>
    [TestMethod]
    public void Constructor_HasThinkTrueAndContentEmptyOrNull_IsThinkingTrue()
    {
        // Arrange
        var vm = new MessageViewModel();
        vm.ThinkContent = "not empty"; // HasThink = true

        // Content = empty
        vm.Content = "";
        Assert.IsTrue(vm.IsThinking, "IsThinking should be true if HasThink is true and Content is empty.");

        // Content = null
        vm.Content = null!;
        Assert.IsTrue(vm.IsThinking, "IsThinking should be true if HasThink is true and Content is null.");
    }

    /// <summary>
    /// Ensures IsThinking is true when IsPending is true, regardless of HasThink or Content.
    /// </summary>
    [TestMethod]
    public void Constructor_IsPendingTrue_IsThinkingTrue()
    {
        // Arrange
        var vm = new MessageViewModel();

        // Various ThinkContent/Content values with IsPending = true
        vm.IsPending = true;
        vm.ThinkContent = "";
        vm.Content = "something";
        Assert.IsTrue(vm.IsThinking, "IsThinking should be true when IsPending is true.");

        vm.ThinkContent = "abc";
        vm.Content = "";
        Assert.IsTrue(vm.IsThinking, "IsThinking should be true when IsPending is true.");
    }

    /// <summary>
    /// Ensures IsThinking is false if HasThink is false, Content is not empty, and IsPending is false.
    /// </summary>
    [TestMethod]
    public void Constructor_HasThinkFalseContentNotEmptyIsPendingFalse_IsThinkingFalse()
    {
        // Arrange
        var vm = new MessageViewModel();

        vm.ThinkContent = "";
        vm.Content = "Non-empty content";
        vm.IsPending = false;

        // Act & Assert
        Assert.IsFalse(vm.IsThinking, "IsThinking should be false if HasThink is false and Content is not empty and IsPending is false.");
    }

    /// <summary>
    /// Ensures DisplayContent shows "<Thinking...>" if IsThinking is true, otherwise Content.
    /// </summary>
    [TestMethod]
    public void Constructor_IsThinkingTrue_DisplayContentShowsThinking()
    {
        // Arrange
        var vm = new MessageViewModel();

        // IsThinking = true when IsPending = true
        vm.IsPending = true;
        vm.Content = "Should be hidden";

        // Act & Assert
        Assert.AreEqual("<Thinking...>", vm.DisplayContent, "DisplayContent should be '<Thinking...>' when IsThinking is true.");
    }

    /// <summary>
    /// Ensures DisplayContent returns Content if IsThinking is false.
    /// </summary>
    [TestMethod]
    public void Constructor_IsThinkingFalse_DisplayContentShowsContent()
    {
        // Arrange
        var vm = new MessageViewModel();

        // Default IsThinking is false
        vm.Content = "Message shown";
        vm.IsPending = false;
        vm.ThinkContent = "";

        // Act & Assert
        Assert.AreEqual("Message shown", vm.DisplayContent, "DisplayContent should show actual Content when IsThinking is false.");
    }

    /// <summary>
    /// Verifies that MessageUpdated returns an observable which emits a notification when a message update occurs.
    /// Test covers: subscription, emission, multiple emissions, and subscription disposal.
    /// </summary>
    [TestMethod]
    public void MessageUpdated_NotificationEmittedOnUpdate_ObservableNotifiesSubscribers()
    {
        // Arrange
        var vm = new MessageViewModel();
        int notificationCount = 0;
        var subscription = vm.MessageUpdated.Subscribe(_ => notificationCount++);

        // Act
        // Simulate message update by calling OnNext on the underlying subject.
        var subjectField = typeof(MessageViewModel)
            .GetProperty("MessageUpdatesSource", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
        var subject = subjectField?.GetValue(vm) as Subject<Unit>;
        subject?.OnNext(Unit.Default);
        subject?.OnNext(Unit.Default);

        // Assert
        Assert.AreEqual(2, notificationCount, "Expected subscriber to receive a notification for each update.");

        // Cleanup
        subscription.Dispose();
        notificationCount = 0;
        subject?.OnNext(Unit.Default);
        Assert.AreEqual(0, notificationCount, "No notification should occur after subscription is disposed.");
    }

    /// <summary>
    /// Verifies that multiple subscribers to MessageUpdated each receive notifications independently.
    /// </summary>
    [TestMethod]
    public void MessageUpdated_MultipleSubscribers_AllReceiveNotifications()
    {
        // Arrange
        var vm = new MessageViewModel();
        int firstCount = 0;
        int secondCount = 0;
        var sub1 = vm.MessageUpdated.Subscribe(_ => firstCount++);
        var sub2 = vm.MessageUpdated.Subscribe(_ => secondCount++);

        // Act
        var subjectField = typeof(MessageViewModel)
            .GetProperty("MessageUpdatesSource", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
        var subject = subjectField?.GetValue(vm) as Subject<Unit>;
        subject?.OnNext(Unit.Default);

        // Assert
        Assert.AreEqual(1, firstCount, "First subscriber should receive update notification.");
        Assert.AreEqual(1, secondCount, "Second subscriber should receive update notification.");

        // Cleanup
        sub1.Dispose();
        sub2.Dispose();
    }

    /// <summary>
    /// Ensures that MessageUpdated completes when the MessageUpdatesSource completes, and subscribers are notified.
    /// </summary>
    [TestMethod]
    public void MessageUpdated_ObservableCompleted_SubscriberIsCompleted()
    {
        // Arrange
        var vm = new MessageViewModel();
        bool isCompleted = false;
        var subscription = vm.MessageUpdated.Subscribe(_ => { }, () => isCompleted = true);

        // Act
        var subjectField = typeof(MessageViewModel)
            .GetProperty("MessageUpdatesSource", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
        var subject = subjectField?.GetValue(vm) as Subject<Unit>;
        subject?.OnCompleted();

        // Assert
        Assert.IsTrue(isCompleted, "Subscriber should be notified when observable completes.");

        // Cleanup
        subscription.Dispose();
    }

    /// <summary>
    /// Tests HasThink and IsThinking for all possible combinations of ThinkContent and Content including nulls, empty, whitespace, and large inputs.
    /// </summary>
    [TestMethod]
    public void Constructor_ThinkContentAndContent_EdgeCases_PropertiesAreCorrect()
    {
        // Arrange
        var longString = new string('A', 10000);
        var whitespaceString = " \t\r\n";

        var vm = new MessageViewModel
        {
            ThinkContent = longString,
            Content = longString,
            IsPending = false
        };

        // Act & Assert
        Assert.IsTrue(vm.HasThink, "HasThink should be true for very long ThinkContent.");
        Assert.IsFalse(vm.IsThinking, "IsThinking should be false if Content is not empty and IsPending is false.");

        vm.ThinkContent = whitespaceString;
        Assert.IsFalse(vm.HasThink, "HasThink should be false for whitespace ThinkContent.");
        Assert.IsFalse(vm.IsThinking, "IsThinking should be false when HasThink is false and Content is not empty.");

        vm.ThinkContent = null;
        Assert.IsFalse(vm.HasThink, "HasThink should be false for null ThinkContent.");

        vm.ThinkContent = "!@#$%^&*()";
        Assert.IsTrue(vm.HasThink, "HasThink should be true for special character ThinkContent.");
    }
}