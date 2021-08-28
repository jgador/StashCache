using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StashCache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Sample.AspNetCore.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class WeatherForecastController : ControllerBase
	{
		private static readonly ICacheKeyGenerator<TypeCacheKeyGenerator> CacheKeyGenerator = CacheKeyGeneratorFactory.GetCacheKeyGenerator<TypeCacheKeyGenerator>();
		private static readonly TimeSpan DefaultCacheExpiry = TimeSpan.FromHours(1);

		private static readonly string[] Summaries = new[]
		{
			"Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
		};

		private readonly ILogger<WeatherForecastController> _logger;
		private readonly ILocalCache _localCache;

		public WeatherForecastController(ILogger<WeatherForecastController> logger, ILocalCache localCache)
		{
			_logger = logger;
			_localCache = localCache;
		}

		[HttpGet]
		public async Task<IEnumerable<WeatherForecast>> GetAsync(CancellationToken cancellationToken)
		{
			var cacheKey = CacheKeyGenerator.GenerateCacheKey<WeatherForecastController>();

			var cachedValues = await _localCache.GetOrAddAsync<IEnumerable<WeatherForecast>>(cacheKey, async() =>
			{
				// Mimic awaitable task here (i.e database call)
				await Task.CompletedTask;

				var random = new Random();

                return Enumerable.Range(1, 5).Select(index => new WeatherForecast
                {
                    Date = DateTime.Now.AddDays(index),
                    TemperatureC = random.Next(-20, 55),
                    Summary = Summaries[random.Next(Summaries.Length)]
                })
                .ToArray();


            }, DefaultCacheExpiry, cancellationToken).ConfigureAwait(false);


			return cachedValues;
		}
	}
}
