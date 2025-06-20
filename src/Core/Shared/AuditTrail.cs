using System.Collections;
using EventSourcingDB;

namespace Core.Shared;

public class AuditTrail<T> : IReadOnlyCollection<StateChange<T>> where T : IReplayable<T>, new()
{
  private readonly List<StateChange<T>> _auditTrail = [];
  private StateChange<T>? Current => _auditTrail.LastOrDefault();
  public T CurrentState => Current is null ? new T() : _auditTrail.Last().State;
  public Change? LastChange => Current?.Change;

  private AuditTrail() { }

  public static AuditTrail<T> Pristine() => new();

  public static AuditTrail<T> FromSnapshot(T snapshot)
  {
    // ToDo: Implement logic to create an AuditTrail from a snapshot
    throw new NotImplementedException();
  }

  public async Task<AuditTrail<T>> Replay(IAsyncEnumerable<Change> changes)
  {
    await foreach (var change in changes)
    {
      var newState = CurrentState.Apply(change.Data);
      var stateChange = new StateChange<T>(change, newState);

      _auditTrail.Add(stateChange);
    }

    return this;
  }

  public void EnsureNotPristine()
  {
    if (_auditTrail.Count == 0)
    {
      throw new InvalidOperationException("Audit trail has no changes.");
    }
  }

  public IEnumerator<StateChange<T>> GetEnumerator() => _auditTrail.GetEnumerator();
  IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
  public int Count => _auditTrail.Count;
}

public record StateChange<T>(
  Change Change,
  T State
);

public record Change(
  string Id,
  string Subject,
  DateTime Time,
  IEventData Data
)
{
  public static Change FromEvent(Event evnt) =>
    new(
      evnt.Id,
      evnt.Subject,
      evnt.Time,
      evnt.Data
    );
}
