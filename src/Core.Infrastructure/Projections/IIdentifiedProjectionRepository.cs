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
