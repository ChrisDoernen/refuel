using System.Linq.Expressions;
using Core.Infrastructure.Cqrs;

namespace Core.Infrastructure.ReadModels;

public interface IReadModelRepository<T> where T : IReplayable<T>, new()
{
  Task<Maybe<StateChange<T>>> MaybeGetById(Guid id, CancellationToken cancellationToken);
  Task<StateChange<T>> GetById(Guid id, CancellationToken cancellationToken);
  Task<IEnumerable<StateChange<T>>> Filter(Expression<Func<T, bool>> filter, CancellationToken cancellationToken);
  Task<IEnumerable<StateChange<T>>> GetManyById(IEnumerable<Guid> ids, CancellationToken cancellationToken);
  Task Upsert(StateChange<T> readModel, CancellationToken cancellationToken);
}

public class ReadModelRepository
{
}
