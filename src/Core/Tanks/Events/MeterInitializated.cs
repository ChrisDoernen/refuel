using EventSourcing;

namespace Core.Tanks.Events;

[EventType("com.example.meter-initialized.v1")]
public record MeterInitializedEventV1 : IEventData;
