using Core.Clubs.Models;
using MediatR;
using MongoDB;

namespace Core.Clubs.Queries;

public record GetClubsQuery : IRequest<IEnumerable<Club>>;

public class GetClubsQueryHandler(
  IDocumentStore<Club> clubStore
) : IRequestHandler<GetClubsQuery, IEnumerable<Club>>
{
  public async Task<IEnumerable<Club>> Handle(
    GetClubsQuery query,
    CancellationToken cancellationToken
  )
  {
    return await clubStore.GetAll(cancellationToken);
  }
}
