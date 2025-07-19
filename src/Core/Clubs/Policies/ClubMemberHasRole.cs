using Core.ClubMembership.Models;
using Core.Infrastructure.Authorization;
using Core.Infrastructure.Cqrs;
using Core.Infrastructure.Roles;
using EventSourcing;

namespace Core.Clubs.Policies;

public record ClubMemberHasRolePolicy(
  Guid ClubId,
  Role Role
) : IAuthorizationPolicy;

public class ClubMemberHasRolePolicyHandler(
  IReplayService<ClubMemberSecurity> replayService,
  IUserAccessor userAccessor
) : IAuthorizationHandler<ClubMemberHasRolePolicy>
{
  public async Task<AuthorizationResult> Handle(
    ClubMemberHasRolePolicy policy,
    CancellationToken cancellationToken
  )
  {
    var userInfo = userAccessor.UserInfo;

    var member = await replayService.GetAuditTrail(
      policy.ClubId,
      new Subject($"/members/{userInfo.Id}/security"),
      cancellationToken
    );

    var isUserInRole = member.CurrentState.RoleIds.Any(r => r.Equals(policy.Role.Id));

    var result = isUserInRole
      ? AuthorizationResult.Succeed()
      : AuthorizationResult.Fail($"{userAccessor.UserInfo} is not in role {policy.Role}, club id {policy.ClubId}.");

    return await Task.FromResult(result);
  }
}
