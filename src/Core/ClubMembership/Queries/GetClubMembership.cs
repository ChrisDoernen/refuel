using System.Linq.Expressions;
using Core.ClubMembership.Projections;
using Core.Infrastructure.Projections;
using MediatR;

namespace Core.ClubMembership.Queries;

public record GetClubMembershipQuery(
  Guid UserId
) : IRequest<IEnumerable<ClubMember>>;

public class GetClubMembershipQueryHandler(
  IIdentifiedProjectionRepository<ClubMember> repository
) : IRequestHandler<GetClubMembershipQuery, IEnumerable<ClubMember>>
{
  public async Task<IEnumerable<ClubMember>> Handle(
    GetClubMembershipQuery query,
    CancellationToken cancellationToken
  )
  {
    Expression<Func<ClubMember, bool>> filter = m => m.Id == query.UserId;

    var changes = await repository.Filter(filter, cancellationToken);

    return changes.Select(c => c.State);
  }
}
