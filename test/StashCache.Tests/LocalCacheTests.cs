using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace StashCache.Tests
{
    [TestClass]
    public class LocalCacheTests
    {
        private static readonly ICacheKeyGenerator<TypeCacheKeyGenerator> CacheKeyGenerator = CacheKeyGeneratorFactory.GetCacheKeyGenerator<TypeCacheKeyGenerator>();
        private List<string> DataSource = null;
        private ServiceCollection Services = null;

        [TestCleanup]
        public void Cleanup()
        {
            DataSource = null;
            Services = null;
        }

        [TestInitialize]
        public void Initialize()
        {
            DataSource = new()
            {
                "cacheitem1",
                "cacheitem2",
                "cacheitem3"
            };

            Services = new();
            Services.AddLogging(configure => configure.AddConsole());
            Services.AddStashCache();
        }

        [TestMethod]
        public void LocalCacheShouldNotBeNull()
        {
            var serviceProvider = Services.BuildServiceProvider();
            var localCache = serviceProvider.GetRequiredService<ILocalCache>();

            Assert.IsNotNull(localCache);
        }

        [TestMethod]
        public async Task GetOrCreateAsync_ShouldReturnTheSameCachedValues_DespiteChangingTheDatasource()
        {
            var serviceProvider = Services.BuildServiceProvider();
            var localCache = serviceProvider.GetRequiredService<ILocalCache>();

            var cacheKey = CacheKeyGenerator.GenerateCacheKey<LocalCacheTests>();

            for (int i = 4; i <= 10; i++)
            {
                var cachedValues = await localCache.GetOrCreateAsync(cacheKey, async () =>
                {
                    await Task.CompletedTask;

                    // Copy the datasource so as not to to cling on it's reference
                    // In real-world scenario, it's calling to an external source (e.g. database)

                    var result = new string[DataSource.Count];
                    DataSource.CopyTo(result);

                    return result.ToList();

                }, TimeSpan.FromMinutes(10), default);

                Assert.IsNotNull(cachedValues);
                Assert.AreEqual(3, cachedValues.Count);
                Assert.IsTrue(cachedValues.Contains("cacheitem1"));
                Assert.IsTrue(cachedValues.Contains("cacheitem2"));
                Assert.IsTrue(cachedValues.Contains("cacheitem3"));

                // Add items to Datasource
                DataSource.Add($"cacheitem{i}");
            }
        }

        [TestMethod]
        public async Task GetOrCreateAsync_ShouldReturnAddedItemsToCache_AfterCacheExpired()
        {
            var serviceProvider = Services.BuildServiceProvider();
            var localCache = serviceProvider.GetRequiredService<ILocalCache>();

            var cacheKey = CacheKeyGenerator.GenerateCacheKey<LocalCacheTests>();

            var cachedValues = await localCache.GetOrCreateAsync(cacheKey, async () =>
            {
                await Task.CompletedTask;

                // Copy the datasource so as not to to cling on it's reference
                // In real-world scenario, it's calling to an external source (e.g. database)

                var result = new string[DataSource.Count];
                DataSource.CopyTo(result);

                return result.ToList();

            }, TimeSpan.FromSeconds(2), default);

            Assert.IsNotNull(cachedValues);
            Assert.AreEqual(3, cachedValues.Count);
            Assert.IsTrue(cachedValues.Contains("cacheitem1"));
            Assert.IsTrue(cachedValues.Contains("cacheitem2"));
            Assert.IsTrue(cachedValues.Contains("cacheitem3"));

            // Sleep in order to expire the cache
            await Task.Delay(TimeSpan.FromSeconds(5));

            for (int i = 4; i <= 10; i++)
            {
                // Add items to Datasource
                DataSource.Add($"cacheitem{i}");
            }

            var cachedValuesAfterExpired = await localCache.GetOrCreateAsync(cacheKey, async () =>
            {
                await Task.CompletedTask;

                // Copy the datasource so as not to to cling on it's reference
                // In real-world scenario, it's calling to an external source (e.g. database)

                var result = new string[DataSource.Count];
                DataSource.CopyTo(result);

                return result.ToList();

            }, TimeSpan.FromSeconds(2), default);

            Assert.IsNotNull(cachedValuesAfterExpired);
            Assert.AreEqual(10, cachedValuesAfterExpired.Count);
            Assert.IsTrue(cachedValuesAfterExpired.Contains("cacheitem1"));
            Assert.IsTrue(cachedValuesAfterExpired.Contains("cacheitem2"));
            Assert.IsTrue(cachedValuesAfterExpired.Contains("cacheitem3"));
            Assert.IsTrue(cachedValuesAfterExpired.Contains("cacheitem4"));
            Assert.IsTrue(cachedValuesAfterExpired.Contains("cacheitem5"));
            Assert.IsTrue(cachedValuesAfterExpired.Contains("cacheitem6"));
            Assert.IsTrue(cachedValuesAfterExpired.Contains("cacheitem7"));
            Assert.IsTrue(cachedValuesAfterExpired.Contains("cacheitem8"));
            Assert.IsTrue(cachedValuesAfterExpired.Contains("cacheitem9"));
            Assert.IsTrue(cachedValuesAfterExpired.Contains("cacheitem10"));
        }
    }
}
