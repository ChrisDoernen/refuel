using System.Reflection;
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

  public static void AddDocumentStore(
    this IServiceCollection services,
    Type type,
    string collectionName
  )
  {
    var repositoryInterface = typeof(IDocumentStore<>);
    var repositoryImplementation = typeof(DocumentStoreFactory).GetMethod(nameof(DocumentStoreFactory.GetStore))!;

    services.AddScoped(
      repositoryInterface.MakeGenericType(type),
      CreateImplementationFactory(repositoryImplementation, type, collectionName)!
    );
  }

  private static Func<IServiceProvider, object?> CreateImplementationFactory(
    MethodInfo genericMethod,
    Type genericType,
    string collectionName
  )
  {
    var method = genericMethod.MakeGenericMethod(genericType);

    return provider => method.Invoke(null, [provider, collectionName]);
  }

  private static class DocumentStoreFactory
  {
    public static DocumentStore<T> GetStore<T>(
      IServiceProvider serviceProvider,
      string collectionName
    ) where T : IDocument
    {
      var database = serviceProvider.GetRequiredService<IMongoDatabase>();

      return new DocumentStore<T>(database, collectionName);
    }
  }
}
