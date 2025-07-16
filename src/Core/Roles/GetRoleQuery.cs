using Core.Infrastructure.Roles;
using MediatR;

namespace Core.Roles;

public record GetRoleQuery(
  string Id
) : IRequest<Role>;

public class GetRoleQueryHandler(
  IRoleProvider roleProvider
) : IRequestHandler<GetRoleQuery, Role>
{
  public async Task<Role> Handle(
    GetRoleQuery query,
    CancellationToken cancellationToken
  )
  {
    return await Task.FromResult(roleProvider.GetRole(query.Id));
  }
}
