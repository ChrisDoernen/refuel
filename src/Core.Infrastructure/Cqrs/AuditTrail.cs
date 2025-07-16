using System.Collections;
using System.Globalization;
using EventSourcingDb.Types;

namespace Core.Infrastructure.Cqrs;

public record AuditTrail<T> : IReadOnlyCollection<StateChange<T>> where T : IReplayable<T>, new()
{
  private readonly List<StateChange<T>> _auditTrail = [];

  public IEnumerator<StateChange<T>> GetEnumerator() => _auditTrail.GetEnumerator();

  IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

  public int Count => _auditTrail.Count;
  private StateChange<T>? Current => _auditTrail.LastOrDefault();
  public T CurrentState => Current is null ? new T() : _auditTrail.Last().State;

  public void Append(StateChange<T> stateChange)
  {
    _auditTrail.Add(stateChange);
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

    return Precondition.IsSubjectOnEventIdPrecondition(
      lastChange.Subject.ToString(),
      lastChange.Id.ToString(CultureInfo.InvariantCulture)
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
