using Core.Clubs;
using Core.Shared;
using Core.Shared.Authorization;
using Core.Users;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using MongoDB;
using MongoDB.Driver;

namespace Core;

public static class ServiceCollectionExtensions
{
  public static void AddCore(
    this IServiceCollection services
  )
  {
    var assembly = typeof(ServiceCollectionExtensions).Assembly;
    services.AddMediatR(c => c.RegisterServicesFromAssembly(assembly));
    services.AddMediatorAuthorization(assembly);

    services.AddTransient(typeof(IPipelineBehavior<,>), typeof(AuthorizationBehavior<,>));

    services.AddAuthorizersFromAssembly(assembly);

    services.AddSingleton<IRoleProvider, RoleProvider>();
    services.AddSingleton<IEventStoreProvider, EventStoreProvider>();
    services.AddHostedService<EventStoreProviderInitService>();

    services.AddTransient<IDocumentStore<User>>(
      sp => new DocumentStore<User>(
        sp.GetRequiredService<IMongoDatabase>(),
        "users"
      )
    );
    services.AddTransient<IDocumentStore<Club>>(
      sp => new DocumentStore<Club>(
        sp.GetRequiredService<IMongoDatabase>(),
        "clubs"
      )
    );
  }
}
