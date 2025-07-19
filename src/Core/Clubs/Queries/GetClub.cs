using Core.Clubs.Models;
using MediatR;
using MongoDB;

namespace Core.Clubs.Queries;

public record GetClubQuery(
  Guid ClubId
) : IRequest<Club>;

public class GetClubQueryHandler(
  IDocumentStore<Club> clubStore
) : IRequestHandler<GetClubQuery, Club>
{
  public async Task<Club> Handle(
    GetClubQuery query,
    CancellationToken cancellationToken
  )
  {
    return await clubStore.GetById(query.ClubId, cancellationToken);
  }
}
