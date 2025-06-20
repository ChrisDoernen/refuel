using Core.Shared;
using Core.Shared.Authorization;

namespace Core.Clubs;

public record UserIsInClubRolePolicy(
  ClubRole Role
) : IAuthorizationPolicy;

public class UserIsInRolePolicyHandler(
  IUserAccessor userAccessor
) : IAuthorizationHandler<UserIsInClubRolePolicy>
{
  public async Task<AuthorizationResult> Handle(
    UserIsInClubRolePolicy policy,
    CancellationToken cancellationToken
  )
  {
    var isUserInRole = userAccessor.User.ClubRoles.Any(r => r.Equals(policy.Role));

    return
      isUserInRole
        ? AuthorizationResult.Succeed()
        : AuthorizationResult.Fail($"User {userAccessor.User} is not in role {policy.Role}");
  }
}
