using Core.Infrastructure;
using Core.Infrastructure.Authorization;
using Core.Infrastructure.Cqrs;
using Core.ClubMembership;
using MediatR;

namespace Core.Clubs;

public record ClubMemberHasRolePolicy(
  Guid ClubId,
  Role Role
) : IAuthorizationPolicy;

public class ClubMemberHasRolePolicyHandler(
  IMediator mediator,
  IUserAccessor userAccessor
) : IAuthorizationHandler<ClubMemberHasRolePolicy>
{
  public async Task<AuthorizationResult> Handle(
    ClubMemberHasRolePolicy policy,
    CancellationToken cancellationToken
  )
  {
    var userInfo = userAccessor.UserInfo;
    var member = await mediator.Send(new GetClubMemberAuditTrailQuery(policy.ClubId, userInfo.Id), cancellationToken);
    var isUserInRole = member.CurrentState.RoleIds.Any(r => r.Equals(policy.Role.Id));

    var result = isUserInRole
      ? AuthorizationResult.Succeed()
      : AuthorizationResult.Fail($"{userAccessor.UserInfo} is not in role {policy.Role}, club id {policy.ClubId}.");

    return await Task.FromResult(result);
  }
}
