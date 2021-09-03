using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace StashCache
{
    public static class CacheKeyGeneratorFactory
    {
        private static readonly object _lock = new object();
        private static readonly IEnumerable<Type> CacheGenerators = null;

        static CacheKeyGeneratorFactory()
        {
            lock (_lock)
            {
                if (CacheGenerators == null)
                {
                    var types = Assembly.GetExecutingAssembly()
                        .GetTypes()
                        .Where(t => t.GetInterfaces()
                            .Any(typeInterface => typeInterface.IsGenericType
                                && typeInterface.GetGenericTypeDefinition() == typeof(ICacheKeyGenerator<>))
                        );

                    CacheGenerators = types;
                }
            }
        }

        /// <summary>
        /// Gets the cache key generator instance for <typeparamref name="TGenerator"/>.
        /// </summary>
        /// <typeparam name="TGenerator">The type of cache key genrator.</typeparam>
        /// <returns>The cache key generator.</returns>
        public static ICacheKeyGenerator<TGenerator> GetCacheKeyGenerator<TGenerator>()
        {
            try
            {
                var generator = CacheGenerators.SingleOrDefault(t => typeof(ICacheKeyGenerator<TGenerator>).IsAssignableFrom(t));

                if (generator != null)
                {
                    return (ICacheKeyGenerator<TGenerator>)Activator.CreateInstance(generator);
                }

                throw new ArgumentException($"No cache key generator found for type {typeof(TGenerator).FullName}", nameof(TGenerator));
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Found multiple cache key generators for type {typeof(TGenerator).FullName}.", nameof(TGenerator), ex);
            }
        }
    }
}
