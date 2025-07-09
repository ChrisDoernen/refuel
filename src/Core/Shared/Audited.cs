using EventSourcingDb.Types;

namespace Core.Shared;

public record Audited<T> where T : IReplayable<T>, new()
{
  public IEnumerable<StateChange<T>> AuditTrail { get; init; } = [];
}

public static class AuditedExtensions
{
  public static Precondition GetIsSubjectOnEventIdPrecondition<T>(
    this Audited<T> audited
  ) where T : IReplayable<T>, new()
  {
    audited.EnsureNotPristine();
    var lastChange = audited.AuditTrail.Last().Change;

    return Precondition.IsSubjectOnEventIdPrecondition(lastChange.Subject, lastChange.Id);
  }

  public static void EnsureNotPristine<T>(
    this Audited<T> audited
  ) where T : IReplayable<T>, new()
  {
    if (audited.AuditTrail.LastOrDefault() is null)
    {
      throw new InvalidOperationException("Audit trail is empty.");
    }
  }
}
