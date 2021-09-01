using StashCache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Sample.AspNetCore.Services
{
    public class WeatherForecastService
    {
        private static readonly ICacheKeyGenerator<TypeCacheKeyGenerator> CacheKeyGenerator = CacheKeyGeneratorFactory.GetCacheKeyGenerator<TypeCacheKeyGenerator>();
        private static readonly TimeSpan DefaultCacheExpiry = TimeSpan.FromHours(1);
        private readonly ILocalCache _localCache;

        public WeatherForecastService(ILocalCache localCache)
        {
            _localCache = localCache;
        }

        public async Task<IEnumerable<WeatherForecast>> GetAll(CancellationToken cancellationToken)
        {
            var cacheKey = CacheKeyGenerator.GenerateCacheKey<WeatherForecastService>();

            var result = await _localCache.GetOrAddAsync(cacheKey, async () =>
            // var result = await _localCache.GetOrAddWithReaderWriterLockSlimAsync(cacheKey, async () =>
            {
                var summaries = await GetSummariesAsyc();

                return summaries;

            }, DefaultCacheExpiry, cancellationToken).ConfigureAwait(false);

            return result;
        }

        public async Task<IEnumerable<WeatherForecast>> GetBySummaryAsync(string summary, CancellationToken cancellationToken)
        {
            var cacheKey = CacheKeyGenerator.GenerateCacheKey<WeatherForecastService>(segments: new string[] { summary, null });

            var cachedValues = await _localCache.GetOrAddAsync(cacheKey, async () =>
            // var cachedValues = await _localCache.GetOrAddWithReaderWriterLockSlimAsync(cacheKey, async () =>
            {
                var result = (await GetSummariesAsyc())
                    .Where(a => a.Summary.ToLower() == summary.ToLower());

                return result;

            }, DefaultCacheExpiry, cancellationToken).ConfigureAwait(false);


            return cachedValues;
        }

        private async Task<IEnumerable<WeatherForecast>> GetSummariesAsyc()
        {
            // Mimic awaitable task here (i.e database call)
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
    }
}
