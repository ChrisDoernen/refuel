namespace Core.Tanks.Refilling;

public record RefilledEventV1(
  Guid UserId,
  int NewFuelLevel
) : ITankRelated;
