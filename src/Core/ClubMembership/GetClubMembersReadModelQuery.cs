using System.Linq.Expressions;
using Core.Infrastructure.Cqrs;
using Core.Infrastructure.ReadModels;
using MediatR;

namespace Core.ClubMembership;

public record GetClubMembersReadModelQuery(
  Guid ClubId
) : IRequest<IEnumerable<ReadModelChange<ClubMember>>>;

public class GetClubMembersQueryHandler(
  IIdentifiedReadModelRepository<ClubMember> repository
) : IRequestHandler<GetClubMembersReadModelQuery, IEnumerable<ReadModelChange<ClubMember>>>
{
  public async Task<IEnumerable<ReadModelChange<ClubMember>>> Handle(
    GetClubMembersReadModelQuery query,
    CancellationToken cancellationToken
  )
  {
    Expression<Func<ClubMember, bool>> filter = m => m.ClubId == query.ClubId;

    return await repository.Filter(filter, cancellationToken);
  }
}
