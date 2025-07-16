using Core.Infrastructure.Authorization;
using Core.Infrastructure.Cqrs;

namespace Api.Auth;

public class UserAccessor : IUserAccessor
{
  private readonly IHttpContextAccessor _httpContextAccessor;
  private readonly Lazy<UserInfo> _user;
  public UserInfo UserInfo => _user.Value;

  public UserAccessor(IHttpContextAccessor httpContextAccessor)
  {
    _httpContextAccessor = httpContextAccessor;
    _user = new Lazy<UserInfo>(GetUserInfo);
  }

  private UserInfo GetUserInfo()
  {
    if (_httpContextAccessor.HttpContext is null)
    {
      // ToDo
    }

    var isUserFound = _httpContextAccessor.HttpContext!.Items.TryGetValue(nameof(UserInfo), out var user);
    if (!isUserFound || user is null)
    {
      throw new Exception($"User not found, set up {nameof(AuthenticationMiddleware)} properly");
    }

    return (UserInfo)user;
  }
}
