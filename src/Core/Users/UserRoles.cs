using Core.Shared;

namespace Core.Users;

public class UserRoles : IRoleDefinition
{
  public static Role Admin = new(
    "global.admin",
    "Admin",
    "Global",
    "Global administrator, rules the world."
  );
}
