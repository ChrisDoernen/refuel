using Core.Infrastructure.Cqrs;
using Core.Tanks.Events;
using Core.Tanks.Projections;
using EventSourcing;
using MediatR;

namespace Core.Tanks.Commands;

public record LogRefillRequestedCommand(
  Guid ClubId,
  Guid TankId
) : IRequest;

public class LogRefillRequestedCommandHandler(
  IReplayService<Tank> replayService,
  IEventStoreProvider eventStoreProvider
) : IRequestHandler<LogRefillRequestedCommand>
{
  public async Task Handle(
    LogRefillRequestedCommand command,
    CancellationToken cancellationToken
  )
  {
    var tank = await replayService.GetAuditTrail(
      command.ClubId,
      new Subject($"/tanks/{command.TankId}"),
      cancellationToken
    );
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
