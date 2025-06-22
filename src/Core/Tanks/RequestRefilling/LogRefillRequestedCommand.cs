using Core.Shared;
using EventSourcingDB;
using MediatR;

namespace Core.Tanks.RequestRefilling;

public record LogRefillRequestedCommand(
  Guid TankId
) : IRequest;

public class LogRefillRequestedCommandHandler(
  IMediator mediator,
  IEventStore eventStore
) : IRequestHandler<LogRefillRequestedCommand>
{
  public async Task Handle(
    LogRefillRequestedCommand command,
    CancellationToken cancellationToken
  )
  {
    var tank = await mediator.Send(new GetTankQuery(command.TankId), cancellationToken);
    
    var refillRequestedEvent = new RefillRequestedEventV1();
    var candidate = new EventCandidate(
      Subject: $"/tanks/{command.TankId}",
      Data: refillRequestedEvent
    );
    await eventStore.StoreEvents(
      [candidate],
      [tank.GetIsSubjectOnEventIdPrecondition()],
      cancellationToken
    );
  }
}
