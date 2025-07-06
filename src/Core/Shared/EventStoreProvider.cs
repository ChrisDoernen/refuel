using EventSourcingDB;

namespace Core.Shared;

public interface IEventStoreProvider
{
  IEventStore ForClub(Guid clubId);
  void RegisterTenant(Guid clubId, string tenantId);
}

public class EventStoreProvider(
  IEventStoreFactory factory
) : IEventStoreProvider
{
  private readonly Dictionary<Guid, string> _tenants = new();

  public void RegisterTenant(
    Guid clubId,
    string tenantId
  ) => _tenants.TryAdd(clubId, tenantId);

  public IEventStore ForClub(
    Guid clubId
  ) => _tenants.GetValueOrDefault(clubId) is string tenantId
    ? factory.ForTenant(tenantId)
    : throw new InvalidOperationException($"No tenant found for club {clubId}");
}
