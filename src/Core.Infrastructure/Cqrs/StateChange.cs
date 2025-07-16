using EventSourcing;

namespace Core.Infrastructure.Cqrs;

/// <summary>
///   Contains a state together with the event that led to this state.
/// </summary>
public record StateChange<T>(
  Event ProcessedEvent,
  T State
) where T : IReplayable<T>, new();

public static class StateChangeExtensions
{
  public static StateChange<T> Apply<T>(
    this StateChange<T> stateChange,
    Event evnt
  ) where T : IReplayable<T>, new()
  {
    if (!(evnt.Id > stateChange.ProcessedEvent.Id))
    {
      return stateChange;
    }

    var newState = stateChange.State.Apply(evnt.Data);

    return new StateChange<T>(evnt, newState);
  }
}
