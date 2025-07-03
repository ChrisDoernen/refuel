using Core.Shared;
using EventSourcingDB;
using MediatR;

namespace Core.Tanks;

public record GetTankQuery(
  Guid ClubId,
  Guid TankId
) : IRequest<Tank>;

public class GetTankQueryHandler(
  IEventStoreFactory eventStoreFactory
) : IRequestHandler<GetTankQuery, Tank>
{
  public async Task<Tank> Handle(
    GetTankQuery query,
    CancellationToken cancellationToken
  )
  {
    var events = await eventStoreFactory
      .ForTenant(query.ClubId)
      .GetEvents(
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
