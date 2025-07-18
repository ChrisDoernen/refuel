using Core.Infrastructure.Cqrs;
using Core.Infrastructure.ReadModels;
using Core.Tanks.FuelExtraction;
using Core.Tanks.MeterInitialization;
using Core.Tanks.MeterReading;
using Core.Tanks.Refilling;
using Core.Tanks.Registration;
using Core.Tanks.RequestRefilling;
using EventSourcing;

namespace Core.Tanks;

public record Tank : IReplayable<Tank>, IIdentifiedReadModel
{
  public Guid Id { get; init; }
  public Guid ClubId { get; init; }
  public string Name { get; init; } = null!;
  public string Description { get; init; } = null!;
  public int Capacity { get; init; }
  public bool RefillRequested { get; init; }
  public int FuelLevel { get; init; }
  public Meter? Meter { get; init; }

  public Tank Apply(IEventData evnt)
  {
    return evnt switch
    {
      TankRegisteredEventV1 tankRegisteredEvent => Apply(tankRegisteredEvent),
      FuelExtractedEventV1 fuelExtractedEvent => Apply(fuelExtractedEvent),
      RefillRequestedEventV1 refillRequested => Apply(refillRequested),
      RefilledEventV1 refilledEvent => Apply(refilledEvent),
      MeterInitializedEventV1 refilledEvent => Apply(refilledEvent),
      MeterReadEventV1 refilledEvent => Apply(refilledEvent),
      _ => throw new InvalidOperationException("Unknown event for tank"),
    };
  }

  private Tank Apply(TankRegisteredEventV1 evnt) => this with
  {
    Id = evnt.TankId,
    FuelLevel = evnt.InitialFuelLevel,
    Capacity = evnt.Capacity,
    Name = evnt.Name,
    ClubId = evnt.Club,
    Description = evnt.Description
  };

  private Tank Apply(FuelExtractedEventV1 evnt) => this with { FuelLevel = FuelLevel - evnt.AmountExtracted };
  private Tank Apply(RefillRequestedEventV1 _) => this with { RefillRequested = true };
  private Tank Apply(RefilledEventV1 evnt) => this with { FuelLevel = evnt.NewFuelLevel, RefillRequested = false };
  private Tank Apply(MeterInitializedEventV1 _) => this with { Meter = new Meter { Value = 0 } };
  private Tank Apply(MeterReadEventV1 evnt) => this with { Meter = Meter with { Value = evnt.Value } };
}

public record Meter
{
  public int Value { get; set; }
}
