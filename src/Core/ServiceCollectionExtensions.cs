using Core.Clubs;
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
    services.AddHostedService<EventStoreProviderInitService>();

    Dictionary<Type, string> documents =
      new()
      {
        { typeof(User), "users" },
        { typeof(Club), "clubs" }
      };

    foreach (var document in documents)
    {
      services.AddDocumentStore(document.Key, document.Value);
    }
  }
}
