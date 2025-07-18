using Core.Infrastructure.Cqrs;
using EventSourcing;

namespace Core.Infrastructure.ReadModels;

/// <summary>
///   Contains a state together with the event that led to this state.
///   To be used on the read side, the event does not contain the whole event data.
/// </summary>
public record ReadModelChange<T>(
  EventMetadata EventMetadata,
  T State
) where T : IReplayable<T>, new();

public static class ReadModelChangeExtensions
{
  public static ReadModelChange<T> Apply<T>(
    this ReadModelChange<T> stateChange,
    Event evnt
  ) where T : IReplayable<T>, new()
  {
    if (evnt.Id <= stateChange.EventMetadata.Id)
    {
      return stateChange;
    }

    var newState = stateChange.State.Apply(evnt.Data);

    return new ReadModelChange<T>(evnt, newState);
  }
}
