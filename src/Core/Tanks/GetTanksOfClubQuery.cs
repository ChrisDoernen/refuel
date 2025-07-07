using Core.Shared;
using Core.Tanks.Registration;
using EventSourcingDB;
using MediatR;

namespace Core.Tanks;

public record GetTanksOfClubQuery(
  Guid ClubId
) : IRequest<IEnumerable<Tank>>;

public class GetTanksOfClubQueryHandler(
  IEventStoreProvider eventStoreProvider,
  IMediator mediator
) : IRequestHandler<GetTanksOfClubQuery, IEnumerable<Tank>>
{
  public async Task<IEnumerable<Tank>> Handle(
    GetTanksOfClubQuery query,
    CancellationToken cancellationToken
  )
  {
    var eventQlQuery =
      $"""
       FROM e IN events
       WHERE e.type == '{EventType.Of<TankRegisteredEventV1>()}'
       PROJECT INTO e
       """;

    var events = await eventStoreProvider
      .ForClub(query.ClubId)
      .RunEventQlQuery(eventQlQuery, cancellationToken);

    return await events
      .Select(e => e.Data)
      .Cast<TankRegisteredEventV1>()
      .SelectAwait(async e => await mediator.Send(new GetTankQuery(query.ClubId, e.TankId), cancellationToken))
      .ToListAsync(cancellationToken);
  }
}
