using dotenv.net;
using EventSourcing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Testing;
using Shared.Testing.EventSourcingDB;
using Xunit;
using Xunit.Microsoft.DependencyInjection;
using Xunit.Microsoft.DependencyInjection.Abstracts;
using Xunit.Sdk;

namespace EventSourcingDB.Tests;

public class EventSourcingDbFixture : TestBedFixture, IAsyncLifetime
{
  private readonly IEnumerable<EventSourcingDbContainer> _testContainers;

  public EventSourcingDbFixture(IMessageSink _)
  {
    DotEnv.Load(new DotEnvOptions(probeForEnv: true, probeLevelsToSearch: 6));


    _testContainers = Configuration!.GetEventSourcingDbContainers();
  }

  public async ValueTask InitializeAsync() => await _testContainers.Start();

  public T Get<T>(ITestOutputHelper testOutputHelper)
    => GetService<T>(testOutputHelper) ?? throw new Exception($"Service missing: {typeof(T).Name}");

  protected override void AddServices(
    IServiceCollection services,
    IConfiguration? configuration
  )
  {
    services.AddEventSourcingDb(
      configuration!,
      connections => connections.ConfigureFromContainers(_testContainers)
    );
  }

  protected override IEnumerable<TestAppSettings> GetTestAppSettings()
  {
    yield return new TestAppSettings { Filename = "appsettings.json", IsOptional = false };
  }

  protected override async ValueTask DisposeAsyncCore() => await _testContainers.Dispose();

  public new async Task DisposeAsync() => await base.DisposeAsync();
}
