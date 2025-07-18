using System.Linq.Expressions;
using Core.Infrastructure.Cqrs;
using Core.Infrastructure.ReadModels;
using MediatR;

namespace Core.ClubMembership;

public record GetClubMembershipQuery(
  Guid UserId
) : IRequest<IEnumerable<ReadModelChange<ClubMember>>>;

public class GetClubMembershipQueryHandler(
  IIdentifiedReadModelRepository<ClubMember> repository
) : IRequestHandler<GetClubMembershipQuery, IEnumerable<ReadModelChange<ClubMember>>>
{
  public async Task<IEnumerable<ReadModelChange<ClubMember>>> Handle(
    GetClubMembershipQuery query,
    CancellationToken cancellationToken
  )
  {
    Expression<Func<ClubMember, bool>> filter = m => m.Id == query.UserId;

    return await repository.Filter(filter, cancellationToken);
  }
}
