using EventSourcingDB;
using MediatR;

namespace Core.Clubs.Creation;

public record CreateClubCommand(
  string Name,
  string? Description
) : IRequest<Guid>;

public class CreateClubCommandHandler(
  IEventStore eventStore
) : IRequestHandler<CreateClubCommand, Guid>
{
  public async Task<Guid> Handle(
    CreateClubCommand command,
    CancellationToken cancellationToken
  )
  {
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
