using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using WeatherApi.Hubs;

namespace WeatherApi.Controllers
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
        private readonly IHubContext<NotificationsHub> _notifications;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IHubContext<NotificationsHub> notifications)
        {
            _logger = logger;
            _notifications = notifications;
        }

        [HttpGet]
        [Authorize("CanSeeForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            _notifications.Clients.All.SendAsync("dataFetched", this.User.Identity.Name);

            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)],
                User = this.User.Identity.Name
            })
            .ToArray();
        }
    }
}
