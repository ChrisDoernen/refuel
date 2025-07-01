using EventSourcingDB;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Shared.Testing;

namespace Dev;

public class TestContainerService : IHostedService, IAsyncDisposable
{
  private readonly EventSourcingDbContainer _eventSourcingDbTestcontainer;
  private readonly ILogger<TestContainerService> _logger;

  public TestContainerService(
    IConfiguration configuration,
    ILogger<TestContainerService> logger
  )
  {
    _logger = logger;

    var options = configuration
      .GetSection(EventSourcingDbOptions.SectionName)
      .Get<EventSourcingDbOptions>()!;

    _eventSourcingDbTestcontainer = new EventSourcingDbContainer(options);
  }

  public async Task StartAsync(CancellationToken cancellationToken)
  {
    _logger.LogInformation("Starting test containers");

    await _eventSourcingDbTestcontainer.Start(cancellationToken);
  }

  public Task StopAsync(CancellationToken _) => Task.CompletedTask;

  public async ValueTask DisposeAsync()
  {
    await _eventSourcingDbTestcontainer.DisposeAsync();
  }
}
