using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Shared.Authorization;

public static class ServiceCollectionExtensions
{
  public static void AddAuthorizersFromAssembly(
    this IServiceCollection services,
    Assembly assembly,
    ServiceLifetime lifetime = ServiceLifetime.Scoped
  )
  {
    var authorizerType = typeof(IAuthorizer<>);
    GetTypesAssignableTo(assembly, authorizerType)
      .ForEach(
        type =>
        {
          foreach (var implementedInterface in type.ImplementedInterfaces)
          {
            if (!implementedInterface.IsGenericType)
            {
              continue;
            }

            if (implementedInterface.GetGenericTypeDefinition() != authorizerType)
            {
              continue;
            }

            var serviceType = implementedInterface.ContainsGenericParameters
              ? authorizerType
              : implementedInterface;

            services.Add(new ServiceDescriptor(serviceType, type, lifetime));
          }
        }
      );
  }

  private static List<TypeInfo> GetTypesAssignableTo(Assembly assembly, Type compareType)
  {
    return assembly.DefinedTypes
      .Where(
        t =>
          t is { IsClass: true, IsAbstract: false }
          && t != compareType
          && t.GetInterfaces()
            .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == compareType)
      )
      .ToList();
  }

  public static void AddMediatorAuthorization(this IServiceCollection services, Assembly assembly)
  {
    AddAuthorizationHandlers(services, assembly);
  }

  private static void AddAuthorizationHandlers(IServiceCollection services, Assembly assembly)
  {
    var authHandlerOpenType = typeof(IAuthorizationHandler<>);
    GetTypesAssignableTo(assembly, authHandlerOpenType)
      .ForEach(
        concretion =>
        {
          foreach (var implementedInterface in concretion.ImplementedInterfaces)
          {
            if (!implementedInterface.IsGenericType)
            {
              continue;
            }

            if (implementedInterface.GetGenericTypeDefinition() != authHandlerOpenType)
            {
              continue;
            }

            services.AddTransient(implementedInterface, concretion);
          }
        }
      );
  }
}
