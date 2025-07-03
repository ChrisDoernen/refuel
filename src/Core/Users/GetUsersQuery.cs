using MediatR;
using MongoDB;

namespace Core.Users;

public record GetUsersQuery : IRequest<IEnumerable<User>>;

public class GetUsersQueryHandler(
  IDocumentStore<User> userStore
) : IRequestHandler<GetUsersQuery, IEnumerable<User>>
{
  public async Task<IEnumerable<User>> Handle(
    GetUsersQuery query,
    CancellationToken cancellationToken
  )
  {
    return await userStore.GetAll(cancellationToken);
  }
}
