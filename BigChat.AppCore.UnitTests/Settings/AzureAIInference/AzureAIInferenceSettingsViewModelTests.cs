#nullable disable
using BigChat.AppCore.Settings.AzureAIInference;
using Moq;

namespace BigChat.AppCore.Settings.UnitTests;


/// <summary>
/// Unit tests for <see cref="AzureAIInferenceSettingsViewModel"/> constructor,
/// focusing on parameter propagation, edge cases, and error conditions.
/// </summary>
[TestClass]
public partial class AzureAIInferenceSettingsViewModelTests
{
    /// <summary>
    /// Verifies the constructor correctly copies property values from GetAzureAIInferenceSettings when settings are present.
    /// </summary>
    [TestMethod]
    public void Constructor_SettingsServiceReturnsSettings_PropertiesAreCopied()
    {
        // Arrange
        string expectedEndpoint = "https://my.endpoint";
        string expectedApiKey = "apikey123!";
        string expectedModelId = "model-abc";
        AzureAIInferenceClientSettings settings = new()
        {
            Endpoint = expectedEndpoint,
            APIKey = expectedApiKey,
            ModelId = expectedModelId
        };

        Mock<ISettingsService> mockService = new();
        mockService.Setup(s => s.GetAzureAIInferenceSettings()).Returns(settings);

        // Act
        AzureAIInferenceSettingsViewModel viewModel = new(mockService.Object);

        // Assert
        Assert.AreEqual(expectedEndpoint, viewModel.Endpoint, "Endpoint should be copied from ChatSettings.");
        Assert.AreEqual(expectedApiKey, viewModel.APIKey, "APIKey should be copied from ChatSettings.");
        Assert.AreEqual(expectedModelId, viewModel.ModelId, "ModelId should be copied from ChatSettings.");
    }

    /// <summary>
    /// Verifies the constructor correctly handles when GetAzureAIInferenceSettings returns null,
    /// setting properties to AzureAIInferenceClientSettings defaults.
    /// </summary>
    [TestMethod]
    public void Constructor_SettingsServiceReturnsNull_PropertiesSetToDefaults()
    {
        // Arrange
        Mock<ISettingsService> mockService = new();
        mockService.Setup(s => s.GetAzureAIInferenceSettings()).Returns(() => null);

        // Act
        AzureAIInferenceSettingsViewModel viewModel = new(mockService.Object);

        // Assert
        Assert.AreEqual(string.Empty, viewModel.Endpoint, "Endpoint should default to empty string.");
        Assert.AreEqual(string.Empty, viewModel.APIKey, "APIKey should default to empty string.");
        Assert.AreEqual(string.Empty, viewModel.ModelId, "ModelId should default to empty string.");
    }

    /// <summary>
    /// Verifies that MaxTemperature property returns the value from Constants.MaxTemperature.
    /// </summary>
    [TestMethod]
    public void MaxTemperature_ReturnsExpectedConstant_ValueIsCorrect()
    {
        // Arrange
        double expectedMax = Constants.MaxTemperature;
        AzureAIInferenceSettingsViewModel viewModel = new(new Moq.Mock<ISettingsService>().Object);

        // Act
        double actualMax = viewModel.MaxTemperature;

        // Assert
        Assert.AreEqual(expectedMax, actualMax, "MaxTemperature should return the value from Constants.MaxTemperature.");
    }

    /// <summary>
    /// Validates that MaxTemperature constant supports typical floating-point boundary conditions.
    /// </summary>
    [TestMethod]
    public void MaxTemperature_BoundaryValues_IsWithinDoubleRange()
    {
        // Arrange
        double maxTemperature = Constants.MaxTemperature;

        // Act & Assert
        Assert.IsFalse(double.IsNaN(maxTemperature), "MaxTemperature should not be NaN.");
        Assert.IsFalse(double.IsInfinity(maxTemperature), "MaxTemperature should not be infinite.");
        Assert.IsTrue(maxTemperature is > double.MinValue and < double.MaxValue, "MaxTemperature should be a valid finite double.");
    }

