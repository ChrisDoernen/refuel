using Core.Infrastructure.Authorization;
using Core.Infrastructure.Cqrs;
using Core.Infrastructure.Roles;

namespace Core.Users.Policies;

public record UserHasRolePolicy(
  Role Role
) : IAuthorizationPolicy;

public class UserHasRolePolicyHandler(
  IUserAccessor userAccessor
) : IAuthorizationHandler<UserHasRolePolicy>
{
  public async Task<AuthorizationResult> Handle(
    UserHasRolePolicy policy,
    CancellationToken cancellationToken
  )
  {
    var userInfo = userAccessor.UserInfo;
    var isUserInRole = userInfo.Roles.Any(r => r.Equals(policy.Role));

    var result = isUserInRole
      ? AuthorizationResult.Succeed()
      : AuthorizationResult.Fail($"{userInfo.Email} is not in role {policy.Role}.");

    return await Task.FromResult(result);
  }
}
