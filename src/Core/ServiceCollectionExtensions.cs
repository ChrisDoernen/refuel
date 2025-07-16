using Core.Clubs;
using Core.Shared;
using Core.Users;
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
    services.AddSingleton<IRoleProvider, RoleProvider>();
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
