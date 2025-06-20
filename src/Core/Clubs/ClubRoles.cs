using Core.Shared;

namespace Core.Clubs;

public static class ClubRoles
{
  public static Role Default = new("club.default");
  public static Role Admin = new("club.admin");
}

public record ClubRole(
  Guid ClubId,
  Role Role
);
