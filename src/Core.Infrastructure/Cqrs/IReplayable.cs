using EventSourcing;

namespace Core.Infrastructure.Cqrs;

public interface IReplayable<T>
{
  T Apply(IEventData evnt);
}

public static class ReplayableExtensions
{
  public static StateChange<T> GetInitialChange<T>(
    this IReplayable<T> replayable,
    Event evnt
  ) where T : IReplayable<T>, new()
  {
    return new StateChange<T>(
      evnt,
      replayable.Apply(evnt.Data)
    );
  }
}
