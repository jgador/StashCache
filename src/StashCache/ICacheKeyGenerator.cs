using System.Runtime.CompilerServices;

namespace StashCache
{
    public interface ICacheKeyGenerator<TGenerator>
    {
        CacheKey GenerateCacheKey<TOwner>([CallerMemberName] string? callerMemberName = null, params string[]? segments);
    }
}
