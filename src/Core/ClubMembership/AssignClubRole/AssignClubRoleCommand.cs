using Core.Shared;
using EventSourcingDB;
using MediatR;

namespace Core.ClubMembership.AssignClubRole;

public record AssignClubRoleCommand(
  Guid ClubId,
  Guid MemberId,
  string RoleId
) : IRequest;

public class AssignClubRoleCommandHandler(
  IMediator mediator,
  IEventStoreFactory eventStoreFactory
) : IRequestHandler<AssignClubRoleCommand>
{
  public async Task Handle(
    AssignClubRoleCommand command,
    CancellationToken cancellationToken
  )
  {
    var member = await mediator.Send(new GetClubMemberQuery(command.ClubId, command.MemberId), cancellationToken);

    if (member.RoleIds.Any(r => r.Equals(command.RoleId)))
    {
      // Nothing to do, as the desired state is already reached.
      return;
    }

    var clubRoleAssignedEvent = new ClubRoleAssignedEventV1(
      command.MemberId,
      command.RoleId
    );
    var candidate = new EventCandidate(
      Subject: $"/members/{command.MemberId}",
      Data: clubRoleAssignedEvent
    );
    await eventStoreFactory
      .ForTenant(command.ClubId)
      .StoreEvents(
        [candidate],
        [member.GetIsSubjectOnEventIdPrecondition()],
        cancellationToken
      );
  }
}
