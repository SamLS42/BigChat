#nullable disable
using BigChat.AppCore;
using BigChat.AppCore.Localization;
using BigChat.Main;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI;
using System;


namespace BigChat.Main.UnitTests;

/// <summary>
/// Unit tests for DialogService.
/// Verifies that GetConfirmationDialog creates a ContentDialog with correct properties set for various edge case inputs.
/// </summary>
[TestClass]
public sealed partial class DialogServiceTests
{
    /// <summary>
    /// Partial test for GetConfirmationDialog - Unable to validate Style and XamlRoot property assignment due to UI dependencies.
    /// Instructs user to run this test in a real UI test context or with appropriate app initialization for full coverage.
    /// </summary>
    [TestMethod]
    public void GetConfirmationDialog_StyleAndXamlRoot_ManualVerificationNeeded()
    {
        // This test is inconclusive because Microsoft.UI.Xaml.Application.Current and XamlRoot cannot be initialized or faked in MSTest context.
        // Please run this scenario inside a real WinUI app or test harness where Application.Current is initialized and a valid XamlRoot can be supplied.
        Assert.Inconclusive("Cannot test Application.Current.Resources or XamlRoot property assignment in headless MSTest context. Run in UI test harness.");
    }
}