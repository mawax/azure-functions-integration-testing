using ExampleFunctionApp.Requests;

namespace ExampleFunctionApp.Validators;

public class ExampleValidator : IExampleValidator
{
    public bool Validate(ExampleInMessage message)
    {
        // dummy validation logic
        return message.Category is not null;
    }
}