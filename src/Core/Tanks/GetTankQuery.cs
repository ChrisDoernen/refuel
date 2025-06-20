using Core.Shared;
using EventSourcingDB;
using MediatR;

namespace Core.Tanks;

public record GetTankQuery(
  Guid TankId
) : IRequest<AuditTrail<Tank>>;

public class GetTankQueryHandler(
  IEventStore eventStore
) : IRequestHandler<GetTankQuery, AuditTrail<Tank>>
{
  public async Task<AuditTrail<Tank>> Handle(
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

    return await AuditTrail<Tank>.Pristine().Replay(changes);
  }
}
