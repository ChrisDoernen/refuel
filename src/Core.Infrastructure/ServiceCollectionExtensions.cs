using System.Reflection;
using Core.Infrastructure.Authorization;
using Core.Infrastructure.Cqrs;
using Core.Infrastructure.ReadModels;
using Core.Infrastructure.Roles;
using EventSourcing;
using MediatR;
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

    services.AddSingleton<IEventStoreProvider, EventStoreProvider>();
    services.AddSingleton<EventStoreSubscriptionService>();

    services.AddSingleton<IRoleProvider>(new RoleProvider(assembly));
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

  public static void AddReadModel(
    this IServiceCollection services,
    Type type,
    string collectionName,
    Func<Subject, Guid> idSelector
  )
  {
    services.AddScoped(
      typeof(IReadModelRepository<>).MakeGenericType(type),
      typeof(ReadModelRepository<>).MakeGenericType(type)
    );

    var persistableType = typeof(PersistableStateChange<>).MakeGenericType(type);
    services.AddDocumentStore(persistableType, collectionName);

    var eventTypes = type
      .GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
      .Where(m => m.Name == "Apply")
      .SelectMany(m => m.GetParameters())
      .Select(p => p.ParameterType)
      .Distinct()
      .ToList();

    var method = typeof(ReadModelSynchronizationServiceFactory)
      .GetMethod(nameof(ReadModelSynchronizationServiceFactory.Get))!
      .MakeGenericMethod(type);

    var factory =
      (IServiceProvider sp) => method.Invoke(null, [sp, type, eventTypes, idSelector]);


    services.AddScoped(
      typeof(IReadModelSynchronizationService),
      factory!
    );
  }

  private static class ReadModelSynchronizationServiceFactory
  {
    public static ReadModelSynchronizationService<T> Get<T>(
      IServiceProvider serviceProvider,
      Type type,
      IEnumerable<Type> eventTypes,
      Func<Subject, Guid> idSelector
    ) where T : IReplayable<T>, new()
    {
      var genericType = typeof(IReadModelRepository<>).MakeGenericType(type);
      var repo = (IReadModelRepository<T>)serviceProvider.GetRequiredService(genericType);

      return new ReadModelSynchronizationService<T>(repo, eventTypes, idSelector);
    }
  }
}
