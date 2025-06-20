using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;
using Xunit.Microsoft.DependencyInjection;
using Xunit.Microsoft.DependencyInjection.Abstracts;

namespace EventSourcingDB.Tests;

public class EventSourcingDbFixture(
  IMessageSink messageSink
) : TestBedFixture, IAsyncLifetime
{
  private const int Port = 3000;
  private const string ApiToken = "secret";

  private readonly IContainer _container = new ContainerBuilder()
    .WithImage("thenativeweb/eventsourcingdb:1.0.2")
    .WithPortBinding(Port, false)
    .WithCommand(
      "run",
      $"--api-token={ApiToken}",
      "--data-directory-temporary",
      "--http-enabled",
      "--https-enabled=false",
      "--with-ui"
    )
    .Build();

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
    services.AddEventSourcingDb(
      configuration!,
      options =>
      {
        options.Url = new UriBuilder(options.Url) { Port = _container.GetMappedPublicPort(Port) }.ToString();
        options.ApiToken = ApiToken;
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
