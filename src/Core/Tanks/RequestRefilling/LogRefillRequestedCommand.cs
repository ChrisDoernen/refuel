using Core.Infrastructure.Cqrs;
using EventSourcing;
using MediatR;

namespace Core.Tanks.RequestRefilling;

public record LogRefillRequestedCommand(
  Guid ClubId,
  Guid TankId
) : IRequest;

public class LogRefillRequestedCommandHandler(
  IMediator mediator,
  IEventStoreProvider eventStoreProvider
) : IRequestHandler<LogRefillRequestedCommand>
{
  public async Task Handle(
    LogRefillRequestedCommand command,
    CancellationToken cancellationToken
  )
  {
    var tank = await mediator.Send(new GetTankAuditTrailQuery(command.ClubId, command.TankId), cancellationToken);
    
    var refillRequestedEvent = new RefillRequestedEventV1();
    var candidate = new EventCandidate(
      Subject: new Subject($"/tanks/{command.TankId}"),
      Data: refillRequestedEvent
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
