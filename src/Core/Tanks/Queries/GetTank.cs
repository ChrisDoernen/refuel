using Core.Infrastructure.Projections;
using Core.Tanks.Projections;
using MediatR;

namespace Core.Tanks.Queries;

public record GetTankQuery(
  Guid TankId
) : IRequest<Tank>;

public class GetTankQueryHandler(
  IIdentifiedProjectionRepository<Tank> repository
) : IRequestHandler<GetTankQuery, Tank>
{
  public async Task<Tank> Handle(
    GetTankQuery query,
    CancellationToken cancellationToken
  )
  {
    var change = await repository.GetById(query.TankId, cancellationToken);

    return change.State;
  }
}
