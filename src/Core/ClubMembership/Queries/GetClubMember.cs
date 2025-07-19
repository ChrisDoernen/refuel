using Core.ClubMembership.Projections;
using Core.Infrastructure.Projections;
using MediatR;

namespace Core.ClubMembership.Queries;

public record GetClubMemberQuery(
  Guid MemberId
) : IRequest<ClubMember>;

public class GetClubMemberReadModelQueryHandler(
  IIdentifiedProjectionRepository<ClubMember> repository
) : IRequestHandler<GetClubMemberQuery, ClubMember>
{
  public async Task<ClubMember> Handle(
    GetClubMemberQuery query,
    CancellationToken cancellationToken
  )
  {
    var change = await repository.GetById(query.MemberId, cancellationToken);

    return change.State;
  }
}
