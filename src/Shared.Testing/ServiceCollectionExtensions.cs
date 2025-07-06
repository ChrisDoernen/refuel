using Microsoft.Extensions.DependencyInjection;

namespace Shared.Testing;

public static class ServiceCollectionExtensions
{
  public static void AddTesting(
    this IServiceCollection services
  )
  {
    services.AddHostedService<TestContainerService>();
    services.AddHostedService<DevDataRestoreService>();
  }
}
