using Core.Shared;

namespace Core.Clubs;

public class ClubRoles : IModuleRoles
{
  public static Role Default = new(
    "club.default",
    "Default",
    "Club",
    "Default club role with basic read permissions."
  );

  public static Role Admin = new(
    "club.admin",
    "Admin",
    "Club",
    "Club administrator with full read and write permissions."
  );
}

public record ClubRole(
  Guid ClubId,
  string RoleId
);
