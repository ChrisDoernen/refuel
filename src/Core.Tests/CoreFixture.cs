using Core.Infrastructure;
using dotenv.net;
using EventSourcing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB;
using Shared.Testing;
using Shared.Testing.EventSourcingDB;
using Shared.Testing.MongoDB;
using Xunit;
using Xunit.Microsoft.DependencyInjection;
using Xunit.Microsoft.DependencyInjection.Abstracts;
using Xunit.Sdk;

namespace Core.Tests;

public class CoreFixture : TestBedFixture, IAsyncLifetime
{
  private readonly IEnumerable<TestContainer> _testContainers;

  public CoreFixture(IMessageSink _)
  {
    DotEnv.Load(new DotEnvOptions(probeForEnv: true, probeLevelsToSearch: 6));

    _testContainers =
    [
      ..Configuration!.GetEventSourcingDbContainers(),
      Configuration!.GetMongoDbContainer()
    ];
  }

  public async ValueTask InitializeAsync() => await _testContainers.Start();

  public T Get<T>(ITestOutputHelper testOutputHelper)
    => GetService<T>(testOutputHelper) ?? throw new Exception($"Service missing: {typeof(T).Name}");

  protected override void AddServices(
    IServiceCollection services,
    IConfiguration? configuration
  )
  {
    services.AddCore();
    services.AddCoreInfrastructure(typeof(Core.ServiceCollectionExtensions).Assembly);
    services.AddEventSourcingDb(
      configuration!,
      connections => connections.ConfigureFromContainers(_testContainers)
    );
    services.AddMongoDb(
      configuration!,
      connections => connections.ConfigureFromContainer(_testContainers)
    );
  }

  protected override IEnumerable<TestAppSettings> GetTestAppSettings()
  {
    yield return new TestAppSettings { Filename = "appsettings.json", IsOptional = false };
  }

  protected override async ValueTask DisposeAsyncCore() => await _testContainers.Dispose();
}
