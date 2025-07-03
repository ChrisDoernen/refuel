using EventSourcingDB;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Shared.Testing;

namespace Dev;

public class TestContainerService : IHostedService, IAsyncDisposable
{
  private readonly IEnumerable<EventSourcingDbContainer> _eventSourcingDbTestcontainer;
  private readonly ILogger<TestContainerService> _logger;

  public TestContainerService(
    IConfiguration configuration,
    ILogger<TestContainerService> logger
  )
  {
    _logger = logger;

    _eventSourcingDbTestcontainer =
      configuration!
        .GetSection("EventSourcingDb:Connections")
        .Get<IList<EventSourcingDbConnection>>()!
        .Select(c => new EventSourcingDbContainer(c));
  }

  public async Task StartAsync(CancellationToken cancellationToken)
  {
    _logger.LogInformation("Starting test containers");

    foreach (var container in _eventSourcingDbTestcontainer)
    {
      await container.Start(cancellationToken);
    }
  }

  public Task StopAsync(CancellationToken _) => Task.CompletedTask;

  public async ValueTask DisposeAsync()
  {
    foreach (var container in _eventSourcingDbTestcontainer)
    {
      await container.DisposeAsync();
    }
  }
}
