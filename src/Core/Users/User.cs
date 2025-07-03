using MongoDB;

namespace Core.Users;

public record User : IDocument
{
  public string Email { get; private init; }
  public string FirstName { get; private init; }
  public string LastName { get; private init; }
  public DateTime SignedUpAtUtc { get; private init; }

  public User(
    string email,
    string firstName,
    string lastName
  )
  {
    Email = email;
    FirstName = firstName;
    LastName = lastName;
    SignedUpAtUtc = DateTime.UtcNow;
  }
}
