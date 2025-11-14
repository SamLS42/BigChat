using System;
using System.Collections.Generic;
using System.Linq;

using BigChat.AppCore;
using BigChat.AppCore.ChatClients;
using BigChat.AppCore.Conversations;
using BigChat.AppCore.MainPage;
using BigChat.AppCore.Notifications;
using BigChat.AppCore.Services;
using BigChat.AppCore.Settings;
using BigChat.AppCore.Settings.OpenAI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace BigChat.AppCore.UnitTests;

/// <summary>
/// Unit tests for <see cref = "RegistrationExtensions"/>.
/// Verifies correct DI registration and resolution for AddCoreServices extension.
/// </summary>
[TestClass]
public class RegistrationExtensionsTests
{
    /// <summary>
    /// Verifies that AddCoreServices returns the same instance as passed and all expected service registrations are present.
    /// </summary>
    [TestMethod]
    public void AddCoreServices_ServiceCollection_ReturnsSameInstanceAndRegistersExpectedServices()
    {
        // Arrange
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddSingleton<ISettingsService>(new Mock<ISettingsService>().Object);

        // Act
        var returned = RegistrationExtensions.AddCoreServices(serviceCollection);

        // Assert
        Assert.AreSame(serviceCollection, returned, "Should return the same IServiceCollection instance passed in.");

        // Verify singleton registrations
        Assert.IsTrue(serviceCollection.Any(sd =>
            sd.ServiceType == typeof(SubjectResolver) && sd.Lifetime == ServiceLifetime.Singleton),
            "SubjectResolver should be registered as singleton.");
        Assert.IsTrue(serviceCollection.Any(sd =>
            sd.ServiceType == typeof(NotificationService) && sd.Lifetime == ServiceLifetime.Singleton),
            "NotificationService should be registered as singleton.");
        Assert.IsTrue(serviceCollection.Any(sd =>
            sd.ServiceType == typeof(DataService) && sd.Lifetime == ServiceLifetime.Singleton),
            "DataService should be registered as singleton.");
        Assert.IsTrue(serviceCollection.Any(sd =>
            sd.ServiceType == typeof(ConversationOperationsService) && sd.Lifetime == ServiceLifetime.Singleton),
            "ConversationOperationsService should be registered as singleton.");

        // Verify transient registrations
        Assert.IsTrue(serviceCollection.Any(sd =>
            sd.ServiceType == typeof(MainPageViewModel) && sd.Lifetime == ServiceLifetime.Transient),
            "MainPageViewModel should be registered as transient.");
        Assert.IsTrue(serviceCollection.Any(sd =>
            sd.ServiceType == typeof(ConversationViewModel) && sd.Lifetime == ServiceLifetime.Transient),
            "ConversationViewModel should be registered as transient.");
        Assert.IsTrue(serviceCollection.Any(sd =>
            sd.ServiceType == typeof(AzureAIInferenceSettingsViewModel) && sd.Lifetime == ServiceLifetime.Transient),
            "AzureAIInferenceSettingsViewModel should be registered as transient.");
        Assert.IsTrue(serviceCollection.Any(sd =>
            sd.ServiceType == typeof(OpenAISettingsViewModel) && sd.Lifetime == ServiceLifetime.Transient),
            "OpenAISettingsViewModel should be registered as transient.");

        // Verify scoped registration
        Assert.IsTrue(serviceCollection.Any(sd =>
            sd.ServiceType == typeof(OllamaSettingsViewModel) && sd.Lifetime == ServiceLifetime.Scoped),
            "OllamaSettingsViewModel should be registered as scoped.");
    }

}