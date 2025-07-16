using Core.Infrastructure;
using Core.Infrastructure.Roles;

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
