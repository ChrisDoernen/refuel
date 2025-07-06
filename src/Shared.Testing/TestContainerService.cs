using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Shared.Testing.EventSourcingDB;
using Shared.Testing.MongoDB;

namespace Shared.Testing;

public class TestContainerService : IHostedService, IAsyncDisposable
{
  private readonly IEnumerable<TestContainer> _testContainers;

  private readonly ILogger<TestContainerService> _logger;

  public TestContainerService(
    IConfiguration configuration,
    ILogger<TestContainerService> logger
  )
  {
    _logger = logger;

    var eventSourcingDbContainers = configuration.GetEventSourcingDbContainers();
    var mongoDbContainer = configuration.GetMongoDbContainer();

    _testContainers = [..eventSourcingDbContainers, mongoDbContainer];
  }

  public async Task StartAsync(CancellationToken cancellationToken)
  {
    _logger.LogInformation("Starting test containers");

    await _testContainers.Start();
  }

  public Task StopAsync(CancellationToken _) => Task.CompletedTask;

  public async ValueTask DisposeAsync() => await _testContainers.Dispose();
}
