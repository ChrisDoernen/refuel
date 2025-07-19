using Core.ClubMembership.Events;
using Core.ClubMembership.Models;
using Core.Infrastructure.Authorization;
using Core.Infrastructure.Cqrs;
using EventSourcing;
using MediatR;

namespace Core.ClubMembership.Commands;

public record AssignTankRoleCommand(
  Guid ClubId,
  Guid MemberId,
  Guid TankId,
  string RoleId
) : IRequest;

public class AssignTankRoleCommandHandler(
  IReplayService<ClubMemberSecurity> replayService,
  IEventStoreProvider eventStoreProvider
) : IRequestHandler<AssignTankRoleCommand>
{
  public async Task Handle(
    AssignTankRoleCommand command,
    CancellationToken cancellationToken
  )
  {
    var auditTrail = await replayService.GetAuditTrail(
      command.ClubId,
      new Subject($"/members/{command.MemberId}"),
      cancellationToken
    );

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
      Subject: new Subject($"/members/{clubMember.Id}/security"),
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

public class AssignTankRoleCommandAuthorizer : Authorizer<AssignTankRoleCommand>
{
  public override async Task BuildPolicy(AssignTankRoleCommand command)
  {
    // UsePolicy(new ClubMemberHasRolePolicy(command.ClubId, ClubRoles.Admin));
  }
}
