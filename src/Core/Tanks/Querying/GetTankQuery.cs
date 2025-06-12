using EventSourcingDbClient;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Core.Tanks.Querying;

public record GetTankQuery(
  Guid Id
) : IRequest<Tank>;

public class GetTankQueryHandler(
  ILogger<GetTankQueryHandler> logger,
  IEventStore eventStore
) : IRequestHandler<GetTankQuery, Tank>
{
  public async Task<Tank> Handle(
    GetTankQuery query,
    CancellationToken cancellationToken
  )
  {
    logger.LogInformation("Get tank query");

    var events = await eventStore.GetEvents(
      $"tanks/{query.Id}",
      new ReadEventsOptions
      {
        Recursive = true
      },
      cancellationToken
    );

    return await events
      .Cast<ITankRelated>()
      .AggregateAsync(
        new Tank(),
        (tank, evnt) => tank.Apply(evnt),
        cancellationToken
      );
  }
}
