using Microsoft.Extensions.Hosting;

namespace ExampleFunctionApp.Integration.Tests;

public class FunctionAppFixture : IAsyncLifetime
{
    private readonly IHost _host;

    public IServiceProvider ServiceProvider => _host.Services;

    public FunctionAppFixture()
    {
        _host = new HostBuilder()
            .ConfigureFunctionsWorkerDefaults()
            .ConfigureServices(services => services.AddServices())
            .ConfigureTestHost()
            .Build();
    }

    public async Task InitializeAsync() => await _host.StartAsync();

    public Task DisposeAsync()
    {
        _host?.Dispose();
        return Task.CompletedTask;
    }
}
