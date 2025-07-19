using Core.Infrastructure.Cqrs;
using Core.Tanks.Events;
using Core.Tanks.Projections;
using EventSourcing;
using MediatR;

namespace Core.Tanks.Commands;

public record LogRefilledCommand(
  Guid ClubId,
  Guid TankId,
  int NewFuelLevel
) : IRequest;

public class LogRefilledCommandHandler(
  IReplayService<Tank> replayService,
  IEventStoreProvider eventStoreProvider
) : IRequestHandler<LogRefilledCommand>
{
  public async Task Handle(
    LogRefilledCommand command,
    CancellationToken cancellationToken
  )
  {
    var auditTrail = await replayService.GetAuditTrail(
      command.ClubId,
      new Subject($"/tanks/{command.TankId}"),
      cancellationToken
    );
    
    var tank = auditTrail.CurrentState;
    
    if (command.NewFuelLevel > tank.Capacity)
    {
      throw new InvalidOperationException("New fuel level exceeds tank capacity.");
    }

    var refilledEvent = new RefilledEventV1(command.NewFuelLevel);
    var candidate = new EventCandidate(
      Subject: new Subject($"/tanks/{command.TankId}"),
      Data: refilledEvent
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
