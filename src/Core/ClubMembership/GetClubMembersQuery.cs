using Core.ClubMembership.JoiningClubs;
using EventSourcingDB;
using MediatR;

namespace Core.ClubMembership;

public record GetClubMembersQuery(
  Guid ClubId
) : IRequest<IEnumerable<ClubMember>>;

public class GetClubMembersQueryHandler(
  IEventStoreFactory eventStoreFactory,
  IMediator mediator
) : IRequestHandler<GetClubMembersQuery, IEnumerable<ClubMember>>
{
  public async Task<IEnumerable<ClubMember>> Handle(
    GetClubMembersQuery query,
    CancellationToken cancellationToken
  )
  {
    var eventQlQuery =
      $"""
       FROM e IN events
       WHERE e.type == '{EventType.Of<UserJoinedClubEventV1>()}'
       PROJECT INTO e
       """;

    var events = await eventStoreFactory
      .ForTenant(query.ClubId)
      .RunEventQlQuery(eventQlQuery, cancellationToken);

    return await events
      .Select(e => e.Data)
      .Cast<UserJoinedClubEventV1>()
      .SelectAwait(async e => await mediator.Send(new GetClubMemberQuery(query.ClubId, e.UserId), cancellationToken))
      .ToListAsync(cancellationToken);
  }
}
