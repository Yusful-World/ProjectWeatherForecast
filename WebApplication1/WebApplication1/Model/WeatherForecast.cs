using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Model
{
    public class WeatherForecast
    {
        public int Id { get; set; }
        public DateOnly Date { get; set; }
        

        [Required]
        [DisplayName("Temperature \"°C\"")]
        public int TemperatureC { get; set; }

        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
        [DisplayName("Forecast Summary")]
        public string? Summary { get; set; }
    }
}
