using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace StashCache
{
	public static class CacheKeyGeneratorFactory
    {
        private static object _lock = new object();
        private static readonly IEnumerable<Type> CacheGenerators = null;

        static CacheKeyGeneratorFactory()
        {
            lock (_lock)
            {
                if (CacheGenerators == null)
                {
                    var types = Assembly.GetExecutingAssembly()
                        .GetTypes()
                        .Where(t => t.GetInterfaces().Any(typeInterface => typeInterface.IsGenericType && typeInterface.GetGenericTypeDefinition() == typeof(ICacheKeyGenerator<>)));

                    CacheGenerators = types;
                }
            }
        }

        public static ICacheKeyGenerator<TGenerator> GetCacheKeyGenerator<TGenerator>()
        {
            // TODO: add checking for multiple or no generator found.
            var generator = CacheGenerators.FirstOrDefault(t => t.IsAssignableTo(typeof(ICacheKeyGenerator<TGenerator>)));

            return Activator.CreateInstance(generator) as ICacheKeyGenerator<TGenerator>;
        }
    }
}
