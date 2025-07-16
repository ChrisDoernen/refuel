using Core.Infrastructure.Cqrs;
using EventSourcing;

namespace Core.Infrastructure.ReadModels;

public interface IReadModelSynchronizationService
{
  Task Replay(Event evnt, CancellationToken cancellationToken);
}

/// <summary>
///   Synchronizes the read model with events, usually replayed from the event store.
/// </summary>
public class ReadModelSynchronizationService<T>(
  IReadModelRepository<T> repository,
  IEnumerable<Type> relevantEventTypes,
  Func<Subject, Guid> getIdFromSubject
) : IReadModelSynchronizationService where T : IReplayable<T>, new()
{
  public async Task Replay(Event evnt, CancellationToken cancellationToken)
  {
    // Discard events not relevant for this read model
    // ToDo: Automated way to determine interesting events with reflection to reduce manual errors?
    if (!relevantEventTypes.Contains(evnt.GetType()))
    {
      return;
    }

    // Most of the time the subject is "/something/{id}"
    var id = getIdFromSubject(evnt.Subject);

    var maybeStateChange = await repository.MaybeGetById(id, cancellationToken);

    var stateChange = maybeStateChange
      .Map(change => change.Apply(evnt))
      .Reduce(() => new T().GetInitialChange(evnt));

    await repository.Upsert(stateChange, cancellationToken);
  }
}
