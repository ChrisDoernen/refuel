using System.Linq.Expressions;
using Core.Infrastructure.Caching;
using Core.Infrastructure.Cqrs;
using Microsoft.Extensions.Caching.Hybrid;

namespace Core.Infrastructure.ReadModels;

public interface IIdentifiedReadModelRepository<T> where T : IIdentifiedReadModel, IReplayable<T>, new()
{
  Task<Maybe<ReadModelChange<T>>> MaybeGetById(
    Guid id,
    CancellationToken cancellationToken = default
  );

  Task<ReadModelChange<T>> GetById(
    Guid id,
    CancellationToken cancellationToken = default
  );

  Task<IEnumerable<ReadModelChange<T>>> Filter(
    Expression<Func<T, bool>> filter,
    CancellationToken cancellationToken = default
  );

  Task<IEnumerable<ReadModelChange<T>>> GetManyById(
    IEnumerable<Guid> ids,
    CancellationToken cancellationToken = default
  );

  Task Upsert(
    ReadModelChange<T> change,
    CancellationToken cancellationToken = default
  );
}

public class IdentifiedReadModelRepository<T>(
  HybridCache cache,
  CacheKey key
) : IIdentifiedReadModelRepository<T> where T : IReplayable<T>, IIdentifiedReadModel, new()
{
  public async Task<Maybe<ReadModelChange<T>>> MaybeGetById(
    Guid id,
    CancellationToken cancellationToken
  )
  {
    var maybeState = await cache.GetEntry<IdentifiedReadModelsState<T>>(key, cancellationToken);

    return maybeState
      .Map(s => Maybe<T>
        .ForValue(s.ReadModels.GetValueOrDefault(id))
        .Map(m => new ReadModelChange<T>(s.Metadata, m))
      );
  }

  public async Task<ReadModelChange<T>> GetById(
    Guid id,
    CancellationToken cancellationToken
  )
  {
    var maybeState = await cache.GetEntry<IdentifiedReadModelsState<T>>(key, cancellationToken);

    return maybeState
      .Map(s => Maybe<T>
        .ForValue(s.ReadModels.GetValueOrDefault(id))
        .Map(m => new ReadModelChange<T>(s.Metadata, m))
        .ReduceThrow(new KeyNotFoundException("Read model not found"))
      ).ReduceThrow(new ReadModelInconsistencyException(key));
  }

  public async Task<IEnumerable<ReadModelChange<T>>> Filter(
    Expression<Func<T, bool>> filter,
    CancellationToken cancellationToken
  )
  {
    var maybeState = await cache.GetEntry<IdentifiedReadModelsState<T>>(key, cancellationToken);
    var predicate = filter.Compile();

    return maybeState
      .Map(s => s.ReadModels
        .Where(m => predicate(m.Value))
        .Select(m => new ReadModelChange<T>(s.Metadata, m.Value))
      )
      .ReduceThrow(new ReadModelInconsistencyException(key));
  }

  public async Task<IEnumerable<ReadModelChange<T>>> GetManyById(
    IEnumerable<Guid> ids,
    CancellationToken cancellationToken
  )
  {
    var maybeState = await cache.GetEntry<IdentifiedReadModelsState<T>>(key, cancellationToken);

    return maybeState
      .Map(s => s.ReadModels
        .Where(m => ids.Contains(m.Key))
        .Select(m => new ReadModelChange<T>(s.Metadata, m.Value))
      )
      .ReduceThrow(new ReadModelInconsistencyException(key));
  }

  public async Task Upsert(
    ReadModelChange<T> change,
    CancellationToken cancellationToken
  )
  {
    var maybeState = await cache.GetEntry<IdentifiedReadModelsState<T>>(key, cancellationToken);

    var newState = maybeState
      .Map(s => new IdentifiedReadModelsState<T>
        {
          Metadata = change.EventMetadata,
          ReadModels = new Dictionary<Guid, T>(s.ReadModels)
          {
            [change.State.Id] = change.State
          }
        }
      )
      .Reduce(
        new IdentifiedReadModelsState<T>
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
