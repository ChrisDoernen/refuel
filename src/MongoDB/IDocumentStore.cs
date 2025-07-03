using System.Linq.Expressions;

namespace MongoDB;

public interface IDocumentStore<T> : IDisposable where T : IDocument
{
  Task<IEnumerable<T>> GetAll(CancellationToken cancellationToken = default);
  Task<T> GetById(Guid id, CancellationToken cancellationToken = default);
  Task<IEnumerable<T>> GetManyById(IEnumerable<Guid> ids, CancellationToken cancellationToken = default);
  Task<T?> GetByIdIfExisting(Guid id, CancellationToken cancellationToken = default);
  Task<IEnumerable<T>> FilterBy(Expression<Func<T, bool>> filter, CancellationToken cancellationToken = default);
  Task<T> GetSingle(Expression<Func<T, bool>> filter, CancellationToken cancellationToken = default);
  Task<bool> ExistsById(Guid id, CancellationToken cancellationToken = default);
  Task<bool> Exists(Expression<Func<T, bool>> expression, CancellationToken cancellationToken = default);
  Task CreateOne(T entity, CancellationToken cancellationToken = default);
  Task CreateMany(IEnumerable<T> entities, CancellationToken cancellationToken = default);
  Task UpdateOne(T entity, CancellationToken cancellationToken = default);
  Task UpdateMany(IEnumerable<T> entities, bool upsert = true, CancellationToken cancellationToken = default);
  Task DeleteOne(T entity, CancellationToken cancellationToken = default);
  Task DeleteMany(IEnumerable<T> entities, CancellationToken cancellationToken = default);
}
