using EventSourcingDB;

namespace Core.Tanks.MeterReading;

[EventType("com.example.meter-read.v1")]
public record MeterReadEventV1(
  int Value
) : IEventData;
