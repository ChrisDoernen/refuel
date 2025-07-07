using Core.Clubs;
using Core.Shared;
using Core.Shared.Authorization;
using Core.Users;
using EventSourcingDB;
using MediatR;

namespace Core.ClubMembership.ClubRoleAssignment;

public record AssignClubRoleCommand(
  Guid ClubId,
  Guid MemberId,
  string RoleId
) : IRequest;

public class AssignClubRoleCommandHandler(
  IMediator mediator,
  IEventStoreProvider eventStoreProvider
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
      MemberId: member.Id,
      RoleId: command.RoleId
    );
    var candidate = new EventCandidate(
      Subject: $"/members/{command.MemberId}",
      Data: clubRoleAssignedEvent
    );
    await eventStoreProvider
      .ForClub(command.ClubId)
      .StoreEvents(
        [candidate],
        [member.GetIsSubjectOnEventIdPrecondition()],
        cancellationToken
      );
  }
}

public class AssignClubRoleCommandAuthorizer : Authorizer<AssignClubRoleCommand>
{
  public override async Task BuildPolicy(AssignClubRoleCommand command)
  {
    // UsePolicy(new ClubMemberHasRolePolicy(command.ClubId, ClubRoles.Admin));
    // UsePolicy(new UserHasRolePolicy(UserRoles.GlobalAdmin));
  }
}
