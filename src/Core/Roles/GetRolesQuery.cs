using Core.Infrastructure.Roles;
using MediatR;

namespace Core.Roles;

public record GetRolesQuery : IRequest<IEnumerable<Role>>;

public class GetRolesQueryHandler(
  IRoleProvider roleProvider
) : IRequestHandler<GetRolesQuery, IEnumerable<Role>>
{
  public async Task<IEnumerable<Role>> Handle(
    GetRolesQuery query,
    CancellationToken cancellationToken
  )
  {
    return await Task.FromResult(roleProvider.Roles);
  }
}
