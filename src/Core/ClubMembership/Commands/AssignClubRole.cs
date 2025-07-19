using Core.ClubMembership.Events;
using Core.ClubMembership.Models;
using Core.Infrastructure.Authorization;
using Core.Infrastructure.Cqrs;
using EventSourcing;
using MediatR;

namespace Core.ClubMembership.Commands;

public record AssignClubRoleCommand(
  Guid ClubId,
  Guid MemberId,
  string RoleId
) : IRequest;

public class AssignClubRoleCommandHandler(
  IReplayService<ClubMember> replayService,
  IEventStoreProvider eventStoreProvider
) : IRequestHandler<AssignClubRoleCommand>
{
  public async Task Handle(
    AssignClubRoleCommand command,
    CancellationToken cancellationToken
  )
  {
    var auditTrail = await replayService.GetAuditTrail(
      command.ClubId,
      new Subject($"/members/{command.MemberId}"),
      cancellationToken
    );

    var member = auditTrail.CurrentState;
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
      Subject: new Subject($"/members/{command.MemberId}"),
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
    // UsePolicy(new UserHasRolePolicy(UserRoles.GlobalAdmin));
  }
}
