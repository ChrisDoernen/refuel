using MediatR;

namespace Core.Shared;

public record GetRoleQuery(
  string Id
) : IRequest<Role>;

public class GetRoleQueryHandler(
  IMediator mediator
) : IRequestHandler<GetRoleQuery, Role>
{
  public async Task<Role> Handle(
    GetRoleQuery query,
    CancellationToken cancellationToken
  )
  {
    var roles = await mediator.Send(new GetRolesQuery(), cancellationToken);

    return roles.FirstOrDefault(r => r.Id.Equals(query.Id))
           ?? throw new KeyNotFoundException($"Role with id '{query.Id}' not found.");
  }
}
