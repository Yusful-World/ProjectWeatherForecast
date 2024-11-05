using Microsoft.AspNetCore.Mvc;
using WebApplication1.Data;
using WebApplication1.Model;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private string GetTemperatureSummary(int temperatureC) => temperatureC switch
        {
            <= 0 => "Freezing",
            > 0 and <= 10 => "Chilly",
            > 10 and <= 20 => "Cool",
            > 20 and <= 25 => "Mild",
            > 25 and <= 30 => "Warm",
            > 30 and <= 35 => "Balmy",
            > 35 and <= 40 => "Hot",
            > 40 and <= 50 => "Sweltering",
            _ => "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        private WebApiDbContext _dbContext;



        public WeatherForecastController(WebApiDbContext dbContext, ILogger<WeatherForecastController> logger)
        {
            _dbContext = dbContext;
            _logger = logger;

            //if (_dbContext.Forecasts.Any())
            //{
            //    _dbContext.Forecasts.RemoveRange(_dbContext.Forecasts);
            //    _dbContext.SaveChanges();
            //}

            if (!_dbContext.Forecasts.Any())
            {
                var forecasts = Enumerable.Range(1, 5).Select(index => 
                {
                    var temperatureC = Random.Shared.Next(-20, 55);
                    return new WeatherForecast
                    {
                        //Id = index,
                        Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                        TemperatureC = temperatureC,
                        Summary = GetTemperatureSummary(temperatureC)
                    };
                }
                ).ToList();

                _dbContext.Forecasts.AddRange(forecasts);
                //_dbContext.Forecasts.RemoveRange(_dbContext.Forecasts);
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

        [HttpPatch("{id}/{date}", Name = "Update Forecast")]
        public IActionResult Patch(int Id, DateOnly date, [FromBody] WeatherForecast updateForecast)
        {
            var forecast = _dbContext.Forecasts.FirstOrDefault(forecast => forecast.Id == Id || forecast.Date == date);
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

        [HttpDelete("{id}/{date}", Name = "Delete Forecast")]
        public IActionResult Delete(int Id, DateOnly date)
        {
            var forecast = _dbContext.Forecasts.FirstOrDefault(forecast => forecast.Id == Id || forecast.Date == date);

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
