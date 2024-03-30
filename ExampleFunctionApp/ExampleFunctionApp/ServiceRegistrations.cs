using ExampleFunctionApp.Validators;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;

namespace ExampleFunctionApp;

public static class ServiceRegistrations
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();

        services.AddTransient<IExampleValidator, ExampleValidator>();

        return services;
    }
}
