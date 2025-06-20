using Core.Users;

namespace Core.Shared;

public interface IUserAccessor
{
  User User { get; }
}
