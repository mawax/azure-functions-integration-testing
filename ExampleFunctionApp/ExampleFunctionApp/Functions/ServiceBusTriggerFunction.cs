using System;
using System.Text.Json;
using Azure.Messaging.ServiceBus;
using ExampleFunctionApp.Requests;
using ExampleFunctionApp.Validators;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace ExampleFunctionApp.Functions
{
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
        public async Task Run(
            [ServiceBusTrigger(
                "myqueue",
                Connection = "ServiceBusConnectionString",
                AutoCompleteMessages = false)]
            ServiceBusReceivedMessage message,
            ServiceBusMessageActions messageActions)
        {
            var parsedMessage = message.Body.ToObjectFromJson<ExampleMessage>(
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );

            if (!_exampleValidator.ValidateExampleMessage(parsedMessage))
            {
                await messageActions.DeadLetterMessageAsync(
                    message,
                    deadLetterReason: $"Invalid message body");
                return;
            }

            await messageActions.CompleteMessageAsync(message);
        }
    }
}
