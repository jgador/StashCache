using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace StashCache
{
	public sealed class LocalCache : ILocalCache
    {
        private static readonly ConcurrentDictionary<CacheKey, object> _locks = new();
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<LocalCache> _logger;

        public LocalCache([NotNull] IMemoryCache memoryCache, [NotNull] ILogger<LocalCache> logger)
        {
            _memoryCache = memoryCache.NotNull(nameof(memoryCache));
            _logger = logger.NotNull(nameof(logger));
        }

        public async Task<TResult> GetOrAddAsync<TResult>(CacheKey cacheKey, Func<Task<TResult>> valueFactory, TimeSpan timeToLive)
        {
            valueFactory.NotNull(nameof(valueFactory));

            if (_memoryCache.TryGetValue(cacheKey, out TResult cachedValue))
            {
                _logger.LogDebug($"Cache hit: {cacheKey}");

                return cachedValue;
            }

            cachedValue = await valueFactory().ConfigureAwait(false);

            var acquiredLock = _locks.GetOrAdd(cacheKey, _ => cachedValue);

            try
            {
                lock (acquiredLock)
                {
                    if (_memoryCache.TryGetValue(cacheKey, out cachedValue))
                    {
                        _logger.LogDebug($"Cache hit: {cacheKey}");
                    }
                    else
                    {
                        _logger.LogDebug($"Cache miss: {cacheKey}");

                        cachedValue = (TResult)acquiredLock;

                        _memoryCache.Set(cacheKey, cachedValue, new MemoryCacheEntryOptions { AbsoluteExpirationRelativeToNow = timeToLive });
                    }

                    return cachedValue;
                }
            }
            finally
            {
                _locks.TryRemove(cacheKey, out _);
            }
        }
    }
}
