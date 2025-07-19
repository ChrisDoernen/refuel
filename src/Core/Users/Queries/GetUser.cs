using Core.Users.Models;
using MediatR;
using MongoDB;

namespace Core.Users.Queries;

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
