using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace StashCache
{
    /// <summary>
    /// An implementation of <see cref="ILocalCache"/> using <see cref="MemoryCache"/> to store its entries.
    /// </summary>
    [DebuggerDisplay("{_memoryCache}")]
	public class LocalCache : ILocalCache
    {
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<LocalCache> _logger;

        /// <summary>
        /// Creates a new <see cref="LocalCache"/> instance.
        /// </summary>
        /// <param name="memoryCache">The underlying in-memory cache where entries are actually stored.</param>
        /// <param name="logger">The logger to which cache events should be logged.</param>
        public LocalCache(IMemoryCache memoryCache, ILogger<LocalCache> logger)
        {
            _memoryCache = memoryCache.NotNull(nameof(memoryCache));
            _logger = logger.NotNull(nameof(logger));
        }

        /// <inheritdoc/>
        public async Task<TItem> GetOrCreateAsync<TItem>(CacheKey cacheKey, Func<Task<TItem>> valueFactory, TimeSpan timeToLive, CancellationToken cancellationToken)
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
    }
}
