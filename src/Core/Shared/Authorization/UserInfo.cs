using Core.Users;

namespace Core.Shared.Authorization;

public record UserInfo(
  User User,
  IEnumerable<Role> Roles
);
