using System.Reflection;
using App.Authorization;
using App.Cqrs;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace App;

public static class ServiceCollectionExtensions
{
  public static void AddApp(
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
    services.AddHostedService<EventStoreSubscriptionService>();
  }
}
