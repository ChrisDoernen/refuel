using Core.ClubMembership.Joining;
using Core.Clubs;
using Core.Shared;
using EventSourcingDB;
using MediatR;

namespace Core.ClubMembership;

public record GetClubMembershipQuery(
  Guid UserId
) : IRequest<IEnumerable<ClubMember>>;

public class GetClubMembershipQueryHandler(
  IEventStoreProvider eventStoreProvider,
  IMediator mediator
) : IRequestHandler<GetClubMembershipQuery, IEnumerable<ClubMember>>
{
  public async Task<IEnumerable<ClubMember>> Handle(
    GetClubMembershipQuery query,
    CancellationToken cancellationToken
  )
  {
    var clubs = await mediator.Send(new GetClubsQuery(), cancellationToken);

    var userMemberships = new List<ClubMember>();

    foreach (var club in clubs)
    {
      var eventQlQuery =
        $"""
         FROM e IN events
         WHERE e.type == '{EventType.Of<UserJoinedClubEventV1>()}'
          AND e.data.userId == '{query.UserId}'
         PROJECT INTO e
         """;

      var events = await eventStoreProvider
        .ForClub(club.Id)
        .RunEventQlQuery(eventQlQuery, cancellationToken);

      var userJoinedEvents = await events
        .Select(e => e.Data)
        .Cast<UserJoinedClubEventV1>()
        .SelectAwait(async e => await mediator.Send(new GetClubMemberQuery(club.Id, e.UserId), cancellationToken))
        .ToListAsync(cancellationToken);

      userMemberships.AddRange(userJoinedEvents);
    }

    return userMemberships;
  }
}
