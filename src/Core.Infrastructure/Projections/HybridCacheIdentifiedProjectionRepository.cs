using System.Linq.Expressions;
using Core.Infrastructure.Caching;
using Core.Infrastructure.Cqrs;
using Microsoft.Extensions.Caching.Hybrid;

namespace Core.Infrastructure.Projections;

public class HybridCacheIdentifiedProjectionRepository<T>(
  HybridCache cache,
  CacheKey key
) : IIdentifiedProjectionRepository<T> where T : IReplayable<T>, IIdentifiedProjection, new()
{
  public async Task<Maybe<ProjectionChange<T>>> MaybeGetById(
    Guid id,
    CancellationToken cancellationToken
  )
  {
    var maybeState = await cache.GetEntry<IdentifiedProjectionState<T>>(key, cancellationToken);

    return maybeState
      .Map(s => Maybe<T>
        .ForValue(s.Projections.GetValueOrDefault(id))
        .Map(m => new ProjectionChange<T>(s.Metadata, m))
      );
  }

  public async Task<ProjectionChange<T>> GetById(
    Guid id,
    CancellationToken cancellationToken
  )
  {
    var maybeState = await cache.GetEntry<IdentifiedProjectionState<T>>(key, cancellationToken);

    return maybeState
      .Map(s => Maybe<T>
        .ForValue(s.Projections.GetValueOrDefault(id))
        .Map(m => new ProjectionChange<T>(s.Metadata, m))
        .ReduceThrow(new KeyNotFoundException($"Projection with id {id} not found in {key.Key}"))
      ).ReduceThrow(new ProjectionInconsistencyException(key));
  }

  public async Task<IEnumerable<ProjectionChange<T>>> Filter(
    Expression<Func<T, bool>> filter,
    CancellationToken cancellationToken
  )
  {
    var maybeState = await cache.GetEntry<IdentifiedProjectionState<T>>(key, cancellationToken);
    var predicate = filter.Compile();

    return maybeState
      .Map(s => s.Projections
        .Where(m => predicate(m.Value))
        .Select(m => new ProjectionChange<T>(s.Metadata, m.Value))
      )
      .ReduceThrow(new ProjectionInconsistencyException(key));
  }

  public async Task<IEnumerable<ProjectionChange<T>>> GetManyById(
    IEnumerable<Guid> ids,
    CancellationToken cancellationToken
  )
  {
    var maybeState = await cache.GetEntry<IdentifiedProjectionState<T>>(key, cancellationToken);

    return maybeState
      .Map(s => s.Projections
        .Where(m => ids.Contains(m.Key))
        .Select(m => new ProjectionChange<T>(s.Metadata, m.Value))
      )
      .ReduceThrow(new ProjectionInconsistencyException(key));
  }

  public async Task Upsert(
    ProjectionChange<T> change,
    CancellationToken cancellationToken
  )
  {
    var maybeState = await cache.GetEntry<IdentifiedProjectionState<T>>(key, cancellationToken);

    var newState = maybeState
      .Map(s => new IdentifiedProjectionState<T>
        {
          Metadata = change.EventMetadata,
          Projections = new Dictionary<Guid, T>(s.Projections)
          {
            [change.State.Id] = change.State
          }
        }
      )
      .Reduce(
        new IdentifiedProjectionState<T>
        {
          Metadata = change.EventMetadata,
          Projections = new Dictionary<Guid, T>
          {
            [change.State.Id] = change.State
          }
        }
      );

    await cache.SetEntry(
      key,
      newState,
      cancellationToken: cancellationToken
    );
  }
}
