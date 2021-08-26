using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StashCache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sample.AspNetCore.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class WeatherForecastController : ControllerBase
	{
		protected static readonly ICacheKeyGenerator<TypeCacheKeyGenerator> CacheKeyGenerator = CacheKeyGeneratorFactory.GetCacheKeyGenerator<TypeCacheKeyGenerator>();

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
		public async Task<IEnumerable<WeatherForecast>> GetAsync()
		{
			var cacheKey = CacheKeyGenerator.GenerateCacheKey<WeatherForecastController>();
			var cacheExpiry = TimeSpan.FromHours(1);

			var cachedValues = await _localCache.GetOrAddAsync(cacheKey, async () =>
			{
				// Mimic awaitable task here (i.e. database call)
				await Task.CompletedTask;

				var rng = new Random();

				return Enumerable.Range(1, 5).Select(index => new WeatherForecast
				{
					Date = DateTime.Now.AddDays(index),
					TemperatureC = rng.Next(-20, 55),
					Summary = Summaries[rng.Next(Summaries.Length)]
				})
				.ToArray();

			}, cacheExpiry).ConfigureAwait(false);

			return cachedValues;
		}
	}
}
