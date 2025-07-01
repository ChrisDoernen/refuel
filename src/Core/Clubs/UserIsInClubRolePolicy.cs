using Core.Shared;
using Core.Shared.Authorization;

namespace Core.Clubs;

public record UserIsInClubRolePolicy(
  Guid ClubId,
  Role Role
) : IAuthorizationPolicy;

public class UserIsInClubRolePolicyHandler(
  IUserAccessor userAccessor
) : IAuthorizationHandler<UserIsInClubRolePolicy>
{
  public async Task<AuthorizationResult> Handle(
    UserIsInClubRolePolicy policy,
    CancellationToken cancellationToken
  )
  {
    var clubRole = new ClubRole(policy.ClubId, policy.Role.Id);
    var isUserInRole = userAccessor.User.ClubRoles.Any(r => r.Equals(clubRole));

    var result = isUserInRole
      ? AuthorizationResult.Succeed()
      : AuthorizationResult.Fail($"{userAccessor.User} is not in role {policy.Role}, club id {policy.ClubId}.");

    return await Task.FromResult(result);
  }
}
