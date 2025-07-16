using Core.Infrastructure.Cqrs;
using EventSourcing;
using EventSourcingDb.Types;
using MediatR;

namespace Core.Tanks;

public record GetTankAuditTrailQuery(
  Guid ClubId,
  Guid TankId
) : IRequest<AuditTrail<Tank>>;

public class GetTankQueryHandler(
  IEventStoreProvider eventStoreProvider
) : IRequestHandler<GetTankAuditTrailQuery, AuditTrail<Tank>>
{
  public async Task<AuditTrail<Tank>> Handle(
    GetTankAuditTrailQuery query,
    CancellationToken cancellationToken
  )
  {
    var events = eventStoreProvider
      .ForClub(query.ClubId)
      .GetEvents(
        new Subject($"/tanks/{query.TankId}"),
        new ReadEventsOptions(true),
        cancellationToken
      );

    var replay = await Replay<Tank>.New().ApplyEventStream(events);

    return replay.GetAuditTrail();
  }
}
