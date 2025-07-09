using DotNet.Testcontainers.Builders;
using EventSourcing;

namespace Shared.Testing.EventSourcingDB;

public class EventSourcingDbContainer : TestContainer
{
  public string TenantId { get; }
  private readonly string _apiToken;

  public EventSourcingDbContainer(
    EventSourcingDbConnection connection
  )
  {
    TenantId = connection.TenantId;
    _apiToken = connection.ApiToken;
    _port = new Uri(connection.Url).Port;

    _container = new ContainerBuilder()
      .WithImage("thenativeweb/eventsourcingdb:1.0.2")
      .WithPortBinding(_port, _randomizeHostPort)
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

  public Action<EventSourcingDbConnection> ConfigureConnection =>
    connection =>
    {
      connection.Url = new UriBuilder(connection.Url) { Port = _container.GetMappedPublicPort(_port) }.ToString();
      connection.ApiToken = _apiToken;
    };
}
