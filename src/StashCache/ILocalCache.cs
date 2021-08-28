using System;
using System.Threading;
using System.Threading.Tasks;

namespace StashCache
{
    public interface ILocalCache
    {
        public Task<TResult> GetOrAddAsync<TResult>(CacheKey cacheKey, Func<CancellationToken, Task<TResult>> valueFactory, TimeSpan timeToLive, CancellationToken cancellationToken);
    }
}
