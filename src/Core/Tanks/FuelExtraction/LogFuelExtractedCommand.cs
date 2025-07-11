using App.Cqrs;
using Core.Shared;
using EventSourcing;
using MediatR;

namespace Core.Tanks.FuelExtraction;

public record LogFuelExtractedCommand(
  Guid ClubId,
  Guid TankId,
  int AmountExtracted
) : IRequest;

public class LogFuelExtractedCommandHandler(
  IMediator mediator,
  IEventStoreProvider eventStoreProvider
) : IRequestHandler<LogFuelExtractedCommand>
{
  public async Task Handle(
    LogFuelExtractedCommand command,
    CancellationToken cancellationToken
  )
  {
    var tank = await mediator.Send(new GetTankQuery(command.ClubId, command.TankId), cancellationToken);

    if (tank.FuelLevel - command.AmountExtracted < 0)
    {
      throw new InvalidOperationException("Fuel level cannot be negative.");
    }

    var fuelExtractedEvent = new FuelExtractedEventV1(command.AmountExtracted);
    var candidate = new EventCandidate(
      Subject: $"/tanks/{command.TankId}",
      Data: fuelExtractedEvent
    );
    await eventStoreProvider
      .ForClub(command.ClubId)
      .StoreEvents(
        [candidate],
        [tank.GetIsSubjectOnEventIdPrecondition()],
        cancellationToken
      );
  }
}
