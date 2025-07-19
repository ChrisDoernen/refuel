using Core.Infrastructure.Caching;

namespace Core.Infrastructure.Projections;

public class ProjectionInconsistencyException : Exception
{
  public ProjectionInconsistencyException(CacheKey key)
    : base($"Projection is inconsistent, cache key: {key}")
  {
  }

  public ProjectionInconsistencyException(string message)
    : base(message)
  {
  }

  public ProjectionInconsistencyException(string message, Exception innerException)
    : base(message, innerException)
  {
  }
}
