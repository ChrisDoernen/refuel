using EventSourcingDB;

namespace Core.Tanks.FuelExtraction;

[EventType("com.example.fuel-extracted.v1")]
public record FuelExtractedEventV1(
  int AmountExtracted
) : IEventData;
