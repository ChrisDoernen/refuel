using System.Reflection;
using System.Text.Json;
using Core.Infrastructure.Authorization;
using Core.Infrastructure.Caching;
using Core.Infrastructure.Cqrs;
using Core.Infrastructure.ReadModels;
using Core.Infrastructure.Roles;
using Core.Infrastructure.Serialization;
using EventSourcing;
using MediatR;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.DependencyInjection;
using MongoDB;
using MongoDB.Driver;

namespace Core.Infrastructure;

public static class ServiceCollectionExtensions
{
  public static void AddCoreInfrastructure(
    this IServiceCollection services,
    Assembly assembly
  )
  {
    services.AddMediatR(c =>
      {
        c.RegisterServicesFromAssembly(assembly);
        c.MediatorImplementationType = typeof(EventSourcingMediator);
      }
    );

    services.AddAuthorizersFromAssembly(assembly);

    services.AddMediatorAuthorization(assembly);
    services.AddTransient(typeof(IPipelineBehavior<,>), typeof(AuthorizationBehavior<,>));

    services.AddTransient(typeof(IAuditTrailReplayService<>), typeof(AuditTrailReplayService<>));

    services.AddSingleton<IEventStoreProvider, EventStoreProvider>();
    services.AddSingleton<EventStoreSubscriptionService>();

    services.AddSingleton<IRoleProvider>(new RoleProvider(assembly));

    var jsonSerializerOptions = new JsonSerializerOptions();
    jsonSerializerOptions.Converters.Add(new SubjectConverter());
    services.AddKeyedSingleton<JsonSerializerOptions>(typeof(IHybridCacheSerializer<>), jsonSerializerOptions);

    services.AddHybridCache(options =>
      {
        options.MaximumPayloadBytes = 1024 * 1024;
        options.MaximumKeyLength = 1024;
        options.DefaultEntryOptions = new HybridCacheEntryOptions
        {
          Expiration = TimeSpan.FromDays(365),
          LocalCacheExpiration = TimeSpan.FromDays(365)
        };
      }
    );
  }

  public static void AddDocumentStore(
    this IServiceCollection services,
    Type type,
    string collectionName
  )
  {
    var method = typeof(DocumentStoreFactory)
      .GetMethod(nameof(DocumentStoreFactory.Get))!
      .MakeGenericMethod(type);

    var factory =
      (IServiceProvider sp) => method.Invoke(null, [sp, collectionName]);

    services.AddScoped(
      typeof(IDocumentStore<>).MakeGenericType(type),
      factory!
    );
  }

  private static class DocumentStoreFactory
  {
    public static DocumentStore<T> Get<T>(
      IServiceProvider serviceProvider,
      string collectionName
    ) where T : IDocument
    {
      var database = serviceProvider.GetRequiredService<IMongoDatabase>();

      return new DocumentStore<T>(database, collectionName);
    }
  }

  public static void AddIdentifiedReadModel(
    this IServiceCollection services,
    Type type,
    CacheKey cacheKey,
    Func<Subject, Guid> idSelector
  )
  {
    var eventTypes = type
      .GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
      .Where(m => m.Name == "Apply")
      .SelectMany(m => m.GetParameters())
      .Select(p => p.ParameterType)
      .Distinct()
      .ToList();

    var syncServiceFactoryMethod = typeof(ReadModelSynchronizationServiceFactory)
      .GetMethod(nameof(ReadModelSynchronizationServiceFactory.Get))!
      .MakeGenericMethod(type);

    var syncServiceFactory =
      (IServiceProvider sp) => syncServiceFactoryMethod.Invoke(null, [sp, type, eventTypes, idSelector]);

    services.AddScoped(
      typeof(IReadModelSynchronizationService),
      syncServiceFactory!
    );

    var repoFactoryMethod = typeof(ReadModelRepositoryFactory)
      .GetMethod(nameof(ReadModelRepositoryFactory.Get))!
      .MakeGenericMethod(type);

    var repoFactory =
      (IServiceProvider sp) => repoFactoryMethod.Invoke(null, [sp, cacheKey]);

    var genericRepo = typeof(IIdentifiedReadModelRepository<>).MakeGenericType(type);

    services.AddScoped(
      genericRepo,
      repoFactory!
    );
  }

  private static class ReadModelSynchronizationServiceFactory
  {
    public static IdentifiedReadModelSynchronizationService<T> Get<T>(
      IServiceProvider serviceProvider,
      Type type,
      IEnumerable<Type> eventTypes,
      Func<Subject, Guid> idSelector
    ) where T : IReplayable<T>, IIdentifiedReadModel, new()
    {
      var genericType = typeof(IIdentifiedReadModelRepository<>).MakeGenericType(type);
      var repo = (IIdentifiedReadModelRepository<T>)serviceProvider.GetRequiredService(genericType);

      return new IdentifiedReadModelSynchronizationService<T>(repo, eventTypes, idSelector);
    }
  }

  private static class ReadModelRepositoryFactory
  {
    public static IdentifiedReadModelRepository<T> Get<T>(
      IServiceProvider serviceProvider,
      CacheKey cacheKey
    ) where T : IReplayable<T>, IIdentifiedReadModel, new()
    {
      var cache = serviceProvider.GetRequiredService<HybridCache>();

      return new IdentifiedReadModelRepository<T>(cache, cacheKey);
    }
  }
}
