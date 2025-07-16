using App.Cqrs;
using EventSourcing;

namespace Core.ClubMembership.AssigningClubRoles;

[EventType("com.example.club-role-assigned.v1")]
public record ClubRoleAssignedEventV1(
  Guid MemberId,
  string RoleId
) : IEventData;
