using App.Cqrs;
using Core.ClubMembership.Joining;
using Core.Clubs;
using Core.Shared;
using EventSourcing;
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


    return userMemberships;
  }
}
