using Core.Shared;
using Core.Shared.Authorization;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Core;

public static class ServiceCollectionExtensions
{
  public static void AddCore(
    this IServiceCollection services
  )
  {
    var assembly = typeof(ServiceCollectionExtensions).Assembly;
    services.AddMediatR(c => c.RegisterServicesFromAssembly(assembly));

    services.AddTransient(typeof(IPipelineBehavior<,>), typeof(AuthorizationBehavior<,>));

    services.AddAuthorizersFromAssembly(assembly);

    services.AddSingleton<IRoleProvider, RoleProvider>();
  }
}
