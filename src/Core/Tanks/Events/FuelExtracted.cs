using EventSourcing;

namespace Core.Tanks.Events;

[EventType("com.example.fuel-extracted.v1")]
public record FuelExtractedEventV1(
  int AmountExtracted
) : IEventData;
