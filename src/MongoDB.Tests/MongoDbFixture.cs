using dotenv.net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Testing;
using Xunit;
using Xunit.Abstractions;
using Xunit.Microsoft.DependencyInjection;
using Xunit.Microsoft.DependencyInjection.Abstracts;

namespace MongoDB.Tests;

public class MongoDbFixture : TestBedFixture, IAsyncLifetime
{
  private readonly MongoDbContainer _testContainer;

  public MongoDbFixture(IMessageSink _)
  {
    DotEnv.Load(new DotEnvOptions(probeForEnv: true, probeLevelsToSearch: 6));

    var mongoDbConnection =
      Configuration!
        .GetSection("MongoDb")
        .Get<MongoDbConnection>()!;

    _testContainer = new MongoDbContainer(mongoDbConnection);
  }

  public async Task InitializeAsync() => await _testContainer.Start();

  public T Get<T>(ITestOutputHelper testOutputHelper)
    => GetService<T>(testOutputHelper) ?? throw new Exception($"Service missing: {typeof(T).Name}");

  protected override void AddServices(
    IServiceCollection services,
    IConfiguration? configuration
  )
  {
    services.AddMongoDb(configuration!, _testContainer.ConfigureConnection);
  }

  protected override IEnumerable<TestAppSettings> GetTestAppSettings()
  {
    yield return new TestAppSettings { Filename = "appsettings.json", IsOptional = false };
  }

  protected override async ValueTask DisposeAsyncCore() => await _testContainer.DisposeAsync();

  public new async Task DisposeAsync() => await base.DisposeAsync();
}
