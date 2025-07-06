using Microsoft.Extensions.Configuration;
using MongoDB;

namespace Shared.Testing.MongoDB;

public static class ConfigurationExtensions
{
  public static MongoDbContainer GetMongoDbContainer(
    this IConfiguration configuration
  )
  {
    var mongoDbConnection =
      configuration
        .GetSection("MongoDb")
        .Get<MongoDbConnection>()!;

    return new MongoDbContainer(mongoDbConnection);
  }
}
