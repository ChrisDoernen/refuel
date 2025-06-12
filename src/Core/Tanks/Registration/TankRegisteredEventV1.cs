using EventSourcingDbClient;

namespace Core.Tanks.Registration;

[EventType("com.example.tank-registered.v1")]
public record TankRegisteredEventV1(
  Guid Id,
  string Name,
  string Club,
  string Description,
  int InitialFuelLevel
): ITankRelated;
