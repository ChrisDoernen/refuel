using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace EventSourcingDB;

public interface IEventStoreFactory
{
  IEventStore ForTenant(Guid tenantId);
}

public class EventStoreFactory(
  IServiceProvider serviceProvider
) : IEventStoreFactory
{
  public IEventStore ForTenant(Guid tenantId)
  {
    var httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
    var options = serviceProvider.GetRequiredService<IOptions<EventSourcingDbOptions>>();

    return new EventStore(
      httpClientFactory,
      options,
      tenantId
    );
  }
}
