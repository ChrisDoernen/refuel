using dotenv.net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Testing;
using Xunit;
using Xunit.Abstractions;
using Xunit.Microsoft.DependencyInjection;
using Xunit.Microsoft.DependencyInjection.Abstracts;

namespace EventSourcingDB.Tests;

public class EventSourcingDbFixture : TestBedFixture, IAsyncLifetime
{
  private readonly IEnumerable<EventSourcingDbContainer> _testContainers;

  public EventSourcingDbFixture(IMessageSink _)
  {
    DotEnv.Load(new DotEnvOptions(probeForEnv: true, probeLevelsToSearch: 6));

    _testContainers =
      Configuration!
        .GetSection("EventSourcingDb:Connections")
        .Get<IList<EventSourcingDbConnection>>()!
        .Select(c => new EventSourcingDbContainer(c));
  }

  public async Task InitializeAsync()
  {
    foreach (var container in _testContainers)
    {
      await container.Start();
    }
  }

  public T Get<T>(ITestOutputHelper testOutputHelper)
    => GetService<T>(testOutputHelper) ?? throw new Exception($"Service missing: {typeof(T).Name}");

  protected override void AddServices(
    IServiceCollection services,
    IConfiguration? configuration
  )
  {
    services.AddEventSourcingDb(
      configuration!,
      connections =>
      {
        foreach (var container in _testContainers)
        {
          var tenantConnection = connections.Single(c => c.TenantId == container.TenantId);
          container.ConfigureConnection(tenantConnection);
        }
      }
    );
  }

  protected override IEnumerable<TestAppSettings> GetTestAppSettings()
  {
    yield return new TestAppSettings { Filename = "appsettings.json", IsOptional = false };
  }

  protected override async ValueTask DisposeAsyncCore()
  {
    foreach (var container in _testContainers)
    {
      await container.DisposeAsync();
    }
  }

  public new async Task DisposeAsync() => await base.DisposeAsync();
}
