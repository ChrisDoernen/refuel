using Core.Shared;
using Core.Shared.Authorization;

namespace Core.Users;

public record UserIsInRolePolicy(
  Role Role
) : IAuthorizationPolicy;

public class UserIsInRolePolicyHandler(
  IUserAccessor userAccessor
) : IAuthorizationHandler<UserIsInRolePolicy>
{
  public async Task<AuthorizationResult> Handle(
    UserIsInRolePolicy policy,
    CancellationToken cancellationToken
  )
  {
    var userInfo = userAccessor.UserInfo;
    var isUserInRole = userInfo.Roles.Any(r => r.Equals(policy.Role));

    var result = isUserInRole
      ? AuthorizationResult.Succeed()
      : AuthorizationResult.Fail($"{userInfo.User.Email} is not in role {policy.Role}.");

    return await Task.FromResult(result);
  }
}
