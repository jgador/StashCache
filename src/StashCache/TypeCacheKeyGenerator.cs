using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace StashCache
{
	public class TypeCacheKeyGenerator : ICacheKeyGenerator<TypeCacheKeyGenerator>
	{
		public CacheKey GenerateCacheKey<TOwner>([CallerMemberName] string callerMemberName = null, IEnumerable<string> segments = null)
		{
			return new(typeof(TOwner), callerMemberName, segments);
		}
	}
}
