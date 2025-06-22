using dotenv.net;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using EventSourcingDB;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;
using Xunit.Microsoft.DependencyInjection;
using Xunit.Microsoft.DependencyInjection.Abstracts;

namespace Core.Tests;

public class CoreFixture : TestBedFixture, IAsyncLifetime
{
  private readonly string _apiToken;
  private readonly int _port;
  private readonly IContainer _container;

  public CoreFixture(IMessageSink _)
  {
    DotEnv.Load(new DotEnvOptions(probeForEnv: true, probeLevelsToSearch: 6));

    _apiToken = Environment.GetEnvironmentVariable("EVENTSOURCINGDB_API_TOKEN") ?? "secret";
    var portEnv = Environment.GetEnvironmentVariable("EVENTSOURCINGDB_DEFAULT_PORT") ?? "3000";
    _port = int.Parse(portEnv);
    var randomizeHostPortEnv = Environment.GetEnvironmentVariable("RANDOMIZE_HOST_PORT");
    var randomizeHostPort = randomizeHostPortEnv is not string || bool.Parse(randomizeHostPortEnv);

    _container = new ContainerBuilder()
      .WithImage("thenativeweb/eventsourcingdb:1.0.2")
      .WithPortBinding(_port, randomizeHostPort)
      .WithCommand(
        "run",
        $"--api-token={_apiToken}",
        $"--http-port={_port}",
        "--data-directory-temporary",
        "--http-enabled",
        "--https-enabled=false",
        "--with-ui"
      )
      .Build();
  }

  public async Task InitializeAsync()
  {
    await _container.StartAsync().ConfigureAwait(false);
  }

  public T Get<T>(ITestOutputHelper testOutputHelper)
    => GetService<T>(testOutputHelper) ?? throw new Exception($"Service missing: {typeof(T).Name}");

  protected override void AddServices(
    IServiceCollection services,
    IConfiguration? configuration
  )
  {
    services.AddCore();
    services.AddEventSourcingDb(
      configuration!,
      options =>
      {
        options.Url = new UriBuilder(options.Url) { Port = _container.GetMappedPublicPort(_port) }.ToString();
        options.ApiToken = _apiToken;
      }
    );
  }

  protected override IEnumerable<TestAppSettings> GetTestAppSettings()
  {
    yield return new TestAppSettings { Filename = "appsettings.json", IsOptional = false };
  }

  protected override async ValueTask DisposeAsyncCore()
  {
    await _container.DisposeAsync();
  }

  public new async Task DisposeAsync() => await base.DisposeAsync();
}
