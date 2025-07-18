using Core.Infrastructure.Cqrs;
using Core.Infrastructure.ReadModels;
using EventSourcing;
using EventSourcingDb.Types;
using MediatR;

namespace Core.ClubMembership;

public record GetClubMemberReadModelQuery(
  Guid MemberId
) : IRequest<ReadModelChange<ClubMember>>;

public class GetClubMemberReadModelQueryHandler(
  IIdentifiedReadModelRepository<ClubMember> repository
) : IRequestHandler<GetClubMemberReadModelQuery, ReadModelChange<ClubMember>>
{
  public async Task<ReadModelChange<ClubMember>> Handle(
    GetClubMemberReadModelQuery query,
    CancellationToken cancellationToken
  )
  {
    return await repository.GetById(query.MemberId, cancellationToken);
  }
}
