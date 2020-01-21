using System;
using System.Collections.Generic;
using System.Linq;

namespace WebApplication2.Models
{
    public class WeatherForecast : EntityBase
    {
        
        public virtual DateTime Date { get; set; }

        public virtual int TemperatureC { get; set; }


        public virtual string Summary { get; set; }

        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };
        public static IEnumerable<WeatherForecast> CreateForecasts(int count, DateTime baseDate)
        {
            var rng = new Random();
            return Enumerable.Range(1, count).Select(index => new WeatherForecast
            {
                Date = baseDate.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }


}
