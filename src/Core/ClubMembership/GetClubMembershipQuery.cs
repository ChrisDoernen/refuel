using System.Linq.Expressions;
using Core.Infrastructure.Cqrs;
using Core.Infrastructure.ReadModels;
using MediatR;

namespace Core.ClubMembership;

public record GetClubMembershipQuery(
  Guid UserId
) : IRequest<IEnumerable<StateChange<ClubMember>>>;

public class GetClubMembershipQueryHandler(
  IReadModelRepository<ClubMember> repository
) : IRequestHandler<GetClubMembershipQuery, IEnumerable<StateChange<ClubMember>>>
{
  public async Task<IEnumerable<StateChange<ClubMember>>> Handle(
    GetClubMembershipQuery query,
    CancellationToken cancellationToken
  )
  {
    Expression<Func<ClubMember, bool>> filter = m => m.Id == query.UserId;

    return await repository.Filter(filter, cancellationToken);
  }
}
