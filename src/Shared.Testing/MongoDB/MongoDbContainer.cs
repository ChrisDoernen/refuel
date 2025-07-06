using DotNet.Testcontainers.Builders;
using MongoDB;

namespace Shared.Testing;

public sealed class MongoDbContainer : TestContainer
{
  public MongoDbContainer(
    MongoDbConnection connection
  )
  {
    _port = new Uri(connection.Url).Port;

    _container = new ContainerBuilder()
      .WithImage("mongo:8")
      .WithPortBinding(_port, _randomizeHostPort)
      .Build();
  }

  public Action<MongoDbConnection> ConfigureConnection =>
    connection =>
    {
      connection.Url = new UriBuilder(connection.Url) { Port = _container.GetMappedPublicPort(_port) }.ToString();
    };
}
