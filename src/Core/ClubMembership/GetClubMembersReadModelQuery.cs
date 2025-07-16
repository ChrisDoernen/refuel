using System.Linq.Expressions;
using Core.Infrastructure.Cqrs;
using Core.Infrastructure.ReadModels;
using MediatR;

namespace Core.ClubMembership;

public record GetClubMembersReadModelQuery(
  Guid ClubId
) : IRequest<IEnumerable<StateChange<ClubMember>>>;

public class GetClubMembersQueryHandler(
  IReadModelRepository<ClubMember> repository
) : IRequestHandler<GetClubMembersReadModelQuery, IEnumerable<StateChange<ClubMember>>>
{
  public async Task<IEnumerable<StateChange<ClubMember>>> Handle(
    GetClubMembersReadModelQuery query,
    CancellationToken cancellationToken
  )
  {
    Expression<Func<ClubMember, bool>> filter = m => m.ClubId == query.ClubId;

    return await repository.Filter(filter, cancellationToken);
  }
}
