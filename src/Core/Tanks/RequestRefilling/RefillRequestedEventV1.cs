using EventSourcingDB;

namespace Core.Tanks.RequestRefilling;

[EventType("com.example.refill-requested.v1")]
public record RefillRequestedEventV1 : IEventData;
