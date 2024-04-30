
using Azure.Messaging.ServiceBus;
using ExampleFunctionApp.Functions;
using ExampleFunctionApp.Messages;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using System.Text.Json;

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
    public async Task ReceiveEvent_InvalidMessageBody_FailsValidation()
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
        var result = await sut.Run(message, messageActions);

        // Assert
        Assert.Null(result);

        await messageActions.Received().DeadLetterMessageAsync(
            message,
            deadLetterReason: Arg.Is<string>(r => r.Contains("Validation failed")));
    }

    [Fact]
    public async Task ReceiveEvent_ValidMessageBody_RepondsWithOutMessage()
    {
        // Arrange
        var sut = ActivatorUtilities.CreateInstance<ServiceBusTriggerFunction>(
            _functionAppFixture.ServiceProvider);

        var messageBody = """
            {
                "itemId": 1000,
                "category": "Books"
            }
            """;

        var message = ServiceBusModelFactory.ServiceBusReceivedMessage(
            body: new BinaryData(messageBody));

        var messageActions = Substitute.For<ServiceBusMessageActions>();

        // Act
        var result = await sut.Run(message, messageActions);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(
            """
            {"ItemId":1000}
            """,
            result);

    }
}