using Microsoft.AspNetCore.Mvc;
using Sample.AspNetCore.Services;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sample.AspNetCore.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly WeatherForecastService _weatherService;

        public WeatherForecastController(WeatherForecastService weatherService)
        {
            _weatherService = weatherService; ;
        }

        [HttpGet]
        [Route("")]
        public async Task<IEnumerable<WeatherForecast>> GetAsync(CancellationToken cancellationToken)
        {
            var result = await _weatherService.GetAll(cancellationToken);

            return result;
        }

        [HttpGet]
        [Route("{summary}")]
        public async Task<IEnumerable<WeatherForecast>> GetSummaryAsync([FromRoute] string summary, CancellationToken cancellationToken)
        {
            var result = await _weatherService.GetBySummaryAsync(summary, cancellationToken);

            return result;
        }
    }
}
