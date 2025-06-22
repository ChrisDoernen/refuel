using Core.Shared;
using EventSourcingDB;
using MediatR;

namespace Core.Tanks.MeterInitialization;

public record InitializeMeterCommand(
  Guid TankId
) : IRequest;

public class InitializeMeterCommandHandler(
  IMediator mediator,
  IEventStore eventStore
) : IRequestHandler<InitializeMeterCommand>
{
  public async Task Handle(
    InitializeMeterCommand command,
    CancellationToken cancellationToken
  )
  {
    var tank = await mediator.Send(new GetTankQuery(command.TankId), cancellationToken);
    tank.EnsureNotPristine();

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
