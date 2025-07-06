using Core.ClubMembership;
using Core.Shared;
using Core.Shared.Authorization;
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
    var user = userAccessor.User;
    var member = await mediator.Send(new GetClubMemberQuery(policy.ClubId, user.Id), cancellationToken);
    var isUserInRole = member.RoleIds.Any(r => r.Equals(policy.Role.Id));

    var result = isUserInRole
      ? AuthorizationResult.Succeed()
      : AuthorizationResult.Fail($"{userAccessor.User} is not in role {policy.Role}, club id {policy.ClubId}.");

    return await Task.FromResult(result);
  }
}
