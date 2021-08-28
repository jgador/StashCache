using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace StashCache
{
    public class TypeCacheKeyGenerator : ICacheKeyGenerator<TypeCacheKeyGenerator>
    {
        public CacheKey GenerateCacheKey<TOwner>([CallerMemberName] string? callerMemberName = null, IEnumerable<string>? segments = null)
        {
#pragma warning disable CS8604 // Possible null reference argument.
            return new(typeof(TOwner), callerMemberName, segments);
#pragma warning restore CS8604 // Possible null reference argument.
        }
    }
}
