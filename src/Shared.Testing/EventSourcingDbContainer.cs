using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using EventSourcingDB;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Shared.Testing;

public class EventSourcingDbContainer : IAsyncDisposable
{
  private readonly string _apiToken;
  private readonly int _port;
  private readonly IContainer _container;

  public EventSourcingDbContainer(EventSourcingDbOptions options)
  {
    _apiToken = options.ApiToken;
    _port = new Uri(options.Url).Port;

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

  public Action<EventSourcingDbOptions> ConfigureOptions =>
    options =>
    {
      options.Url = new UriBuilder(options.Url) { Port = _container.GetMappedPublicPort(_port) }.ToString();
      options.ApiToken = _apiToken;
    };

  public async Task<EventSourcingDbContainer> Start(CancellationToken cancellationToken = default)
  {
    await _container.StartAsync(cancellationToken).ConfigureAwait(false);

    return this;
  }

  public async ValueTask DisposeAsync() => await _container.DisposeAsync();
}
