using Core.Clubs;
using Core.Shared;
using Core.Users.AssignClubRole;
using Core.Users.SignUp;
using EventSourcingDB;

namespace Core.Users;

public record User : Audited<User>, IReplayable<User>
{
  public Guid Id { get; private init; }
  public string Email { get; private init; } = null!;
  public string FirstName { get; private init; } = null!;
  public string LastName { get; private init; } = null!;
  public IEnumerable<ClubRole> ClubRoles { get; private init; } = [];

  public User Apply(IEventData evnt)
  {
    return evnt switch
    {
      UserSignedUpEventV1 userSignedUpEvent => Apply(userSignedUpEvent),
      ClubRoleAssignedEventV1 clubRoleAssignedEvent => Apply(clubRoleAssignedEvent),
      _ => throw new InvalidOperationException("Unknown event for user"),
    };
  }

  private User Apply(UserSignedUpEventV1 evnt)
    => this with
    {
      Id = evnt.UserId,
      Email = evnt.Email,
      FirstName = evnt.FirstName,
      LastName = evnt.LastName,
    };

  private User Apply(ClubRoleAssignedEventV1 evnt)
    => this with
    {
      ClubRoles = [.. ClubRoles.Append(evnt.Role)]
    };
}
