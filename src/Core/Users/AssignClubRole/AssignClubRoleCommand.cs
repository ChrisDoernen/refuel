using Core.Clubs;
using Core.Shared;
using EventSourcingDB;
using MediatR;

namespace Core.Users.AssignClubRole;

public record AssignClubRoleCommand(
  Guid UserId,
  ClubRole Role
) : IRequest;

public class AssignClubRoleCommandHandler(
  IMediator mediator,
  IEventStore eventStore
) : IRequestHandler<AssignClubRoleCommand>
{
  public async Task Handle(
    AssignClubRoleCommand command,
    CancellationToken cancellationToken
  )
  {
    var user = await mediator.Send(new GetUserQuery(command.UserId), cancellationToken);

    if (user.ClubRoles.Any(r => r.Equals(command.Role)))
    {
      // Nothing to do, as the desired state is already reached.
      return;
    }

    var clubRoleAssignedEvent = new ClubRoleAssignedEventV1(
      command.UserId,
      command.Role
    );
    var candidate = new EventCandidate(
      Subject: $"/users/{command.UserId}",
      Data: clubRoleAssignedEvent
    );
    await eventStore.StoreEvents(
      [candidate],
      [user.GetIsSubjectOnEventIdPrecondition()],
      cancellationToken
    );
  }
}
