namespace Core.Infrastructure.Authorization;

public record UserInfo(
  Guid Id,
  string Email,
  string FullName,
  IEnumerable<Role> Roles
);
