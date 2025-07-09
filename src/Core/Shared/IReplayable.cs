using EventSourcing;

namespace Core.Shared;

public interface IReplayable<T>
{
  T Apply(IEventData evnt);
}
