using Core.ClubMembership.Events;
using Core.Infrastructure.Cqrs;
using EventSourcing;

namespace Core.ClubMembership.Models;

public record ClubMember : IReplayable<ClubMember>
{
  public Guid Id { get; init; }
  public Guid ClubId { get; init; }
  public string Email { get; init; } = null!;
  public string FirstName { get; init; } = null!;
  public string LastName { get; init; } = null!;
  public IEnumerable<string> RoleIds { get; init; } = [];
  public Dictionary<Guid, IEnumerable<string>> TankRoleAssignments { get; init; } = new();

  public ClubMember Apply(IEventData evnt)
  {
    return evnt switch
    {
      UserJoinedClubEventV1 joinedClubEventV1 => Apply(joinedClubEventV1),
      ClubRoleAssignedEventV1 clubRoleAssignedEventV1 => Apply(clubRoleAssignedEventV1),
      TankRoleAssignedEventV1 tankRoleAssignedEventV1 => Apply(tankRoleAssignedEventV1),
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
