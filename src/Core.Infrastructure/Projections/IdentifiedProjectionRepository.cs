using System.Linq.Expressions;
using Core.Infrastructure.Caching;
using Core.Infrastructure.Cqrs;
using Microsoft.Extensions.Caching.Hybrid;

namespace Core.Infrastructure.Projections;

public interface IIdentifiedProjectionRepository<T> where T : IIdentifiedProjection, IReplayable<T>, new()
{
  Task<Maybe<ProjectionChange<T>>> MaybeGetById(
    Guid id,
    CancellationToken cancellationToken = default
  );

  Task<ProjectionChange<T>> GetById(
    Guid id,
    CancellationToken cancellationToken = default
  );

  Task<IEnumerable<ProjectionChange<T>>> Filter(
    Expression<Func<T, bool>> filter,
    CancellationToken cancellationToken = default
  );

  Task<IEnumerable<ProjectionChange<T>>> GetManyById(
    IEnumerable<Guid> ids,
    CancellationToken cancellationToken = default
  );

  Task Upsert(
    ProjectionChange<T> change,
    CancellationToken cancellationToken = default
  );
}

public class IdentifiedProjectionRepository<T>(
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
        .ForValue(s.ReadModels.GetValueOrDefault(id))
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
        .ForValue(s.ReadModels.GetValueOrDefault(id))
        .Map(m => new ProjectionChange<T>(s.Metadata, m))
        .ReduceThrow(new KeyNotFoundException("Read model not found"))
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
      .Map(s => s.ReadModels
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
      .Map(s => s.ReadModels
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
          ReadModels = new Dictionary<Guid, T>(s.ReadModels)
          {
            [change.State.Id] = change.State
          }
        }
      )
      .Reduce(
        new IdentifiedProjectionState<T>
        {
          Metadata = change.EventMetadata,
          ReadModels = new Dictionary<Guid, T>
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
