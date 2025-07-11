using EventSourcing;

namespace App.Cqrs;

public interface IReplayable<T>
{
  T Apply(IEventData evnt);
}
