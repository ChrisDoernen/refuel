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
  }
}
