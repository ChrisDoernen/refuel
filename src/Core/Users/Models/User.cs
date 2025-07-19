using MongoDB;

namespace Core.Users.Models;

public class User(
  string email,
  string firstName,
  string lastName
) : Document
{
  public string Email { get; private init; } = email;
  public string FirstName { get; private init; } = firstName;
  public string LastName { get; private init; } = lastName;
  public DateTime SignedUpAtUtc { get; private init; } = DateTime.UtcNow;
}
