using System.Collections;
using System.Collections.Concurrent;
using EventSourcing;

namespace Core.Infrastructure.Cqrs;

public record AuditTrail<T> : IReadOnlyCollection<StateChange<T>> where T : IReplayable<T>, new()
{
  private readonly ConcurrentQueue<StateChange<T>> _auditTrail = [];

  public IEnumerator<StateChange<T>> GetEnumerator() => _auditTrail.GetEnumerator();

  IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

  public int Count => _auditTrail.Count;

  public Maybe<StateChange<T>> CurrentChange => Maybe<StateChange<T>>.ForValue(_auditTrail.LastOrDefault());

  public T CurrentState => CurrentChange.Map(change => change.State).Reduce(new T());

  public void Append(StateChange<T> stateChange)
  {
    var currentEventId = _auditTrail.LastOrDefault() is StateChange<T> change
      ? (long)change.ProcessedEvent.Id
      : -1;

    if (!(stateChange.ProcessedEvent.Id > currentEventId))
    {
      return;
    }

    _auditTrail.Enqueue(stateChange);
  }
}

public static class AuditTrailExtensions
{
  public static Precondition GetIsSubjectOnEventIdPrecondition<T>(
    this AuditTrail<T> auditTrail
  ) where T : IReplayable<T>, new()
  {
    auditTrail.EnsureNotPristine();
    var lastChange = auditTrail.Last().ProcessedEvent;

    return new IsSubjectOnEventIdPrecondition(
      lastChange.Subject,
      lastChange.Id
    );
  }

  public static void EnsureNotPristine<T>(
    this AuditTrail<T> auditTrail
  ) where T : IReplayable<T>, new()
  {
    if (auditTrail.LastOrDefault() is null)
    {
      throw new InvalidOperationException("Audit trail is empty.");
    }
  }
}
