using ExampleFunctionApp.Requests;

namespace ExampleFunctionApp.Validators;

public class ExampleValidator : IExampleValidator
{
    public bool ValidateExampleMessage(ExampleMessage message)
    {
        return message.Category is not null;
    }
}