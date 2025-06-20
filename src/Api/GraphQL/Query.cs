using Core.Tanks;
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
