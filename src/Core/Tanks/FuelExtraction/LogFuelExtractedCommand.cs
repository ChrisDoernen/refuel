using Core.Tanks.Querying;
using EventSourcingDbClient;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Core.Tanks.FuelExtraction;

public record LogFuelExtractedCommand(
  Guid TankId,
  int AmountExtracted
) : IRequest;

public class LogFuelExtractedCommandHandler(
  IMediator mediator,
  IEventStore eventStore,
  ILogger<LogFuelExtractedCommandHandler> logger
) : IRequestHandler<LogFuelExtractedCommand>
{
  public async Task Handle(
    LogFuelExtractedCommand command,
    CancellationToken cancellationToken
  )
  {
    logger.LogInformation("Fuel extracted command");

    var tank = await mediator.Send(new GetTankQuery(command.TankId), cancellationToken);

    tank.EnsureHasChanges();
    if (tank.CurrentState.FuelLevel - command.AmountExtracted < 0)
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
      cancellationToken: cancellationToken
    );
  }
}
