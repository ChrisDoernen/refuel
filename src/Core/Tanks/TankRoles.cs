using Core.Shared;

namespace Core.Tanks;

public class TankRoles : IRoleDefinition
{
  public static Role Default = new(
    "tank.default",
    "Default",
    "Tank",
    "Default role with read permissions."
  );

  public static Role User = new(
    "tank.user",
    "User",
    "Tank",
    "Tank user, can extract fuel."
  );

  public static Role Responsible = new(
    "tank.responsible",
    "Responsible",
    "Tank",
    "Tank responsible, can initialize meters and refuel."
  );
}

public record ClubRole(
  Guid ClubId,
  string RoleId
);
