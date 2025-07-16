using System.Reflection;
using Core.Infrastructure.Authorization;
using Core.Infrastructure.Cqrs;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

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
    services.AddHostedService<EventStoreSubscriptionService>();
  }
}
