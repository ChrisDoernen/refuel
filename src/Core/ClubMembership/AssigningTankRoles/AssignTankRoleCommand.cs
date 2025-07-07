using Core.ClubMembership.ClubRoleAssignment;
using Core.Shared;
using Core.Shared.Authorization;
using EventSourcingDB;
using MediatR;

namespace Core.ClubMembership.AssigningTankRoles;

public record AssignTankRoleCommand(
  Guid ClubId,
  Guid MemberId,
  Guid TankId,
  string RoleId
) : IRequest;

public class AssignTankRoleCommandHandler(
  IMediator mediator,
  IEventStoreProvider eventStoreProvider
) : IRequestHandler<AssignTankRoleCommand>
{
  public async Task Handle(
    AssignTankRoleCommand command,
    CancellationToken cancellationToken
  )
  {
    var member = await mediator.Send(new GetClubMemberQuery(command.ClubId, command.MemberId), cancellationToken);

    if (member.TankRoleAssignments.TryGetValue(command.TankId, out var roles) && roles.Contains(command.RoleId))
    {
      // Nothing to do, as the desired state is already reached.
      return;
    }

    var clubRoleAssignedEvent = new TankRoleAssignedEventV1(
      MemberId: member.Id,
      TankId: command.TankId,
      RoleId: command.RoleId
    );
    var candidate = new EventCandidate(
      Subject: $"/members/{member.Id}",
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
  }
}
