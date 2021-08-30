using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StashCache;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Benchmarks.StashCache
{
    [MemoryDiagnoser]
    [Orderer(SummaryOrderPolicy.FastestToSlowest)]
    [RankColumn]
    public class LocalCacheBenchmarks
    {
        private static readonly ILocalCache LocalCache;
        private static readonly TimeSpan DefaultCacheExpiry = TimeSpan.FromHours(1);
        private static readonly ICacheKeyGenerator<TypeCacheKeyGenerator> CacheKeyGenerator = CacheKeyGeneratorFactory.GetCacheKeyGenerator<TypeCacheKeyGenerator>();

        static LocalCacheBenchmarks()
        {
            var services = new ServiceCollection();

            services.AddLogging(configure => configure.AddConsole());
            services.AddStashCache();

            var serviceProvider = services.BuildServiceProvider();

            var localCache = serviceProvider.GetRequiredService<ILocalCache>();

            LocalCache = localCache;
        }

        [Benchmark]
        public async Task LocalCacheGetOrAddAsync()
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            var cacheKey = CacheKeyGenerator.GenerateCacheKey<LocalCacheBenchmarks>();

            var result = await LocalCache.GetOrAddAsync(cacheKey, async () =>
            {
                var summaries = await GetSummariesAsyc();

                return summaries;

            }, DefaultCacheExpiry, cts.Token).ConfigureAwait(false);
        }

        [Benchmark]
        public async Task LocalCacheGetOrAddWithReaderWriterLockSlimAsync()
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            var cacheKey = CacheKeyGenerator.GenerateCacheKey<LocalCacheBenchmarks>();

            var result = await LocalCache.GetOrAddWithReaderWriterLockSlimAsync(cacheKey, async () =>
            {
                var summaries = await GetSummariesAsyc();

                return summaries;

            }, DefaultCacheExpiry, cts.Token).ConfigureAwait(false);
        }

        private async Task<IEnumerable<WeatherForecast>> GetSummariesAsyc()
        {
            await Task.CompletedTask;

            var random = new Random();

            return new List<WeatherForecast>()
            {
                new WeatherForecast { Summary = "Freezing", TemperatureC = random.Next(-20, 55) },
                new WeatherForecast { Summary = "Bracing", TemperatureC = random.Next(-20, 55) },
                new WeatherForecast { Summary = "Chilly", TemperatureC = random.Next(-20, 55) },
                new WeatherForecast { Summary = "Cool", TemperatureC = random.Next(-20, 55) },
                new WeatherForecast { Summary = "Mild", TemperatureC = random.Next(-20, 55) },
                new WeatherForecast { Summary = "Warm", TemperatureC = random.Next(-20, 55) },
                new WeatherForecast { Summary = "Balmy", TemperatureC = random.Next(-20, 55) },
                new WeatherForecast { Summary = "Hot", TemperatureC = random.Next(-20, 55) },
                new WeatherForecast { Summary = "Sweltering", TemperatureC = random.Next(-20, 55) },
                new WeatherForecast { Summary = "Scorching", TemperatureC = random.Next(-20, 55) }
            }.ToArray();
        }

        private class WeatherForecast
        {
            public int TemperatureC { get; set; }
            public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
            public string Summary { get; set; }
        }
    }
}
