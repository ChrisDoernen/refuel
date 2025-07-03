using Core.Shared;
using EventSourcingDB;
using MediatR;

namespace Core.Tanks.Refilling;

public record LogRefilledCommand(
  Guid ClubId,
  Guid TankId,
  int NewFuelLevel
) : IRequest;

public class LogRefilledCommandHandler(
  IMediator mediator,
  IEventStoreFactory eventStoreFactory
) : IRequestHandler<LogRefilledCommand>
{
  public async Task Handle(
    LogRefilledCommand command,
    CancellationToken cancellationToken
  )
  {
    var tank = await mediator.Send(new GetTankQuery(command.ClubId, command.TankId), cancellationToken);

    if (command.NewFuelLevel > tank.Capacity)
    {
      throw new InvalidOperationException("New fuel level exceeds tank capacity.");
    }

    var refilledEvent = new RefilledEventV1(command.NewFuelLevel);
    var candidate = new EventCandidate(
      Subject: $"/tanks/{command.TankId}",
      Data: refilledEvent
    );
    await eventStoreFactory
      .ForTenant(command.ClubId)
      .StoreEvents(
        [candidate],
        [tank.GetIsSubjectOnEventIdPrecondition()],
        cancellationToken
      );
  }
}
