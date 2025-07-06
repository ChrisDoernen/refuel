using EventSourcingDB;

namespace Shared.Testing.EventSourcingDB;

public static class EventSourcingDbConnectionsExtensions
{
  public static void ConfigureFromContainers(
    this EventSourcingDbConnections connections,
    IEnumerable<TestContainer> containers
  )
  {
    foreach (var container in containers.OfType<EventSourcingDbContainer>())
    {
      var tenantConnection = connections.ForTenant(container.TenantId);
      container.ConfigureConnection(tenantConnection);
    }
  }
}
