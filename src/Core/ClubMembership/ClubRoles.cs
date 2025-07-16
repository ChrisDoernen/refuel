using Core.Infrastructure;

namespace Core.ClubMembership;

public class ClubRoles : IRoleDefinition
{
  public static Role Member = new(
    "club.member",
    "Member",
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
