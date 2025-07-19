using EventSourcing;

namespace Core.Tanks.Events;

[EventType("com.example.tank-registered.v1")]
public record TankRegisteredEventV1(
  Guid TankId,
  Guid Club,
  string Name,
  string Description,
  int Capacity,
  int InitialFuelLevel
) : IEventData;
