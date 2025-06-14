using Core.Tanks.Querying;
using EventSourcingDbClient;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Core.Tanks.MeterInitialization;

public record InitializeMeterCommand(
  Guid TankId
) : IRequest;

public class InitializeMeterCommandHandler(
  IMediator mediator,
  IEventStore eventStore,
  ILogger<InitializeMeterCommandHandler> logger
) : IRequestHandler<InitializeMeterCommand>
{
  public async Task Handle(
    InitializeMeterCommand command,
    CancellationToken cancellationToken
  )
  {
    logger.LogInformation("Initialize meter command");

    var tank = await mediator.Send(new GetTankQuery(command.TankId), cancellationToken);

    tank.EnsureHasChanges();
    var meterInitializedEvent = new MeterInitializedEventV1();
    var candidate = new EventCandidate(
      Subject: $"/tanks/{command.TankId}/meter",
      Data: meterInitializedEvent
    );
    await eventStore.StoreEvents(
      [candidate],
      [new IsSubjectPristine(candidate.Subject)],
      cancellationToken
    );
  }
}
