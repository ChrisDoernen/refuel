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
  Func<Subject, Guid> idSelector
) : IReadModelSynchronizationService where T : IReplayable<T>, new()
{
  public async Task Replay(Event evnt, CancellationToken cancellationToken)
  {
    if (!relevantEventTypes.Contains(evnt.Data.GetType()))
    {
      return;
    }

    var id = idSelector(evnt.Subject);

    var maybeStateChange = await repository.MaybeGetById(id, cancellationToken);

    var stateChange = maybeStateChange
      .Map(change => change.Apply(evnt))
      .Reduce(() => new T().GetInitialChange(evnt));

    await repository.Upsert(id, stateChange, cancellationToken);
  }
}
