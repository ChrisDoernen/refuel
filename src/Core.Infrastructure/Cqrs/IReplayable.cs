using EventSourcing;

namespace Core.Infrastructure.Cqrs;

public interface IReplayable<T>
{
  T Apply(IEventData evnt);
}
