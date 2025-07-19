using Core.Clubs.Models;
using Core.Infrastructure.Cqrs;
using MediatR;
using MongoDB;

namespace Core.Clubs.Commands;

public record CreateClubCommand(
  string Name,
  string TenantId,
  string? Description
) : IRequest<Guid>;

public class CreateClubCommandHandler(
  IDocumentStore<Club> clubStore,
  IEventStoreProvider eventStoreProvider
) : IRequestHandler<CreateClubCommand, Guid>
{
  public async Task<Guid> Handle(
    CreateClubCommand command,
    CancellationToken cancellationToken
  )
  {
    var club = new Club(command.Name, command.TenantId, command.Description);

    await clubStore.CreateOne(club, cancellationToken);

    eventStoreProvider.RegisterTenant(club.Id, club.TenantId);

    return club.Id;
  }
}
