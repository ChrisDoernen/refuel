using Core.Shared;
using EventSourcingDB;
using MediatR;

namespace Core.Tanks.MeterInitialization;

public record InitializeMeterCommand(
  Guid ClubId,
  Guid TankId
) : IRequest;

public class InitializeMeterCommandHandler(
  IMediator mediator,
  IEventStoreFactory eventStoreFactory
) : IRequestHandler<InitializeMeterCommand>
{
  public async Task Handle(
    InitializeMeterCommand command,
    CancellationToken cancellationToken
  )
  {
    var tank = await mediator.Send(new GetTankQuery(command.ClubId, command.TankId), cancellationToken);
    tank.EnsureNotPristine();

    var meterInitializedEvent = new MeterInitializedEventV1();
    var candidate = new EventCandidate(
      Subject: $"/tanks/{command.TankId}/meter",
      Data: meterInitializedEvent
    );
    await eventStoreFactory
      .ForTenant(command.ClubId)
      .StoreEvents(
        [candidate],
        [new IsSubjectPristine(candidate.Subject)],
        cancellationToken
      );
  }
}
