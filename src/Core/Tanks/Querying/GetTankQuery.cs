using Core.Shared;
using EventSourcingDbClient;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Core.Tanks.Querying;

public record GetTankQuery(
  Guid Id
) : IRequest<AuditTrail<Tank>>;

public class GetTankQueryHandler(
  ILogger<GetTankQueryHandler> logger,
  IEventStore eventStore
) : IRequestHandler<GetTankQuery, AuditTrail<Tank>>
{
  public async Task<AuditTrail<Tank>> Handle(
    GetTankQuery query,
    CancellationToken cancellationToken
  )
  {
    logger.LogInformation("Get tank query");

    var events = await eventStore.GetEvents(
      $"/tanks/{query.Id}",
      new ReadEventsOptions
      {
        Recursive = true
      },
      cancellationToken
    );

    var changes = events.Select(Change.FromEvent);

    var auditTrail = await AuditTrail<Tank>.Pristine().Replay(changes);

    return auditTrail;
  }
}
