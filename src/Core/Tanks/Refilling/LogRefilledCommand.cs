using Core.Infrastructure.Cqrs;
using EventSourcing;
using MediatR;

namespace Core.Tanks.Refilling;

public record LogRefilledCommand(
  Guid ClubId,
  Guid TankId,
  int NewFuelLevel
) : IRequest;

public class LogRefilledCommandHandler(
  IMediator mediator,
  IEventStoreProvider eventStoreProvider
) : IRequestHandler<LogRefilledCommand>
{
  public async Task Handle(
    LogRefilledCommand command,
    CancellationToken cancellationToken
  )
  {
    var auditTrail = await mediator.Send(new GetTankAuditTrailQuery(command.ClubId, command.TankId), cancellationToken);

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
