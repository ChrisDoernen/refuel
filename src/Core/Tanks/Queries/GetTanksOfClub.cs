using System.Linq.Expressions;
using Core.Infrastructure.Projections;
using Core.Tanks.Projections;
using MediatR;

namespace Core.Tanks.Queries;

public record GetTanksOfClubQuery(
  Guid ClubId
) : IRequest<IEnumerable<Tank>>;

public class GetTanksOfClubQueryHandler(
  IIdentifiedProjectionRepository<Tank> repository
) : IRequestHandler<GetTanksOfClubQuery, IEnumerable<Tank>>
{
  public async Task<IEnumerable<Tank>> Handle(
    GetTanksOfClubQuery query,
    CancellationToken cancellationToken
  )
  {
    Expression<Func<Tank, bool>> filter = t => t.ClubId == query.ClubId;

    var changes = await repository.Filter(filter, cancellationToken);

    return changes.Select(c => c.State);
  }
}
