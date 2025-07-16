using Core.Infrastructure.Cqrs;
using MediatR;

namespace Core.Tanks;

public record GetTanksOfClubQuery(
  Guid ClubId
) : IRequest<IEnumerable<Tank>>;

public class GetTanksOfClubQueryHandler(
  IEventStoreProvider eventStoreProvider,
  IMediator mediator
) : IRequestHandler<GetTanksOfClubQuery, IEnumerable<Tank>>
{
  public async Task<IEnumerable<Tank>> Handle(
    GetTanksOfClubQuery query,
    CancellationToken cancellationToken
  )
  {
    return new List<Tank>();
  }
}
