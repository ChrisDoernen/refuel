using EventSourcingDB;

namespace Core.ClubMembership.AssignClubRole;

[EventType("com.example.club-role-assigned.v1")]
public record ClubRoleAssignedEventV1(
  Guid MemberId,
  string RoleId
) : IEventData;
