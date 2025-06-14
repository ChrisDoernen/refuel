using EventSourcingDbClient;

namespace Core.Tanks.Refilling;

[EventType("com.example.refill-requested.v1")]
public record RefillRequestedEventV1 : IEventData;
