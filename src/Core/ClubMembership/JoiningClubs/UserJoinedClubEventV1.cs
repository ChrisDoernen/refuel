using EventSourcingDB;

namespace Core.ClubMembership.JoiningClubs;

[EventType("com.example.user-joined-club.v1")]
public record UserJoinedClubEventV1(
  Guid UserId,
  string Email,
  string FirstName,
  string LastName
) : IEventData;
