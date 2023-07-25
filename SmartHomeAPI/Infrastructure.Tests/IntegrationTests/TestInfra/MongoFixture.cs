using Docker.DotNet.Models;
using Docker.DotNet;

namespace Infrastructure.Tests.IntegrationTests.TestInfra;
public class MongoFixture : IAsyncLifetime
{
    private const string MONGO_CONTAINER_NAME = "mongodb_integration";
    private const string MONGO_NETWORK_NAME = "integration_tests_network";
    private readonly DockerClient _dockerClient;

    public MongoFixture()
    {
        _dockerClient = new DockerClientConfiguration(new Uri("npipe://./pipe/docker_engine")).CreateClient();
    }

    public async Task InitializeAsync()
    {
        await CreateNetworkIfNotExistsAsync();
        await StartMongoContainerAsync();
    }

    public async Task DisposeAsync()
    {
        await StopMongoContainerAsync();
        _dockerClient.Dispose();
    }

    private async Task CreateNetworkIfNotExistsAsync()
    {
        var networks = await _dockerClient.Networks.ListNetworksAsync();
        foreach (var network in networks)
        {
            if (network.Name == MONGO_NETWORK_NAME)
            {
                return; 
            }
        }

        await _dockerClient.Networks.CreateNetworkAsync(new NetworksCreateParameters
        {
            Name = MONGO_NETWORK_NAME
        });
    }

    private async Task StartMongoContainerAsync()
    {
        await _dockerClient.Containers.CreateContainerAsync(new CreateContainerParameters
        {
            Image = "mongo",
            Name = MONGO_CONTAINER_NAME,
            HostConfig = new HostConfig
            {
                PortBindings = new Dictionary<string, IList<PortBinding>>
                    {
                        { "27017", new List<PortBinding> { new PortBinding { HostPort = "27017" } } }
                    },
                NetworkMode = MONGO_NETWORK_NAME 
            },
            ExposedPorts = new Dictionary<string, EmptyStruct>
                {
                    { "27017", new EmptyStruct() }
                }
        });

        await _dockerClient.Containers.StartContainerAsync(MONGO_CONTAINER_NAME, null);
    }

    private async Task StopMongoContainerAsync()
    {
        await _dockerClient.Containers.StopContainerAsync(MONGO_CONTAINER_NAME, new ContainerStopParameters());
        await _dockerClient.Containers.RemoveContainerAsync(MONGO_CONTAINER_NAME, new ContainerRemoveParameters { Force = true });
    }
}
