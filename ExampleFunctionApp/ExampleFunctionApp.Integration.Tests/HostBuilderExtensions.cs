using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ExampleFunctionApp.Integration.Tests;

public static class HostBuilderExtensions
{
    public static IHostBuilder ConfigureTestHost(this IHostBuilder hostBuilder)
    {
        hostBuilder.ConfigureLogging(logging =>
        {
            logging.ClearProviders();
        });

        hostBuilder.ConfigureServices(services =>
        {
            // Remove internal class WorkerHostedService to prevent unconfigured gRPC exception: 
            // "gRPC channel URI 'http://:63425' could not be parsed."

            // Removing this hosted service does mean triggers such as Storage Queue Triggers stop working:
            // See https://github.com/Azure/azure-functions-dotnet-worker/issues/968
            var hostedService = services.First(
                descriptor => descriptor.ImplementationType?.Name == "WorkerHostedService");
            services.Remove(hostedService);
        });

        return hostBuilder;
    }
}
