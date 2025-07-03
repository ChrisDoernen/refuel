using MediatR;
using MongoDB;

namespace Core.Users;

public record GetUserQuery(
  Guid UserId
) : IRequest<User>;

public class GetUserQueryHandler(
  IDocumentStore<User> userStore
) : IRequestHandler<GetUserQuery, User>
{
  public async Task<User> Handle(
    GetUserQuery query,
    CancellationToken cancellationToken
  )
  {
    return await userStore.GetById(query.UserId, cancellationToken);
  }
}
