using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace EventSourcingDB;

public interface IEventStoreFactory
{
  IEventStore ForTenant(string tenantId);
}

public class EventStoreFactory(
  IServiceProvider serviceProvider
) : IEventStoreFactory
{
  public IEventStore ForTenant(string tenantId)
  {
    var options = serviceProvider.GetRequiredService<IOptions<EventSourcingDbOptions>>();
    var httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
    var httpClient = httpClientFactory.CreateClient($"eventsourcingdb-{tenantId}");

    if (httpClient.BaseAddress is null)
    {
      throw new InvalidOperationException($"Tenant '{tenantId}' has no EventSourcingDB configured.");
    }

    return new EventStore(
      options,
      httpClient
    );
  }
}
