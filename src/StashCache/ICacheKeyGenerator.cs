using System.Runtime.CompilerServices;

namespace StashCache
{
    /// <summary>
    /// A cache key generator for the local in-memory cache.
    /// </summary>
    /// <typeparam name="TGenerator">The type of cache key generator.</typeparam>
    public interface ICacheKeyGenerator<TGenerator>
    {
        /// <summary>
        /// Generates a cache key.
        /// </summary>
        /// <typeparam name="TOwner">The class from which the cache key is generated.</typeparam>
        /// <param name="callerMemberName">The name of the method in <typeparamref name="TOwner"/> that needs caching.</param>
        /// <param name="segments">The optional collection of segments that will be appended to the key.</param>
        /// <returns>A cache key for local in-memory cache.</returns>
        CacheKey GenerateCacheKey<TOwner>([CallerMemberName] string callerMemberName = null, string[] segments = null);
    }
}
