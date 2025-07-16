using Core.Infrastructure;

namespace Core.Users;

public class UserRoles : IRoleDefinition
{
  public static Role GlobalAdmin = new(
    "global.admin",
    "Admin",
    "Global",
    "Global administrator, rules the world."
  );
}
