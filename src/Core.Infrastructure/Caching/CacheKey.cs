namespace Core.Infrastructure.Caching;

public record CacheKey(
  string Key,
  IEnumerable<string>? Tags = null
);
