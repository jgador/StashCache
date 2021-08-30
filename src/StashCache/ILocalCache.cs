using System;
using System.Threading;
using System.Threading.Tasks;

namespace StashCache
{
    public interface ILocalCache
    {
        Task<TResult> GetOrAddAsync<TResult>(CacheKey cacheKey, Func<Task<TResult>> valueFactory, TimeSpan timeToLive, CancellationToken cancellationToken);

        Task<TResult> GetOrAddWithReaderWriterLockSlimAsync<TResult>(CacheKey cacheKey, Func<Task<TResult>> valueFactory, TimeSpan timeToLive, CancellationToken cancellationToken);
    }
}
