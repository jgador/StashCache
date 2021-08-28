using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
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

        public async Task<TResult> GetOrAddAsync<TResult>(CacheKey cacheKey, Func<CancellationToken, Task<TResult>> valueFactory, TimeSpan timeToLive, CancellationToken cancellationToken)
        {
            valueFactory.NotNull(nameof(valueFactory));

            if (_memoryCache.TryGetValue(cacheKey, out TResult cachedValue))
            {
                _logger.LogDebug($"Cache hit: {cacheKey}");

                return cachedValue;
            }

            cachedValue = await valueFactory(cancellationToken).ConfigureAwait(false);

            if (cachedValue == null)
            {
                throw new NotSupportedException("Does not support caching of null value.");
            }

            var newCacheValue = _locks.GetOrAdd(cacheKey, cachedValue);

            try
            {
                lock (newCacheValue)
                {
                    if (_memoryCache.TryGetValue(cacheKey, out cachedValue))
                    {
                        _logger.LogDebug($"Cache hit: {cacheKey}");
                    }
                    else
                    {
                        cachedValue = (TResult)newCacheValue;

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
