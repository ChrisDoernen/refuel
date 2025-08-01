using System.Reflection;
using System.Text.Json;
using Core.Infrastructure.Authorization;
using Core.Infrastructure.Caching;
using Core.Infrastructure.Cqrs;
using Core.Infrastructure.Projections;
using Core.Infrastructure.Roles;
using Core.Infrastructure.Serialization;
using EventSourcing;
using MediatR;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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

    services.AddTransient(typeof(IReplayService<>), typeof(ReplayService<>));

    services.AddSingleton<IEventStoreProvider, EventStoreProvider>();
    
    services.AddSingleton<EventStoreObserver>();
    services.AddHostedService(sp => sp.GetRequiredService<EventStoreObserver>());

    services.AddSingleton<IRoleProvider>(new RoleProvider(assembly));

    var jsonSerializerOptions = new JsonSerializerOptions();
    jsonSerializerOptions.Converters.Add(new SubjectConverter());
    services.AddKeyedSingleton<JsonSerializerOptions>(typeof(IHybridCacheSerializer<>), jsonSerializerOptions);

    services.AddHybridCache(options =>
      {
        options.MaximumPayloadBytes = 10 + 1024 * 1024;
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

  public static void AddIdentifiedProjection(
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

    var syncServiceFactoryMethod = typeof(ProjectorFactory)
      .GetMethod(nameof(ProjectorFactory.Get))!
      .MakeGenericMethod(type);

    var syncServiceFactory =
      (IServiceProvider sp) => syncServiceFactoryMethod.Invoke(null, [sp, type, eventTypes, idSelector]);

    services.AddScoped(
      typeof(IProjector),
      syncServiceFactory!
    );

    var repoFactoryMethod = typeof(ProjectionRepositoryFactory)
      .GetMethod(nameof(ProjectionRepositoryFactory.Get))!
      .MakeGenericMethod(type);

    var repoFactory =
      (IServiceProvider sp) => repoFactoryMethod.Invoke(null, [sp, cacheKey]);

    var genericRepo = typeof(IIdentifiedProjectionRepository<>).MakeGenericType(type);

    services.AddScoped(
      genericRepo,
      repoFactory!
    );
  }

  private static class ProjectorFactory
  {
    public static IdentifiedProjector<T> Get<T>(
      IServiceProvider serviceProvider,
      Type type,
      IEnumerable<Type> eventTypes,
      Func<Subject, Guid> idSelector
    ) where T : IReplayable<T>, IIdentifiedProjection, new()
    {
      var genericType = typeof(IIdentifiedProjectionRepository<>).MakeGenericType(type);
      var repo = (IIdentifiedProjectionRepository<T>)serviceProvider.GetRequiredService(genericType);

      return new IdentifiedProjector<T>(repo, eventTypes, idSelector);
    }
  }

  private static class ProjectionRepositoryFactory
  {
    public static HybridCacheIdentifiedProjectionRepository<T> Get<T>(
      IServiceProvider serviceProvider,
      CacheKey cacheKey
    ) where T : IReplayable<T>, IIdentifiedProjection, new()
    {
      var cache = serviceProvider.GetRequiredService<HybridCache>();

      return new HybridCacheIdentifiedProjectionRepository<T>(cache, cacheKey);
    }
  }
}
