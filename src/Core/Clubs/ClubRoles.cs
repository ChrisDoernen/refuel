using Core.Shared;

namespace Core.Clubs;

public class ClubRoles : IRoleDefinition
{
  public static Role Default = new(
    "club.default",
    "Default",
    "Club",
    "Default role with basic read permissions."
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
