using MediatR;
using MongoDB;

namespace Core.Users;

public record GetUserByEmailQuery(
  string Email
) : IRequest<User>;

public class GetUserByEmailQueryHandler(
  IDocumentStore<User> userStore
) : IRequestHandler<GetUserByEmailQuery, User>
{
  public async Task<User> Handle(
    GetUserByEmailQuery query,
    CancellationToken cancellationToken
  )
  {
    return await userStore.GetSingle(u => u.Email.Equals(query.Email), cancellationToken);
  }
}
