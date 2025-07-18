using Core.Infrastructure.Cqrs;
using EventSourcing;

namespace Core.Infrastructure.ReadModels;

public interface IReadModelSynchronizationService
{
  Task Replay(Event evnt, CancellationToken cancellationToken);
}

/// <summary>
///   Synchronizes the read models with events, usually replayed from the event store.
/// </summary>
public class IdentifiedReadModelSynchronizationService<T>(
  IIdentifiedReadModelRepository<T> repository,
  IEnumerable<Type> relevantEventTypes,
  Func<Subject, Guid> idSelector
) : IReadModelSynchronizationService where T : IReplayable<T>, IIdentifiedReadModel, new()
{
  public async Task Replay(Event evnt, CancellationToken cancellationToken)
  {
    if (!relevantEventTypes.Contains(evnt.Data.GetType()))
    {
      return;
    }

    var id = idSelector(evnt.Subject);

    var maybeLastChange = await repository.MaybeGetById(id, cancellationToken);

    var newChange = maybeLastChange
      .Map(change => change.Apply(evnt))
      .Reduce(() => new T().GetInitialReadModelChange(evnt));

    await repository.Upsert(newChange, cancellationToken);
  }
}
