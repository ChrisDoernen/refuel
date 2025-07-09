using Core.Shared;
using EventSourcingDb.Types;
using MediatR;
using EventCandidate = EventSourcing.EventCandidate;

namespace Core.Tanks.MeterInitialization;

public record InitializeMeterCommand(
  Guid ClubId,
  Guid TankId
) : IRequest;

public class InitializeMeterCommandHandler(
  IMediator mediator,
  IEventStoreProvider eventStoreProvider
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
    await eventStoreProvider
      .ForClub(command.ClubId)
      .StoreEvents(
        [candidate],
        [Precondition.IsSubjectPristinePrecondition(candidate.Subject)],
        cancellationToken
      );
  }
}
