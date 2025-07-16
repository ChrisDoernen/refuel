using Core.Infrastructure.Authorization;

namespace Core.Infrastructure.Cqrs;

public interface IUserAccessor
{
  UserInfo UserInfo { get; }
}
