using Microsoft.Extensions.Caching.Memory;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace StashCache
{
    /// <summary>
    /// Represents a local in-memory cache.
    /// </summary>
    public interface ILocalCache
    {
        /// <summary>
        /// Creates a cache entry to the <see cref="MemoryCache"/>
        /// if the key does not already exist.
        /// </summary>
        /// <typeparam name="TItem">The type of the values in the <see cref="MemoryCache"/>.</typeparam>
        /// <param name="cacheKey">An object identifying the requested entry.</param>
        /// <param name="valueFactory">The function used to generate a value for the key.</param>
        /// <param name="timeToLive">The absolute expiration time, relative to now.</param>
        /// <param name="cancellationToken">A cancellation token to observe.</param>
        /// <returns>The value for the key. This will either be the existing value for the key if the
        /// key is already in the <see cref="MemoryCache"/>, or the new value for the key as returned by <paramref name="valueFactory"/>
        /// if the key was not in the <see cref="MemoryCache"/>.
        /// </returns>
        Task<TItem> GetOrCreateAsync<TItem>(CacheKey cacheKey, Func<Task<TItem>> valueFactory, TimeSpan timeToLive, CancellationToken cancellationToken);
    }
}
