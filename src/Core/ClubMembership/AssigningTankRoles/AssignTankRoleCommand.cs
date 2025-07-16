using App.Authorization;
using App.Cqrs;
using Core.ClubMembership.AssigningClubRoles;
using EventSourcing;
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
    var auditTrail = await mediator.Send(new GetClubMemberAuditTrailQuery(command.ClubId, command.MemberId), cancellationToken);

    var clubMember = auditTrail.CurrentState;
    if (
      clubMember.TankRoleAssignments.TryGetValue(command.TankId, out var roles)
      && roles.Contains(command.RoleId)
    )
    {
      // Nothing to do, as the desired state is already reached.
      return;
    }

    var clubRoleAssignedEvent = new TankRoleAssignedEventV1(
      MemberId: clubMember.Id,
      TankId: command.TankId,
      RoleId: command.RoleId
    );
    var candidate = new EventCandidate(
      Subject: new Subject($"/members/{clubMember.Id}"),
      Data: clubRoleAssignedEvent
    );
    await eventStoreProvider
      .ForClub(command.ClubId)
      .StoreEvents(
        [candidate],
        [auditTrail.GetIsSubjectOnEventIdPrecondition()],
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
