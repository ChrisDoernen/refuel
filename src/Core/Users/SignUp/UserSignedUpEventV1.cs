using EventSourcingDB;

namespace Core.Users.SignUp;

[EventType("com.example.user-signed-up.v1")]
public record UserSignedUpEventV1(
  Guid UserId,
  string FirstName,
  string LastName,
  string Email
) : IEventData;
