using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace StashCache
{
    public sealed class LocalCache : ILocalCache
    {
        private static readonly ConcurrentDictionary<CacheKey, object> _locks = new ConcurrentDictionary<CacheKey, object>();
        private ReaderWriterLockSlim _readerWriterLock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<LocalCache> _logger;

        public LocalCache(IMemoryCache memoryCache, ILogger<LocalCache> logger)
        {
            _memoryCache = memoryCache.NotNull(nameof(memoryCache));
            _logger = logger.NotNull(nameof(logger));
        }

        public async Task<TResult> GetOrAddAsync<TResult>(CacheKey cacheKey, Func<Task<TResult>> valueFactory, TimeSpan timeToLive, CancellationToken cancellationToken)
        {
            valueFactory.NotNull(nameof(valueFactory));

            if (_memoryCache.TryGetValue(cacheKey, out TResult cacheItem))
            {
                _logger.LogDebug($"Cache hit: {cacheKey}");

                return cacheItem;
            }

            cacheItem = await valueFactory().ConfigureAwait(false);

            if (cacheItem == null)
            {
                throw new NotSupportedException("Does not support caching of null value.");
            }

            _logger.LogDebug($"Cache miss: {cacheKey}");

            _memoryCache.Set(cacheKey, cacheItem, new MemoryCacheEntryOptions { AbsoluteExpirationRelativeToNow = timeToLive });

            return cacheItem;

            //var newCacheValue = _locks.GetOrAdd(cacheKey, cachedValue);

            //try
            //{
            //    lock (newCacheValue)
            //    {
            //        if (_memoryCache.TryGetValue(cacheKey, out cachedValue))
            //        {
            //            _logger.LogDebug($"Cache hit: {cacheKey}");
            //        }
            //        else
            //        {
            //            _logger.LogDebug($"Cache miss: {cacheKey}");

            //            cachedValue = (TResult)newCacheValue;

            //            _memoryCache.Set(cacheKey, cachedValue, new MemoryCacheEntryOptions { AbsoluteExpirationRelativeToNow = timeToLive });
            //        }

            //        return cachedValue;
            //    }
            //}
            //finally
            //{
            //    _locks.TryRemove(cacheKey, out _);
            //}
        }

        public async Task<TResult> GetOrAddWithReaderWriterLockSlimAsync<TResult>(CacheKey cacheKey, Func<Task<TResult>> valueFactory, TimeSpan timeToLive, CancellationToken cancellationToken)
        {
            valueFactory.NotNull(nameof(valueFactory));

            _readerWriterLock.EnterUpgradeableReadLock();

            try
            {
                if (_memoryCache.TryGetValue(cacheKey, out TResult cachedValue))
                {
                    _logger.LogDebug($"Cache hit: {cacheKey}");

                    return cachedValue;
                }

                _readerWriterLock.EnterWriteLock();

                try
                {
                    _logger.LogDebug($"Cache miss: {cacheKey}");

                    cachedValue = await valueFactory().ConfigureAwait(false);

                    if (cachedValue == null)
                    {
                        throw new NotSupportedException("Does not support caching of null value.");
                    }

                    _memoryCache.Set(cacheKey, cachedValue, new MemoryCacheEntryOptions { AbsoluteExpirationRelativeToNow = timeToLive });

                    return cachedValue;
                }
                finally
                {
                    _readerWriterLock.ExitWriteLock();
                }

            }
            finally
            {
                _readerWriterLock.ExitUpgradeableReadLock();
            }
        }
    }
}
