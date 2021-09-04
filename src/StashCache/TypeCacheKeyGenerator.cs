using System.Runtime.CompilerServices;

namespace StashCache
{
    /// <summary>
    /// <para>
    ///     Generates key which can be uniquely identified by the following fields:
    /// </para>
    /// <para>a) Type from which the key is generated</para>
    /// <para>b) The method that returns cached values</para>
    /// <para>c) Optional collection of segments that can be appended to the key</para>
    /// </summary>
    public class TypeCacheKeyGenerator : ICacheKeyGenerator<TypeCacheKeyGenerator>
    {
        /// <inheritdoc/>
        public CacheKey GenerateCacheKey<TOwner>([CallerMemberName] string callerMemberName = null, string[] segments = null)
        {
            return new CacheKey(typeof(TOwner), callerMemberName, segments);
        }
    }
}
