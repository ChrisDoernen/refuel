using Core.Shared;
using Core.Tanks.FuelExtraction;
using Core.Tanks.Refilling;
using Core.Tanks.Registration;
using EventSourcingDbClient;

namespace Core.Tanks;

public record Tank : IReplayable<Tank>
{
  public Guid Id { get; private init; }
  public Guid ClubId { get; private init; }
  public string Name { get; private init; } = null!;
  public string Description { get; private init; } = null!;
  public int Capacity { get; private init; }
  public bool RefillRequested { get; private init; }
  public int FuelLevel { get; private init; }
  public Meter? Meter { get; private init; }

  public Tank Apply(IEventData evnt)
  {
    return evnt switch
    {
      TankRegisteredEventV1 tankRegisteredEvent => Apply(tankRegisteredEvent),
      FuelExtractedEventV1 fuelExtractedEvent => Apply(fuelExtractedEvent),
      RefillRequestedEventV1 refillRequested => Apply(refillRequested),
      RefilledEventV1 refilledEvent => Apply(refilledEvent),
      _ => throw new InvalidOperationException("Unknown event for tank"),
    };
  }

  private Tank Apply(TankRegisteredEventV1 evnt) => this with
  {
    Id = evnt.Id,
    FuelLevel = evnt.InitialFuelLevel,
    Capacity = evnt.Capacity,
    Name = evnt.Name,
    ClubId = evnt.Club,
    Description = evnt.Description
  };

  private Tank Apply(FuelExtractedEventV1 evnt) => this with { FuelLevel = FuelLevel - evnt.AmountExtracted };
  private Tank Apply(RefillRequestedEventV1 _) => this with { RefillRequested = true };
  private Tank Apply(RefilledEventV1 evnt) => this with { FuelLevel = evnt.NewFuelLevel, RefillRequested = false };
}

public record Meter(
  int Value
);
