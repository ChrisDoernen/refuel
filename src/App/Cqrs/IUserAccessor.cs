using App.Authorization;

namespace App.Cqrs;

public interface IUserAccessor
{
  UserInfo UserInfo { get; }
}
