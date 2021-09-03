using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
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
        [Arguments(50, 1000)]
        [Arguments(100, 1000)]
        [Arguments(500, 1000)]
        [Arguments(1000, 1000)]
        [Arguments(2000, 2000)]
        // [Arguments(5000, 2000)]
        // [Arguments(10000, 2000)]
        public async Task GetOrAddAsync(int cacheItemsCount, int retrieveCount)
		{
			using (var cts = new CancellationTokenSource())
			{
				while (retrieveCount-- > 0)
				{
					await AddOrRetrieveFromCacheAsync(cacheItemsCount, cts.Token).ConfigureAwait(false);
				}
			}
		}

		private async Task AddOrRetrieveFromCacheAsync(int records, CancellationToken cancellationToken)
		{
			for (int i = 1; i <= records; i++)
			{
				var cacheKey = CacheKeyGenerator.GenerateCacheKey<LocalCacheBenchmarks>(segments: new string[1] { i.ToString() });

				var result = await LocalCache.GetOrCreateAsync(cacheKey, async () =>
				{
					var summaries = await GetSummariesAsyc();

					return summaries;

				}, DefaultCacheExpiry, cancellationToken).ConfigureAwait(false);

				// Debug.Assert(result != null);
			}
		}

		private async Task<IEnumerable<WeatherForecast>> GetSummariesAsyc()
		{
			await Task.CompletedTask;

			return new List<WeatherForecast>()
			{
				new WeatherForecast { Summary = "Freezing", TemperatureC = 1 },
				new WeatherForecast { Summary = "Bracing", TemperatureC = 2 },
				new WeatherForecast { Summary = "Chilly", TemperatureC = 3 },
				new WeatherForecast { Summary = "Cool", TemperatureC = 4 },
				new WeatherForecast { Summary = "Mild", TemperatureC = 5 },
				new WeatherForecast { Summary = "Warm", TemperatureC = 6 },
				new WeatherForecast { Summary = "Balmy", TemperatureC = 7 },
				new WeatherForecast { Summary = "Hot", TemperatureC = 8 },
				new WeatherForecast { Summary = "Sweltering", TemperatureC = 9 },
				new WeatherForecast { Summary = "Scorching", TemperatureC = 10 }
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
