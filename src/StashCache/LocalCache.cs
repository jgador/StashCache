using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace StashCache
{
	public sealed class LocalCache : ILocalCache
    {
        private ReaderWriterLockSlim _readerWriterLock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<LocalCache> _logger;

        public LocalCache(IMemoryCache memoryCache, ILogger<LocalCache> logger)
        {
            _memoryCache = memoryCache.NotNull(nameof(memoryCache));
            _logger = logger.NotNull(nameof(logger));
        }

        public async Task<TItem> GetOrAddAsync<TItem>(CacheKey cacheKey, Func<Task<TItem>> valueFactory, TimeSpan timeToLive, CancellationToken cancellationToken)
        {
            valueFactory.NotNull(nameof(valueFactory));

            if (!_memoryCache.TryGetValue(cacheKey, out object result))
            {
                result = await valueFactory().ConfigureAwait(false);

                _memoryCache.Set(cacheKey, result, new MemoryCacheEntryOptions { AbsoluteExpirationRelativeToNow = timeToLive });

                _logger.LogDebug($"Cache miss: {cacheKey}");
            }

            return (TItem)result;
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
