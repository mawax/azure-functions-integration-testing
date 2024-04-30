using System.Text.Json;
using Azure.Messaging.ServiceBus;
using ExampleFunctionApp.Messages;
using ExampleFunctionApp.Requests;
using ExampleFunctionApp.Validators;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace ExampleFunctionApp.Functions;

public class ServiceBusTriggerFunction
{
    private readonly ILogger<ServiceBusTriggerFunction> _logger;
    private readonly IExampleValidator _exampleValidator;

    public ServiceBusTriggerFunction(
        ILogger<ServiceBusTriggerFunction> logger,
        IExampleValidator exampleValidator)
    {
        _logger = logger;
        _exampleValidator = exampleValidator;
    }

    [Function(nameof(ServiceBusTriggerFunction))]
    [ServiceBusOutput("my-out-queue", Connection = "ServiceBusConnectionString")]
    public async Task<string?> Run(
        [ServiceBusTrigger(
            "my-in-queue",
            Connection = "ServiceBusConnectionString",
            AutoCompleteMessages = true)]
        ServiceBusReceivedMessage message,
        ServiceBusMessageActions messageActions)
    {
        var parsedMessage = message.Body.ToObjectFromJson<ExampleInMessage>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        );

        if (!_exampleValidator.Validate(parsedMessage))
        {
            await messageActions.DeadLetterMessageAsync(
                message,
                deadLetterReason: "Validation failed: invalid message body");
            return null;
        }

        var response = new ExampleOutMessage(parsedMessage.ItemId);
        return JsonSerializer.Serialize(response);
    }
}
