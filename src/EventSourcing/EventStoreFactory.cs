using EventSourcingDb;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EventSourcing;

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
    var converter =
      serviceProvider.GetService<EventConverter>() ??
      throw new InvalidOperationException("EventConverter is not registered in the service provider.");
    var options = serviceProvider.GetService<IOptions<EventSourcingDbOptions>>()!;
    var logger =
      serviceProvider.GetService<ILogger<IEventStore>>() ??
      throw new InvalidOperationException("ILogger<IEventStore> is not registered in the service provider.");
    var client =
      serviceProvider.GetKeyedService<Client>($"esdbclient-{tenantId}") ??
      throw new InvalidOperationException($"No EventStore client found for tenant '{tenantId}'.");

    return new EventStore(
      logger,
      client,
      converter,
      options
    );
  }
}
