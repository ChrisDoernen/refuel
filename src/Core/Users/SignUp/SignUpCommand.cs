using MediatR;
using MongoDB;

namespace Core.Users.SignUp;

public record SignUpCommand(
  string Email,
  string FirstName,
  string LastName
) : IRequest<Guid>;

public class SignUpCommandHandler(
  IDocumentStore<User> userStore
) : IRequestHandler<SignUpCommand, Guid>
{
  public async Task<Guid> Handle(
    SignUpCommand command,
    CancellationToken cancellationToken
  )
  {
    var user = new User(
      command.Email,
      command.FirstName,
      command.LastName
    );
    await userStore.CreateOne(user, cancellationToken);

    return user.Id;
  }
}
