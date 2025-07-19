using EventSourcing;

namespace Core.Tanks.Events;

[EventType("com.example.meter-read.v1")]
public record MeterReadEventV1(
  int Value
) : IEventData;
