using System.Reflection;
using MediatR;

namespace Core.Shared;

public record GetRolesQuery : IRequest<IEnumerable<Role>>;

public class GetRolesQueryHandler : IRequestHandler<GetRolesQuery, IEnumerable<Role>>
{
  public async Task<IEnumerable<Role>> Handle(
    GetRolesQuery query,
    CancellationToken cancellationToken
  )
  {
    var roles = Assembly.GetExecutingAssembly()
      .GetTypes()
      .Where(t => typeof(IModuleRoles).IsAssignableFrom(t) && t.IsClass)
      .SelectMany(
        type => type.GetFields(BindingFlags.Public | BindingFlags.Static)
          .Where(p => p.FieldType == typeof(Role))
          .Select(p => p.GetValue(null) as Role)
      )
      .OfType<Role>();

    return await Task.FromResult(roles);
  }
}
