using Core.Infrastructure.Cqrs;
using EventSourcing;
using MongoDB;

namespace Core.Infrastructure.ReadModels;

public class PersistableStateChange<T>(
  Guid id,
  StateChange<T> stateChange
) : IDocument where T : IReplayable<T>, new()
{
  public Guid Id { get; set; } = id;
  public T State { get; set; } = stateChange.State;
  public Event Event { get; set; } = stateChange.ProcessedEvent;
}

public static class PersistableStateChangeExtensions
{
  public static StateChange<T> ToStateChange<T>(
    this PersistableStateChange<T> persistable
  ) where T : IReplayable<T>, new()
  {
    return new StateChange<T>(persistable.Event, persistable.State);
  }
}
