using ExampleFunctionApp.Requests;

namespace ExampleFunctionApp.Validators;

public interface IExampleValidator
{
    bool ValidateExampleMessage(ExampleMessage message);
}
