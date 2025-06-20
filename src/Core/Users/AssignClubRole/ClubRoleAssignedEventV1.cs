using Core.Clubs;
using EventSourcingDB;

namespace Core.Users.AssignClubRole;

[EventType("com.example.club-role-assigned.v1")]
public record ClubRoleAssignedEventV1(
  Guid UserId,
  ClubRole Role
) : IEventData;
