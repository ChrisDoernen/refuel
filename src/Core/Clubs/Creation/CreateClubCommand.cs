using MediatR;
using MongoDB;

namespace Core.Clubs.Creation;

public record CreateClubCommand(
  string Name,
  string? Description
) : IRequest<Guid>;

public class CreateClubCommandHandler(
  IDocumentStore<Club> clubStore
) : IRequestHandler<CreateClubCommand, Guid>
{
  public async Task<Guid> Handle(
    CreateClubCommand command,
    CancellationToken cancellationToken
  )
  {
    var club = new Club(command.Name, command.Description);

    await clubStore.CreateOne(club, cancellationToken);

    return club.Id;
  }
}
