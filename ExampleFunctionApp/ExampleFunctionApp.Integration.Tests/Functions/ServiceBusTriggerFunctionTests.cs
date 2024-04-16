
using Azure.Messaging.ServiceBus;
using ExampleFunctionApp.Functions;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace ExampleFunctionApp.Integration.Tests.Functions;

[Collection("FunctionApp")]
public class ServiceBusTriggerFunctionTests
{
    private readonly FunctionAppFixture _functionAppFixture;

    public ServiceBusTriggerFunctionTests(FunctionAppFixture functionAppFixture)
    {
        _functionAppFixture = functionAppFixture;
    }

    [Fact]
    public async Task ReceiveEvent_InvalidMessageBody_DeadletterQueuesMessage()
    {
        // Arrange
        var sut = ActivatorUtilities.CreateInstance<ServiceBusTriggerFunction>(
            _functionAppFixture.ServiceProvider);

        var messageBody = """
            {
                "invalid": "message"
            }
            """;

        var message = ServiceBusModelFactory.ServiceBusReceivedMessage(
            body: new BinaryData(messageBody));

        var messageActions = Substitute.For<ServiceBusMessageActions>();

        // Act
        await sut.Run(message, messageActions);

        // Assert
        await messageActions.Received().DeadLetterMessageAsync(
            message,
            deadLetterReason: "Invalid message body");
    }

    [Fact]
    public async Task ReceiveEvent_ValidMessageBody_CompletedMessage()
    {
        // Arrange
        var sut = ActivatorUtilities.CreateInstance<ServiceBusTriggerFunction>(
            _functionAppFixture.ServiceProvider);

        var messageBody = """
            {
                "id": 1000,
                "category": "Books"
            }
            """;

        var message = ServiceBusModelFactory.ServiceBusReceivedMessage(
            body: new BinaryData(messageBody));

        var messageActions = Substitute.For<ServiceBusMessageActions>();

        // Act
        await sut.Run(message, messageActions);

        // Assert
        await messageActions.Received().CompleteMessageAsync(message);
    }
}