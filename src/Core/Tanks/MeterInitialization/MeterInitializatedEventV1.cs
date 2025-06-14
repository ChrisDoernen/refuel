using EventSourcingDbClient;

namespace Core.Tanks.MeterInitialization;

[EventType("com.example.meter-initialized.v1")]
public record MeterInitializedEventV1 : IEventData;
