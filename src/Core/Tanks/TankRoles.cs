using App;

namespace Core.Tanks;

public class TankRoles : IRoleDefinition
{
  public static Role User = new(
    "tank.user",
    "User",
    "Tank",
    "Tank users can extract fuel."
  );

  public static Role Responsible = new(
    "tank.responsible",
    "Responsible",
    "Tank",
    "Tank responsibles can initialize meters and refuel tanks."
  );
}
