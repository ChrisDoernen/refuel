using System.Security.Claims;
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

      throw new UnauthorizedAccessException();
    }

    try
    {
      var user = await GetUser(context.User);
      context.Items.Add(nameof(User), user);
    }
    catch (Exception ex)
    {
      logger.LogWarning($"Error parsing user: {ex}");

      throw;
    }

    await next(context);
  }

  private async Task<User> GetUser(ClaimsPrincipal principal)
  {
    var email = principal.FindFirst(ClaimTypes.Email)?.Value;
    if (email is null)
    {
      throw new Exception("Missing email claim in JWT token");
    }

    return await mediator.Send(new GetUserByEmailQuery(email));
  }
}
