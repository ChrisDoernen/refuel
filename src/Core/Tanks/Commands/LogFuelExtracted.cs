using Core.Infrastructure.Cqrs;
using Core.Tanks.Events;
using Core.Tanks.Projections;
using EventSourcing;
using MediatR;

namespace Core.Tanks.Commands;

public record LogFuelExtractedCommand(
  Guid ClubId,
  Guid TankId,
  int AmountExtracted
) : IRequest;

public class LogFuelExtractedCommandHandler(
  IReplayService<Tank> replayService,
  IEventStoreProvider eventStoreProvider
) : IRequestHandler<LogFuelExtractedCommand>
{
  public async Task Handle(
    LogFuelExtractedCommand command,
    CancellationToken cancellationToken
  )
  {
    var auditTrail = await replayService.GetAuditTrail(
      command.ClubId,
      new Subject($"/tanks/{command.TankId}"),
      cancellationToken
    );
    var tank = auditTrail.CurrentState;

    if (tank.FuelLevel - command.AmountExtracted < 0)
    {
      throw new InvalidOperationException("Fuel level cannot be negative.");
    }

    var fuelExtractedEvent = new FuelExtractedEventV1(command.AmountExtracted);
    var candidate = new EventCandidate(
      Subject: new Subject($"/tanks/{command.TankId}"),
      Data: fuelExtractedEvent
    );
    await eventStoreProvider
      .ForClub(command.ClubId)
      .StoreEvents(
        [candidate],
        [auditTrail.GetIsSubjectOnEventIdPrecondition()],
        cancellationToken
      );
  }
}
