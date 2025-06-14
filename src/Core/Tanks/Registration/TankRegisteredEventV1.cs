using EventSourcingDbClient;

namespace Core.Tanks.Registration;

[EventType("com.example.tank-registered.v1")]
public record TankRegisteredEventV1(
  Guid Id,
  Guid Club,
  string Name,
  string Description,
  int Capacity,
  int InitialFuelLevel
) :  IEventData;
