using Microsoft.AspNetCore.Mvc;
using Assessment_Backend.Core.Servies.InterFace;
using Assessment_Backend.DataLayer.Entities.User;
namespace Assessment_Backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly AssessmentDbContext _context;
        public WeatherForecastController(AssessmentDbContext context)
        {
               _context = context;
        }
        [HttpGet("x")]
        public async Task<ActionResult<bool>> x()
        {
            _context.Teachers.Add(new Teacher()
            {
                UserId = 1,
                
            });
            _context.SaveChanges();
            return true;
        }
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}
