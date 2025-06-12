namespace Core.Tanks.FuelExtraction;

public record FuelExtractedEventV1(
  Guid UserId,
  int AmountExtracted
) : ITankRelated;

