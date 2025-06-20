using EventSourcingDB;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Core.Clubs.Creation;

public record CreateClubCommand(
  Guid Id,
  string Name,
  string? Description
) : IRequest<Guid>;

public class CreateClubCommandHandler(
  ILogger<CreateClubCommandHandler> logger,
  IEventStore eventStore
) : IRequestHandler<CreateClubCommand, Guid>
{
  public async Task<Guid> Handle(
    CreateClubCommand command,
    CancellationToken cancellationToken
  )
  {
    logger.LogInformation("Create club command");

    var evnt = new ClubCreatedEventV1(
      Guid.CreateVersion7(),
      command.Name,
      command.Description
    );

    var candidate = new EventCandidate(
      Subject: $"/clubs/{evnt.Id}",
      Data: evnt
    );

    await eventStore.StoreEvents(
      [candidate],
      [new IsSubjectPristine(candidate.Subject)],
      cancellationToken
    );

    return evnt.Id;
  }
}
