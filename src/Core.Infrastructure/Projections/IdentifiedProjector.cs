using Core.Infrastructure.Cqrs;
using EventSourcing;

namespace Core.Infrastructure.Projections;

public interface IProjector
{
  Task Project(Event evnt, CancellationToken cancellationToken);
}

/// <summary>
///   Projects events to identified projections, usually replayed from the event store.
/// </summary>
public class IdentifiedProjector<T>(
  IIdentifiedProjectionRepository<T> repository,
  IEnumerable<Type> relevantEventTypes,
  Func<Subject, Guid> idSelector
) : IProjector where T : IReplayable<T>, IIdentifiedProjection, new()
{
  public async Task Project(Event evnt, CancellationToken cancellationToken)
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
