using Core.ClubMembership.Joining;
using Core.Shared;
using EventSourcing;
using MediatR;

namespace Core.ClubMembership;

public record GetClubMembersQuery(
  Guid ClubId
) : IRequest<IEnumerable<ClubMember>>;

public class GetClubMembersQueryHandler(
  IEventStoreProvider eventStoreProvider,
  IMediator mediator
) : IRequestHandler<GetClubMembersQuery, IEnumerable<ClubMember>>
{
  public async Task<IEnumerable<ClubMember>> Handle(
    GetClubMembersQuery query,
    CancellationToken cancellationToken
  )
  {
    return new List<ClubMember>();
  }
}
