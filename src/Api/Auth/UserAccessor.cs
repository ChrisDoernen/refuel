using Core.Shared;
using Core.Users;

namespace Api.Auth;

public class UserAccessor : IUserAccessor
{
  private readonly IHttpContextAccessor _httpContextAccessor;
  private readonly Lazy<User> _user;
  public User User => _user.Value;

  public UserAccessor(IHttpContextAccessor httpContextAccessor)
  {
    _httpContextAccessor = httpContextAccessor;
    _user = new Lazy<User>(GetMdsUser);
  }

  private User GetMdsUser()
  {
    if (_httpContextAccessor.HttpContext is null)
    {
      // ToDo
    }

    var isMdsUserFound = _httpContextAccessor.HttpContext!.Items.TryGetValue(nameof(User), out var user);
    if (!isMdsUserFound || user is null)
    {
      throw new Exception($"User not found, set up {nameof(AuthenticationMiddleware)} properly");
    }

    return (User)user;
  }
}
