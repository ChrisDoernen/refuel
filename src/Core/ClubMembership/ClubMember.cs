using App.Cqrs;
using Core.ClubMembership.AssigningTankRoles;
using Core.ClubMembership.ClubRoleAssignment;
using Core.ClubMembership.Joining;
using Core.Shared;
using EventSourcing;

namespace Core.ClubMembership;

public record ClubMember : Audited<ClubMember>, IReplayable<ClubMember>
{
  public Guid ClubId { get; private init; }
  public Guid Id { get; private init; }
  public string Email { get; private init; } = null!;
  public string FirstName { get; private init; } = null!;
  public string LastName { get; private init; } = null!;
  public IEnumerable<string> RoleIds { get; private init; } = [];
  public Dictionary<Guid, IEnumerable<string>> TankRoleAssignments { get; private init; } = new();

  public ClubMember Apply(IEventData evnt)
  {
    return evnt switch
    {
      UserJoinedClubEventV1 joinedClubEventV1 => Apply(joinedClubEventV1),
      ClubRoleAssignedEventV1 clubRoleAssignedEvent => Apply(clubRoleAssignedEvent),
      TankRoleAssignedEventV1 tankRoleAssignedEvent => Apply(tankRoleAssignedEvent),
      _ => throw new InvalidOperationException("Unknown event for club member"),
    };
  }

  private ClubMember Apply(UserJoinedClubEventV1 evnt)
    => this with
    {
      ClubId = evnt.ClubId,
      Id = evnt.UserId,
      Email = evnt.Email,
      FirstName = evnt.FirstName,
      LastName = evnt.LastName,
    };

  private ClubMember Apply(ClubRoleAssignedEventV1 evnt)
    => this with
    {
      RoleIds = [.. RoleIds.Append(evnt.RoleId)]
    };

  private ClubMember Apply(TankRoleAssignedEventV1 evnt)
  {
    var updatedAssignments = new Dictionary<Guid, IEnumerable<string>>(TankRoleAssignments);

    if (updatedAssignments.TryGetValue(evnt.TankId, out var roles))
    {
      updatedAssignments[evnt.TankId] = roles.Append(evnt.RoleId);
    }
    else
    {
      updatedAssignments[evnt.TankId] = [evnt.RoleId];
    }

    return this with { TankRoleAssignments = updatedAssignments };
  }
}
