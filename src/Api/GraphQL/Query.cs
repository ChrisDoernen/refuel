using Core.Tanks;
using Core.Tanks.Querying;
using MediatR;

namespace Api.GraphQL;

public class Query
{
  public async Task<Tank> GetTank(
    Guid tankId,
    IMediator mediator,
    CancellationToken cancellationToken
  )
  {
    throw new NotImplementedException();
  }
}
