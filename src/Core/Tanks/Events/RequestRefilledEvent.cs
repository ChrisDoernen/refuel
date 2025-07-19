using EventSourcing;

namespace Core.Tanks.Events;

[EventType("com.example.refilled.v1")]
public record RefilledEventV1(
  int NewFuelLevel
) : IEventData;
