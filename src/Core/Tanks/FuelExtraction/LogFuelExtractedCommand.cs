using Core.Shared;
using EventSourcingDB;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Core.Tanks.FuelExtraction;

public record LogFuelExtractedCommand(
  Guid TankId,
  int AmountExtracted
) : IRequest;

public class LogFuelExtractedCommandHandler(
  IMediator mediator,
  IEventStore eventStore
) : IRequestHandler<LogFuelExtractedCommand>
{
  public async Task Handle(
    LogFuelExtractedCommand command,
    CancellationToken cancellationToken
  )
  {
    var tank = await mediator.Send(new GetTankQuery(command.TankId), cancellationToken);

    if (tank.FuelLevel - command.AmountExtracted < 0)
    {
      throw new InvalidOperationException("Fuel level cannot be negative.");
    }

    var fuelExtractedEvent = new FuelExtractedEventV1(command.AmountExtracted);
    var candidate = new EventCandidate(
      Subject: $"/tanks/{command.TankId}",
      Data: fuelExtractedEvent
    );
    await eventStore.StoreEvents(
      [candidate],
      [tank.GetIsSubjectOnEventIdPrecondition()],
      cancellationToken
    );
  }
}
