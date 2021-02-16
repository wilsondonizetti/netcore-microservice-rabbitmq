using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace api31.Controllers
{
    /// <summary>
    /// tempo
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Tempo
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }

        /// <summary>
        /// Versao
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("version")]
        public IActionResult GetVersion()
        {
            return Ok("3.1");
        }

        [HttpPost]
        [Route("msg")]
        public IActionResult PostMessage(string msg,[FromServices]RabbitMQService mqService){
            try
            {
                mqService.PostMessage<string>("mq.teste", msg);
                return Ok("mensagem postada!");   
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }            
        }
    }
}
