using Microsoft.Extensions.Caching.Hybrid;

namespace Core.Infrastructure.Caching;

public static class HybridCacheExtensions
{
  public static async Task SetEntry<T>(
    this HybridCache cache,
    CacheKey entryKey,
    T data,
    HybridCacheEntryOptions? options = null,
    CancellationToken cancellationToken = default
  ) => await cache.SetAsync(entryKey.Key, data, options, entryKey.Tags, cancellationToken);

  public static async Task SetEntryWithExpiration<T>(
    this HybridCache cache,
    CacheKey entryKey,
    T data,
    TimeSpan expiration,
    CancellationToken cancellationToken = default
  ) => await cache.SetAsync(
    entryKey.Key,
    data,
    new HybridCacheEntryOptions
    {
      Expiration = expiration
    },
    entryKey.Tags,
    cancellationToken
  );

  public static async Task<T> GetAndSetEntryIfExpired<T>(
    this HybridCache cache,
    CacheKey entryKey,
    Func<CancellationToken, Task<T>> fetchData,
    HybridCacheEntryOptions? options = null,
    CancellationToken cancellationToken = default
  ) =>
    await cache.GetOrCreateAsync(
      entryKey.Key,
      async cancel => await fetchData(cancel),
      options,
      entryKey.Tags,
      cancellationToken
    );

  public static async Task<T> GetAndSetEntryWithExpirationIfExpired<T>(
    this HybridCache cache,
    CacheKey entryKey,
    Func<CancellationToken, Task<T>> fetchData,
    TimeSpan expiration,
    CancellationToken cancellationToken = default
  ) =>
    await cache.GetOrCreateAsync(
      entryKey.Key,
      async cancel => await fetchData(cancel),
      new HybridCacheEntryOptions
      {
        Expiration = expiration
      },
      entryKey.Tags,
      cancellationToken
    );

  public static async Task<Maybe<T>> GetEntry<T>(
    this HybridCache cache,
    CacheKey entryKey,
    CancellationToken cancellationToken = default
  )
  {
    var entry = await cache.GetOrCreateAsync<T>(
      entryKey.Key,
      factory: null!,
      new HybridCacheEntryOptions
      {
        Flags =
          HybridCacheEntryFlags.DisableUnderlyingData
          | HybridCacheEntryFlags.DisableLocalCacheWrite
          | HybridCacheEntryFlags.DisableDistributedCacheWrite,
      },
      cancellationToken: cancellationToken
    );

    return Maybe<T>.ForValue(entry);
  }

  public static async Task RemoveByTag(
    this HybridCache cache,
    string tag,
    CancellationToken cancellationToken = default
  ) => await cache.RemoveByTagAsync(tag, cancellationToken);

  public static async Task RemoveByKey(
    this HybridCache cache,
    CacheKey entryKey,
    CancellationToken cancellationToken = default
  ) => await cache.RemoveAsync(entryKey.Key, cancellationToken);
}
