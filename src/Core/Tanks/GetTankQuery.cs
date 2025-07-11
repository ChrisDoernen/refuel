using App.Cqrs;
using Core.Shared;
using EventSourcingDb.Types;
using MediatR;

namespace Core.Tanks;

public record GetTankQuery(
  Guid ClubId,
  Guid TankId
) : IRequest<Tank>;

public class GetTankQueryHandler(
  IEventStoreProvider eventStoreProvider
) : IRequestHandler<GetTankQuery, Tank>
{
  public async Task<Tank> Handle(
    GetTankQuery query,
    CancellationToken cancellationToken
  )
  {
    var events = eventStoreProvider
      .ForClub(query.ClubId)
      .GetEvents($"/tanks/{query.TankId}",
        new ReadEventsOptions(true),
        cancellationToken
      );

    var changes = events.Select(Change.FromEvent);
    var tank = await AuditTrail<Tank>.Pristine().Replay(changes);

    return tank.GetAudited();
  }
}
