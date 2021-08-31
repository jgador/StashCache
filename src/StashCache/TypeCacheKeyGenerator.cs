using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace StashCache
{
    public class TypeCacheKeyGenerator : ICacheKeyGenerator<TypeCacheKeyGenerator>
    {
        public CacheKey GenerateCacheKey<TOwner>([CallerMemberName] string callerMemberName = null, string[] segments = null)
        {
            return new CacheKey(typeof(TOwner), callerMemberName, segments);
        }
    }
}
