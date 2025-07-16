using System.Linq.Expressions;
using Core.Infrastructure.Cqrs;
using MongoDB;

namespace Core.Infrastructure.ReadModels;

public interface IReadModelRepository<T> where T : IReplayable<T>, new()
{
  Task<Maybe<StateChange<T>>> MaybeGetById(Guid id, CancellationToken cancellationToken);
  Task<StateChange<T>> GetById(Guid id, CancellationToken cancellationToken);
  Task<IEnumerable<StateChange<T>>> Filter(Expression<Func<T, bool>> filter, CancellationToken cancellationToken);
  Task<IEnumerable<StateChange<T>>> GetManyById(IEnumerable<Guid> ids, CancellationToken cancellationToken);
  Task Upsert(StateChange<T> stateChange, CancellationToken cancellationToken);
}

public class ReadModelRepository<T>(
  IDocumentStore<PersistableStateChange<T>> documentStore
) : IReadModelRepository<T> where T : IReplayable<T>, new()
{
  public async Task<Maybe<StateChange<T>>> MaybeGetById(
    Guid id,
    CancellationToken cancellationToken
  )
  {
    var change = await documentStore.GetByIdIfExisting(id, cancellationToken);

    return Maybe<PersistableStateChange<T>>
      .ForValue(change)
      .Map(d => d.ToStateChange());
  }

  public async Task<StateChange<T>> GetById(
    Guid id,
    CancellationToken cancellationToken
  )
  {
    var change = await documentStore.GetById(id, cancellationToken);

    return change.ToStateChange();
  }

  public async Task<IEnumerable<StateChange<T>>> Filter(
    Expression<Func<T, bool>> filter,
    CancellationToken cancellationToken
  )
  {
    var param = Expression.Parameter(typeof(PersistableStateChange<T>), "m");
    var stateProperty = Expression.PropertyOrField(param, nameof(PersistableStateChange<T>.State));
    var invokedExpr = Expression.Invoke(filter, stateProperty);
    var expression = Expression.Lambda<Func<PersistableStateChange<T>, bool>>(invokedExpr, param);

    var changes = await documentStore.FilterBy(expression, cancellationToken);

    return changes.Select(c => c.ToStateChange());
  }

  public Task<IEnumerable<StateChange<T>>> GetManyById(
    IEnumerable<Guid> ids,
    CancellationToken cancellationToken
  )
  {
    throw new NotImplementedException();
  }

  public async Task Upsert(
    StateChange<T> stateChange,
    CancellationToken cancellationToken
  )
  {
    var persistableChange = new PersistableStateChange<T>(Guid.NewGuid(), stateChange);

    await documentStore.UpsertOne(persistableChange, cancellationToken);
  }
}
