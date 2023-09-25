using Testcontainers.MongoDb;

namespace Presentation.Tests.IntegrationTests.TestInfra;

public class MongoFixture : IAsyncLifetime
{
    private MongoDbContainer? _container;

    public string ConnectionString =>
        _container?.GetConnectionString()
        ?? throw new InvalidOperationException("The container was not created.");
    public string Database { get; } = $"IntegrationTest_{Guid.NewGuid()}";

    public Task InitializeAsync()
    {
        _container = new MongoDbBuilder()
            .WithImage("mongo:5.0.4")
            .WithUsername("integrationtest")
            .WithPassword("IntegrationTesting101")
            .Build();

        return _container.StartAsync();
    }

    public Task DisposeAsync()
    {
        return _container?.StopAsync() ?? Task.CompletedTask;
    }
}
