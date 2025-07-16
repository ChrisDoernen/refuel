using EventSourcing;

namespace Core.Infrastructure.Cqrs;

public class Replay<T> where T : IReplayable<T>, new()
{
  private readonly AuditTrail<T> _auditTrail = [];

  private Replay() { }

  public static Replay<T> New() => new();

  public static Replay<T> FromSnapshot(T snapshot)
  {
    // ToDo: Implement logic to create an AuditTrail from a snapshot
    throw new NotImplementedException();
  }

  public async Task<Replay<T>> ApplyEventStream(IAsyncEnumerable<Event> events)
  {
    await foreach (var evnt in events)
    {
      ApplyEvent(evnt);
    }

    return this;
  }

  public Replay<T> ApplyEvent(Event evnt)
  {
    var newState = _auditTrail.CurrentState.Apply(evnt.Data);
    var stateChange = new StateChange<T>(evnt, newState);

    _auditTrail.Append(stateChange);

    return this;
  }

  public AuditTrail<T> GetAuditTrail() => _auditTrail;
}
