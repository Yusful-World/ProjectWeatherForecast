using Microsoft.AspNetCore.Mvc;
using WebApplication1.Data;
using WebApplication1.Model;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        private WebApiDbContext _dbContext;

        public WeatherForecastController()
        {
        }

        public WeatherForecastController(WebApiDbContext dbContext, ILogger<WeatherForecastController> logger)
        {
            _dbContext = dbContext;
            _logger = logger;

            if (!_dbContext.Forecasts.Any())
            {
                var forecasts = Enumerable.Range(1, 5).Select(index => new WeatherForecast
                {
                    Id = index,
                    Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    TemperatureC = Random.Shared.Next(-20, 55),
                    Summary = Summaries[Random.Shared.Next(Summaries.Length)]
                }
                ).ToList();

                _dbContext.Forecasts.AddRange(forecasts);
                _dbContext.SaveChanges();
            }
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            return _dbContext.Forecasts.ToList();

        }

        [HttpPost(Name = "Create New Forecast")]
        public IActionResult Post([FromBody] WeatherForecast newForecast)
        {
            if (newForecast == null)
            {
                return BadRequest("weather forecast data is required");
            }

            _dbContext.Forecasts.Add(newForecast);
            _dbContext.SaveChanges();

            return CreatedAtRoute("GetWeatherForecast", new { Id = newForecast.Id }, "New forecast created successfully");
        }

        [HttpPatch("{Id}", Name = "Update Forecast")]
        public IActionResult Patch(int Id, [FromBody] WeatherForecast updateForecast)
        {
            var forecast = _dbContext.Forecasts.FirstOrDefault(forecast => forecast.Id == Id);
            if (forecast == null)
            {
                return NotFound("Weather forecast data not found");
            }

            if (updateForecast.Date != default)
            {
                forecast.Date = updateForecast.Date;
            }
            if (updateForecast.TemperatureC != 0)
            {
                forecast.TemperatureC = updateForecast.TemperatureC;
            }
            if (!string.IsNullOrEmpty(updateForecast.Summary))
            {
                forecast.Summary = updateForecast.Summary;
            }

            _dbContext.SaveChanges();
            
            return Ok("forecast updated successfully");

        }

        [HttpDelete("{Id}", Name = "Delete Forecast")]
        public IActionResult Delete(int Id)
        {
            var forecast =  _dbContext.Forecasts.FirstOrDefault(f => f.Id == Id);
            if (forecast == null)
            {
                return NotFound("Weather forecast data not found");
            }

            _dbContext.Forecasts.Remove(forecast);
            _dbContext.SaveChanges();
            
            return NoContent();
        }

    }
}
