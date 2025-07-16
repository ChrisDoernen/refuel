using App.Cqrs;
using App.ReadModels;
using EventSourcing;
using EventSourcingDb.Types;
using MediatR;

namespace Core.ClubMembership;

public record GetClubMemberReadModelQuery(
  Guid MemberId
) : IRequest<StateChange<ClubMember>>;

public class GetClubMemberReadModelQueryHandler(
  IReadModelRepository<ClubMember> repository
) : IRequestHandler<GetClubMemberReadModelQuery, StateChange<ClubMember>>
{
  public async Task<StateChange<ClubMember>> Handle(
    GetClubMemberReadModelQuery query,
    CancellationToken cancellationToken
  )
  {
    return await repository.FindById(query.MemberId, cancellationToken);
  }
}
