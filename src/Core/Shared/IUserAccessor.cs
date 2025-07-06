using Core.Shared.Authorization;

namespace Core.Shared;

public interface IUserAccessor
{
  UserInfo UserInfo { get; }
}
