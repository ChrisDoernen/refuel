using Core.ClubMembership.AssignClubRole;
using Core.ClubMembership.JoiningClubs;
using Core.Shared;
using EventSourcingDB;

namespace Core.ClubMembership;

public record ClubMember : Audited<ClubMember>, IReplayable<ClubMember>
{
  public Guid ClubId { get; private init; }
  public Guid Id { get; private init; }
  public string Email { get; private init; } = null!;
  public string FirstName { get; private init; } = null!;
  public string LastName { get; private init; } = null!;
  public IEnumerable<string> RoleIds { get; private init; } = [];

  public ClubMember Apply(IEventData evnt)
  {
    return evnt switch
    {
      UserJoinedClubEventV1 userSignedUpEvent => Apply(userSignedUpEvent),
      ClubRoleAssignedEventV1 clubRoleAssignedEvent => Apply(clubRoleAssignedEvent),
      _ => throw new InvalidOperationException("Unknown event for club member"),
    };
  }

  private ClubMember Apply(UserJoinedClubEventV1 evnt)
    => this with
    {
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
}