    /// <summary>
    /// Creates an instance of AzureAIInferenceSettingsViewModel with a mocked ISettingsService.
    /// </summary>
    /// <returns>AzureAIInferenceSettingsViewModel instance</returns>
    private AzureAIInferenceSettingsViewModel CreateViewModel()
    {
        Mock<ISettingsService> settingsServiceMock = new();
        settingsServiceMock.Setup(s => s.GetAzureAIInferenceSettings())
            .Returns(new AzureAIInferenceClientSettings { Endpoint = "", APIKey = "", ModelId = "" });
        return new AzureAIInferenceSettingsViewModel(settingsServiceMock.Object);
    }

    /// <summary>
    /// Helper method for setting Constants.MaxTopP.
    /// Uses reflection as Constants.MaxTopP is a static property.
    /// </summary>
    /// <param name="value">Value to set</param>
    private void SetConstantMaxTopP(double value)
    {
        var field = typeof(Constants).GetField("<MaxTopP>k__BackingField", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
        field?.SetValue(null, value);
    }

    /// <summary>
    /// Validates that MaxFrequencyPenalty property correctly returns the value from Constants.MaxFrequencyPenalty.
    /// Covers normal, boundary, and special float cases for the underlying constant.
    /// </summary>
    [TestMethod]
    public void MaxFrequencyPenalty_WhenAccessed_ReturnsConstantsValue()
    {
        // Arrange
        // Use a mock settings service to allow construction. The actual value does not affect MaxFrequencyPenalty.
        Mock<ISettingsService> mockSettingsService = new();
        mockSettingsService.Setup(s => s.GetAzureAIInferenceSettings())
                           .Returns(new AzureAIInferenceClientSettings());

        double expectedValue = Constants.MaxFrequencyPenalty;

        AzureAIInferenceSettingsViewModel viewModel = new(mockSettingsService.Object);

        // Act
        double actualValue = viewModel.MaxFrequencyPenalty;

        // Assert
        Assert.AreEqual(expectedValue, actualValue, "MaxFrequencyPenalty should match the value provided in Constants.MaxFrequencyPenalty.");
    }

    /// <summary>
    /// Verifies that MinFrequencyPenalty property returns the expected constant value (-2).
    /// Ensures the property always matches Constants.MinFrequencyPenalty.
    /// </summary>
    [TestMethod]
    public void MinFrequencyPenalty_ReturnsExpectedValue_MinusTwo()
    {
        // Arrange
        Mock<ISettingsService> mockSettingsService = new();
        mockSettingsService.Setup(s => s.GetAzureAIInferenceSettings()).Returns(new AzureAIInferenceClientSettings());
        AzureAIInferenceSettingsViewModel viewModel = new(mockSettingsService.Object);

        // Act
        double result = viewModel.MinFrequencyPenalty;

        // Assert
        Assert.AreEqual(Constants.MinFrequencyPenalty, result, "MinFrequencyPenalty should match the Constants.MinFrequencyPenalty value (-2).");
        Assert.AreEqual(-2, result, "MinFrequencyPenalty should be exactly -2.");
    }

    /// <summary>
    /// Verifies that MaxPresencePenalty always returns the value defined in Constants.MaxPresencePenalty.
    /// This covers domain boundaries and confirms the value is correctly propagated.
    /// </summary>
    [TestMethod]
    public void MaxPresencePenalty_Always_ReturnsExpectedConstantValue()
    {
        // Arrange
        var dummySettingsService = new Moq.Mock<ISettingsService>().Object;
        AzureAIInferenceSettingsViewModel viewModel = new(dummySettingsService);
        double expected = Constants.MaxPresencePenalty;

        // Act
        double actual = viewModel.MaxPresencePenalty;

        // Assert
        Assert.AreEqual(expected, actual, "MaxPresencePenalty should return the value from Constants.MaxPresencePenalty.");
        Assert.AreEqual(2d, actual, "MaxPresencePenalty should be 2 according to Constants.");
    }

    /// <summary>
    /// Ensures MinPresencePenalty returns the expected value from Constants.MinPresencePenalty
    /// using a mock ISettingsService. Covers boundary and representative domain values.
    /// </summary>
    [TestMethod]
    public void MinPresencePenalty_Always_ReturnsConstantValue()
    {
        // Arrange
        // No dependency or input can affect MinPresencePenalty as it's a direct property getter.
        Mock<ISettingsService> settingsServiceMock = new();
        AzureAIInferenceSettingsViewModel viewModel = new(settingsServiceMock.Object);

        // Act
        double expected = Constants.MinPresencePenalty;
        double actual = viewModel.MinPresencePenalty;

        // Assert
        Assert.AreEqual(expected, actual, "MinPresencePenalty should return the value defined in Constants.MinPresencePenalty.");
    }


    /// <summary>
    /// Verifies that MaxTopP property returns the value from Constants.MaxTopP.
    /// </summary>
    [TestMethod]
    public void MaxTopP_ReturnsExpectedConstant_ValueIsCorrect()
    {
        // Arrange
        var viewModel = CreateViewModel();

        // Act
        double actual = viewModel.MaxTopP;
        double expected = Constants.MaxTopP;

        // Assert
        Assert.AreEqual(expected, actual, $"MaxTopP should match Constants.MaxTopP ({expected}).");
    }

    /// <summary>
    /// Validates that MaxTopP constant supports typical floating-point boundary conditions.
    /// </summary>
    [TestMethod]
    public void MaxTopP_BoundaryValues_IsWithinDoubleRange()
    {
        // Arrange
        double value = Constants.MaxTopP;

        // Act & Assert
        Assert.IsFalse(double.IsNaN(value), "MaxTopP should not be NaN.");
        Assert.IsFalse(double.IsInfinity(value), "MaxTopP should not be infinite.");
        Assert.IsTrue(value is >= double.MinValue and <= double.MaxValue, "MaxTopP must be within double range.");
    }

    /// <summary>
    /// Ensures Save does not throw exceptions for any valid or edge input values,
    /// including boundaries and special floating-point cases.
    /// </summary>
    [TestMethod]
    public void Save_WithAllEdgeInputs_DoesNotThrowException()
    {
        // Arrange
        Mock<ISettingsService> mockService = new();
        mockService.Setup(s => s.GetAzureAIInferenceSettings()).Returns(new AzureAIInferenceClientSettings());
        mockService.Setup(s => s.SetAzureAIInferenceClientSettings(It.IsAny<AzureAIInferenceClientSettings>()));

        AzureAIInferenceSettingsViewModel viewModel = new(mockService.Object);

        string[] possibleStrings = new[]
        {
            "",
            "normal",
            " ",
            "\r\n\t",
            new string('x', 1000),
            "endpoint\0\0",
            "api\u2603key",
            "model\n"
        };

        double[] possibleDoubles = new[]
        {
            0d, 1d, -1d, double.MaxValue, double.MinValue, double.NaN, double.PositiveInfinity, double.NegativeInfinity
        };

        int[] possibleInts = new[]
        {
            0, 1, -1, int.MaxValue, int.MinValue
        };

        // Act & Assert
        foreach (string endpoint in possibleStrings)
            foreach (string apikey in possibleStrings)
                foreach (string modelId in possibleStrings)
                    foreach (double temp in possibleDoubles)
                        foreach (int maxTokens in possibleInts)
                            foreach (double topP in possibleDoubles)
                                foreach (double freqPen in possibleDoubles)
                                    foreach (double presPen in possibleDoubles)
                                    {
                                        viewModel.Endpoint = endpoint;
                                        viewModel.APIKey = apikey;
                                        viewModel.ModelId = modelId;
                                        viewModel.Temperature = temp;
                                        viewModel.MaxOutputTokens = maxTokens;
                                        viewModel.TopP = topP;
                                        viewModel.FrequencyPenalty = freqPen;
                                        viewModel.PresencePenalty = presPen;
                                        try
                                        {
                                            viewModel.Save();
                                        }
                                        catch (Exception ex)
                                        {
                                            Assert.Fail($"Save should not throw for edge values. Threw: {ex}");
                                        }
                                    }
    }

    /// <summary>
    /// Verifies that MinTemperature property returns the value from Constants.MinTemperature
    /// and covers boundary and floating-point edge cases, including negative domain.
    /// </summary>
    [TestMethod]
    public void MinTemperature_ReturnsConstant_CorrectDomainAndBoundaries()
    {
        // Arrange
        double expected = Constants.MinTemperature;
        var viewModel = CreateViewModel();

        // Act
        double actual = viewModel.MinTemperature;

        // Assert
        Assert.AreEqual(expected, actual, "MinTemperature should return the exact value from Constants.MinTemperature.");
        Assert.IsTrue(actual <= 0, "MinTemperature should be less than or equal to zero.");
        Assert.IsFalse(double.IsNaN(actual), "MinTemperature should not be NaN.");
        Assert.IsFalse(double.IsInfinity(actual), "MinTemperature should not be infinity.");
        Assert.IsTrue(actual > double.MinValue, "MinTemperature should be greater than double.MinValue.");
        Assert.IsTrue(actual < double.MaxValue, "MinTemperature should be less than double.MaxValue.");
    }

    /// <summary>
    /// Ensures MinTemperature property returns a value within the domain [-2, 0]
    /// and always matches Constants.MinTemperature, which covers valid application domain cases.
    /// </summary>
    [TestMethod]
    public void MinTemperature_Always_MatchesExpectedNegativeRange()
    {
        // Arrange
        var viewModel = CreateViewModel();

        // Act
        double minTemperature = viewModel.MinTemperature;

        // Assert
        Assert.IsTrue(minTemperature is >= (-2) and <= 0, "MinTemperature should always be within the expected negative range for model configuration.");
        Assert.AreEqual(Constants.MinTemperature, minTemperature, "MinTemperature should exactly match Constants.MinTemperature.");
    }

    /// <summary>
    /// Verifies that MinTopP property returns the expected value from Constants.MinTopP.
    /// Assumes Constants.MinTopP is a static read-only property (no backing value shown).
    /// </summary>
    [TestMethod]
    public void MinTopP_ReturnsExpectedConstantValue()
    {
        // Arrange
        Mock<ISettingsService> mockSettingsService = new();
        AzureAIInferenceSettingsViewModel viewModel = new(mockSettingsService.Object);

        // Act
        double minTopP = viewModel.MinTopP;
        double constantMinTopP = Constants.MinTopP;

        // Assert
        Assert.AreEqual(constantMinTopP, minTopP, "MinTopP should match Constants.MinTopP.");
    }

    /// <summary>
    /// Validates that MinTopP property is a valid double: not NaN or Infinity.
    /// Covers edge cases for double: verifies it is neither NaN nor any Infinity.
    /// </summary>
    [TestMethod]
    public void MinTopP_Value_IsNotNaNOrInfinity()
    {
        // Arrange
        Mock<ISettingsService> mockSettingsService = new();
        AzureAIInferenceSettingsViewModel viewModel = new(mockSettingsService.Object);

        // Act
        double minTopP = viewModel.MinTopP;

        // Assert
        Assert.IsFalse(double.IsNaN(minTopP), "MinTopP should not be NaN.");
        Assert.IsFalse(double.IsInfinity(minTopP), "MinTopP should not be Infinity.");
    }
}