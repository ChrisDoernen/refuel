using System.Linq.Expressions;
using Core.ClubMembership.Projections;
using Core.Infrastructure.Projections;
using MediatR;

namespace Core.ClubMembership.Queries;

public record GetClubMembersReadModelQuery(
  Guid ClubId
) : IRequest<IEnumerable<ClubMember>>;

public class GetClubMembersQueryHandler(
  IIdentifiedProjectionRepository<ClubMember> repository
) : IRequestHandler<GetClubMembersReadModelQuery, IEnumerable<ClubMember>>
{
  public async Task<IEnumerable<ClubMember>> Handle(
    GetClubMembersReadModelQuery query,
    CancellationToken cancellationToken
  )
  {
    Expression<Func<ClubMember, bool>> filter = m => m.ClubId == query.ClubId;

    var change = await repository.Filter(filter, cancellationToken);

    return change.Select(c => c.State);
  }
}
