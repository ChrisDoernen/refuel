using Core.Clubs.Creation;
using EventSourcingDB;
using MediatR;

namespace Core.Clubs;

public record GetClubsQuery : IRequest<IEnumerable<Club>>;

public class GetClubsQueryHandler(
  IEventStore eventStore,
  IMediator mediator
) : IRequestHandler<GetClubsQuery, IEnumerable<Club>>
{
  public async Task<IEnumerable<Club>> Handle(
    GetClubsQuery query,
    CancellationToken cancellationToken
  )
  {
    var eventQlQuery =
      $"""
       FROM e IN events
       WHERE e.type == '{EventType.Of<ClubCreatedEventV1>()}'
       PROJECT INTO e
       """;

    var events = await eventStore.RunEventQlQuery(eventQlQuery, cancellationToken);

    return await events
      .Select(e => e.Data)
      .Cast<ClubCreatedEventV1>()
      .SelectAwait(async e => await mediator.Send(new GetClubQuery(e.Id), cancellationToken))
      .ToListAsync(cancellationToken);
  }
}
