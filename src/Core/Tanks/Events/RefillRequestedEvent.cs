using EventSourcing;

namespace Core.Tanks.Events;

[EventType("com.example.refill-requested.v1")]
public record RefillRequestedEventV1 : IEventData;
