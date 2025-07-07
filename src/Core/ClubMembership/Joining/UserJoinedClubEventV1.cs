using EventSourcingDB;

namespace Core.ClubMembership.Joining;

[EventType("com.example.user-joined-club.v1")]
public record UserJoinedClubEventV1(
  Guid ClubId,
  Guid UserId,
  string Email,
  string FirstName,
  string LastName
) : IEventData;
