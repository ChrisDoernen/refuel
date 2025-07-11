using App.Authorization;
using App.Cqrs;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace App;

public static class ServiceCollectionExtensions
{
  public static void AddApp(
    this IServiceCollection services
  )
  {
    var assembly = typeof(ServiceCollectionExtensions).Assembly;

    services.AddMediatorAuthorization(assembly);
    services.AddTransient(typeof(IPipelineBehavior<,>), typeof(AuthorizationBehavior<,>));
    services.AddAuthorizersFromAssembly(assembly);

    services.AddSingleton<IEventStoreProvider, EventStoreProvider>();
    services.AddHostedService<EventStoreSubscriptionService>();
  }
}
