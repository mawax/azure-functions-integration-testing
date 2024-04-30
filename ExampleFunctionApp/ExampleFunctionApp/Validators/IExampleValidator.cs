using ExampleFunctionApp.Requests;

namespace ExampleFunctionApp.Validators;

public interface IExampleValidator
{
    bool Validate(ExampleInMessage message);
}
