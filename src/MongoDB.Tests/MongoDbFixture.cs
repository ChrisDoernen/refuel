using dotenv.net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Shared.Testing;
using Shared.Testing.MongoDB;
using Xunit;
using Xunit.Microsoft.DependencyInjection;
using Xunit.Microsoft.DependencyInjection.Abstracts;
using Xunit.Sdk;

namespace MongoDB.Tests;

public class MongoDbFixture : TestBedFixture, IAsyncLifetime
{
  private readonly MongoDbContainer _testContainer;

  public MongoDbFixture(IMessageSink _)
  {
    DotEnv.Load(new DotEnvOptions(probeForEnv: true, probeLevelsToSearch: 6));

    _testContainer = Configuration!.GetMongoDbContainer();
  }

  public async ValueTask InitializeAsync() => await _testContainer.Start();

  public T Get<T>(ITestOutputHelper testOutputHelper)
    => GetService<T>(testOutputHelper) ?? throw new Exception($"Service missing: {typeof(T).Name}");

  protected override void AddServices(
    IServiceCollection services,
    IConfiguration? configuration
  )
  {
    services.AddMongoDb(configuration!, _testContainer.ConfigureConnection);
    services.AddTransient<IDocumentStore<TestDocument>>(
      sp =>
      {
        var database = sp.GetRequiredService<IMongoDatabase>();

        return new DocumentStore<TestDocument>(database, "testDocuments");
      }
    );
  }

  protected override IEnumerable<TestAppSettings> GetTestAppSettings()
  {
    yield return new TestAppSettings { Filename = "appsettings.json", IsOptional = false };
  }

  protected override async ValueTask DisposeAsyncCore() => await _testContainer.DisposeAsync();

  public new async Task DisposeAsync() => await base.DisposeAsync();
}
