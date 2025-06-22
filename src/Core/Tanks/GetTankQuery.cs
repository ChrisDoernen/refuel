using Core.Shared;
using EventSourcingDB;
using MediatR;

namespace Core.Tanks;

public record GetTankQuery(
  Guid TankId
) : IRequest<Tank>;

public class GetTankQueryHandler(
  IEventStore eventStore
) : IRequestHandler<GetTankQuery, Tank>
{
  public async Task<Tank> Handle(
    GetTankQuery query,
    CancellationToken cancellationToken
  )
  {
    var events = await eventStore.GetEvents(
      $"/tanks/{query.TankId}",
      new ReadEventsOptions
      {
        Recursive = true
      },
      cancellationToken
    );

    var changes = events.Select(Change.FromEvent);
    var tank = await AuditTrail<Tank>.Pristine().Replay(changes);

    return tank.GetAudited();
  }
}
