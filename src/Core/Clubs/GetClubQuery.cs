using Core.Shared;
using EventSourcingDB;
using MediatR;

namespace Core.Clubs;

public record GetClubQuery(
  Guid ClubId
) : IRequest<Club>;

public class GetClubQueryHandler(
  IEventStore eventStore
) : IRequestHandler<GetClubQuery, Club>
{
  public async Task<Club> Handle(
    GetClubQuery query,
    CancellationToken cancellationToken
  )
  {
    var events = await eventStore.GetEvents(
      $"/clubs/{query.ClubId}",
      new ReadEventsOptions
      {
        Recursive = true
      },
      cancellationToken
    );

    var changes = events.Select(Change.FromEvent);
    var club = await AuditTrail<Club>.Pristine().Replay(changes);

    return club.GetAudited();
  }
}
