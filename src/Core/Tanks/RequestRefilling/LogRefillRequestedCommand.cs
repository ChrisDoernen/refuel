using Core.Tanks.Querying;
using EventSourcingDbClient;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Core.Tanks.RequestRefilling;

public record LogRefillRequestedCommand(
  Guid TankId
) : IRequest;

public class LogRefillRequestedCommandHandler(
  IMediator mediator,
  IEventStore eventStore,
  ILogger<LogRefillRequestedCommandHandler> logger
) : IRequestHandler<LogRefillRequestedCommand>
{
  public async Task Handle(
    LogRefillRequestedCommand command,
    CancellationToken cancellationToken
  )
  {
    logger.LogInformation("Log refill requested command");

    var tank = await mediator.Send(new GetTankQuery(command.TankId), cancellationToken);
    
    var refillRequestedEvent = new RefillRequestedEventV1();
    var candidate = new EventCandidate(
      Subject: $"/tanks/{command.TankId}",
      Data: refillRequestedEvent
    );
    await eventStore.StoreEvents(
      [candidate],
      [new IsSubjectOnEventId(tank.LastChange!.Subject, tank.LastChange.Id)],
      cancellationToken
    );
  }
}
