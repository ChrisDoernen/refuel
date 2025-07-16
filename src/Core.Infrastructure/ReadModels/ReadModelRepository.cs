using System.Linq.Expressions;
using Core.Infrastructure.Cqrs;

namespace Core.Infrastructure.ReadModels;

public interface IReadModelRepository<T> where T : IReplayable<T>, new()
{
  // Returns null if the state change/read model does not exist
  Task<StateChange<T>?> GetById(Guid id, CancellationToken cancellationToken);
  Task<StateChange<T>> FindById(Guid id, CancellationToken cancellationToken);
  Task<IEnumerable<StateChange<T>>> Filter(Expression<Func<T, bool>> filter, CancellationToken cancellationToken);
  Task<IEnumerable<StateChange<T>>> GetManyById(IEnumerable<Guid> ids, CancellationToken cancellationToken);
  Task Create(StateChange<T> readModel, CancellationToken cancellationToken);
  Task Update(StateChange<T> readModel, CancellationToken cancellationToken);
}

public class ReadModelRepository
{
}
