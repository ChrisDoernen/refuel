using App;
using App.Authorization;
using App.Cqrs;
using Core.ClubMembership;
using Core.Shared;
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
    var member = await mediator.Send(new GetClubMemberQuery(policy.ClubId, userInfo.Id), cancellationToken);
    var isUserInRole = member.RoleIds.Any(r => r.Equals(policy.Role.Id));

    var result = isUserInRole
      ? AuthorizationResult.Succeed()
      : AuthorizationResult.Fail($"{userAccessor.UserInfo} is not in role {policy.Role}, club id {policy.ClubId}.");

    return await Task.FromResult(result);
  }
}
