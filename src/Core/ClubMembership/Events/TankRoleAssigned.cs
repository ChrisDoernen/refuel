using EventSourcing;

namespace Core.ClubMembership.Events;

[EventType("com.example.tank-role-assigned.v1")]
public record TankRoleAssignedEventV1(
  Guid MemberId,
  Guid TankId,
  string RoleId
) : IEventData;
