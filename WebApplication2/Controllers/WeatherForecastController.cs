using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHibernate;
using NHibernate.Linq;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly ISession _session;

        public WeatherForecastController(ISession session, ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
            _session = session;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            using (var tx = _session.BeginTransaction())
            {
                var result = await _session.LoadAsync<WeatherForecast>(id);
                return Ok(result);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            using(var tx = _session.BeginTransaction())
            {
                var results = await _session.Query<WeatherForecast>()
                    .Where(x => x.Date >= DateTime.Today)
                    .ToListAsync();
                return Ok(results);
            }
        }
    }
}
