using EventSourcingDB;

namespace Core.Tanks.Refilling;

[EventType("com.example.refilled.v1")]
public record RefilledEventV1(
  int NewFuelLevel
) : IEventData;
