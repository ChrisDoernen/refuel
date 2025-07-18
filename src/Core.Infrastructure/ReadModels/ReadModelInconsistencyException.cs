using Core.Infrastructure.Caching;

namespace Core.Infrastructure.ReadModels;

public class ReadModelInconsistencyException : Exception
{
  public ReadModelInconsistencyException(CacheKey key)
    : base($"Read model is inconsistent for cache key: {key}")
  {
  }

  public ReadModelInconsistencyException(string message)
    : base(message)
  {
  }

  public ReadModelInconsistencyException(string message, Exception innerException)
    : base(message, innerException)
  {
  }
}
