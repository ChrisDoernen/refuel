using System.Security.Claims;
using Core.Shared;
using Core.Shared.Authorization;
using Core.Users;
using MediatR;

namespace Api.Auth;

public class AuthenticationMiddleware(
  IMediator mediator,
  ILogger<AuthenticationMiddleware> logger
) : IMiddleware
{
  public async Task InvokeAsync(HttpContext context, RequestDelegate next)
  {
    var identity = context.User.Identity;
    if (identity is null || !identity.IsAuthenticated)
    {
      logger.LogWarning($"Unauthenticated request on path {context.Request.Path}");
      
      await next(context);
    }

    try
    {
      var userInfo = await GetUser(context.User);
      context.Items.Add(nameof(UserInfo), userInfo);
    }
    catch (Exception ex)
    {
      logger.LogWarning($"Error parsing user: {ex}");
    }

    await next(context);
  }

  private static readonly IEnumerable<Role> GlobalRoles = [UserRoles.Admin];

  private async Task<UserInfo> GetUser(ClaimsPrincipal principal)
  {
    var email = principal.FindFirst(ClaimTypes.Email)?.Value;
    if (email is null)
    {
      throw new Exception("Missing email claim in JWT token");
    }

    var roles = principal.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
    var globalRoles = GlobalRoles.Where(r => roles.Contains(r.Id));

    var user = await mediator.Send(new GetUserByEmailQuery(email));

    return new UserInfo(user, globalRoles);
  }
}
