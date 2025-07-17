using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

namespace MongoDB;

public static class ServiceCollectionExtensions
{
  public static void AddMongoDb(
    this IServiceCollection services,
    IConfiguration configuration,
    Action<MongoDbConnection>? configureConnection = null
  )
  {
    var objectSerializer = new ObjectSerializer(type =>
      ObjectSerializer.DefaultAllowedTypes(type) || type.FullName.StartsWith("Core")
    );
    BsonSerializer.RegisterSerializer(objectSerializer);
    BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));
    BsonSerializer.RegisterSerializer(new DateTimeSerializer(DateTimeKind.Utc, BsonType.Document));

    var mongoDbConnection =
      configuration
        .GetSection("MongoDb")
        .Get<MongoDbConnection>()
      ?? throw new InvalidOperationException("MongoDb options are not configured.");

    configureConnection?.Invoke(mongoDbConnection);

    var mongoClient = new MongoClient(mongoDbConnection.Url);
    var database = mongoClient.GetDatabase(mongoDbConnection.Database);

    services.AddSingleton(database);
  }
}
