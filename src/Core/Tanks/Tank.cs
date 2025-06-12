using Core.Tanks.FuelExtraction;
using Core.Tanks.Refilling;
using Core.Tanks.Registration;

namespace Core.Tanks;

public record Tank
{
  public Guid Id { get; private init; }
  public int FuelLevel { get; private init; }
  public Meter? Meter { get; private init; }
  public string Name { get; private init; } = null!;
  public string Club { get; private init; } = null!;
  public string Description { get; private init; } = null!;
  
  public Tank Apply(ITankRelated evnt)
  {
    return evnt switch
    {
      FuelExtractedEventV1 fuelExtractedEvent => Apply(fuelExtractedEvent),
      RefilledEventV1 refilledEvent => Apply(refilledEvent),
      _ => throw new InvalidOperationException("Unknown event for tank"),
    };
  }

  private Tank Apply(TankRegisteredEventV1 evnt) => this with
  {
    Id = evnt.Id,
    FuelLevel = evnt.InitialFuelLevel,
    Name = evnt.Name,
    Club = evnt.Club,
    Description = evnt.Description
  };

  private Tank Apply(FuelExtractedEventV1 evnt) => this with { FuelLevel = FuelLevel - evnt.AmountExtracted };
  private Tank Apply(RefilledEventV1 evnt) => this with { FuelLevel = FuelLevel - evnt.NewFuelLevel };
}

public record Meter(
  int Value
);
