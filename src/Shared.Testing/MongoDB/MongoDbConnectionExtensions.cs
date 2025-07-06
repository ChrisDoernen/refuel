using MongoDB;

namespace Shared.Testing.MongoDB;

public static class MongoDbConnectionExtensions
{
  public static void ConfigureFromContainer(
    this MongoDbConnection connection,
    IEnumerable<TestContainer> containers
  )
  {
    foreach (var container in containers.OfType<MongoDbContainer>())
    {
      container.ConfigureConnection(connection);
    }
  }
}
