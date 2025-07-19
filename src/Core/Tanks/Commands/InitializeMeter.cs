using Core.Infrastructure.Cqrs;
using Core.Tanks.Events;
using Core.Tanks.Projections;
using EventSourcing;
using EventSourcingDb.Types;
using MediatR;
using EventCandidate = EventSourcing.EventCandidate;
using IsSubjectPristinePrecondition = EventSourcing.IsSubjectPristinePrecondition;

namespace Core.Tanks.Commands;

public record InitializeMeterCommand(
  Guid ClubId,
  Guid TankId
) : IRequest;

public class InitializeMeterCommandHandler(
  IReplayService<Tank> replayService,
  IEventStoreProvider eventStoreProvider
) : IRequestHandler<InitializeMeterCommand>
{
  public async Task Handle(
    InitializeMeterCommand command,
    CancellationToken cancellationToken
  )
  {
    var tank = await replayService.GetAuditTrail(
      command.ClubId,
      new Subject($"/tanks/{command.TankId}"),
      cancellationToken
    );
    tank.EnsureNotPristine();

    var meterInitializedEvent = new MeterInitializedEventV1();
    var candidate = new EventCandidate(
      Subject: new Subject($"/tanks/{command.TankId}/meter"),
      Data: meterInitializedEvent
    );
    await eventStoreProvider
      .ForClub(command.ClubId)
      .StoreEvents(
        [candidate],
        [new IsSubjectPristinePrecondition(candidate.Subject)],
        cancellationToken
      );
  }
}
