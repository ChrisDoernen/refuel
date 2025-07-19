using Core.Infrastructure.Authorization;
using Core.Users.Models;
using Core.Users.Policies;
using MediatR;
using MongoDB;

namespace Core.Users.Queries;

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

public class GetUsersQueryAuthorizer : Authorizer<GetUsersQuery>
{
  public override async Task BuildPolicy(GetUsersQuery _)
  {
    UsePolicy(new UserHasRolePolicy(UserRoles.GlobalAdmin));
  }
}
