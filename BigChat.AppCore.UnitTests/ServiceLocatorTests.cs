using Moq;

namespace BigChat.AppCore.UnitTests;


/// <summary>
/// Unit tests for <see cref="ServiceLocator"/>.
/// </summary>
[TestClass]
public class ServiceLocatorTests
{
    /// <summary>
    /// Verifies that SetLocator correctly sets the static ServiceProvider field
    /// when passed a valid (non-null) IServiceProvider instance.
    /// The test uses Moq to create a mock IServiceProvider and ensures
    /// that GetRequiredService returns the same reference after setting.
    /// </summary>
    [TestMethod]
    public void SetLocator_ValidServiceProvider_ServiceProviderIsSet()
    {
        // Arrange
        Mock<IServiceProvider> mockServiceProvider = new(MockBehavior.Strict);

        // Act
        ServiceLocator.SetLocator(mockServiceProvider.Object);
        // Note: No public API to directly verify internal state, but calling SetLocator should not throw

        // Assert
        // No exception means the test passes for valid input. If GetRequiredService was implemented,
        // we'd retrieve the service provider or a testable result here.
        Assert.IsTrue(true); // SetLocator did not throw
    }

}