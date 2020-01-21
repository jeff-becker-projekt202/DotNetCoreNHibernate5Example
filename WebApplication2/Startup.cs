using System;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WebApplication2.Models;
using NHibernate;
using NHibernate.Tool.hbm2ddl;
using WebApplication2.Db;

namespace WebApplication2
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }


        public void ConfigureServices(IServiceCollection services)
        {

            //TODO: NHiberate uses log4net internally if you're not using log4net as the logging provider
            // Log4net needs to be set up with an appender to write to the asp.net core logging system
            services.AddOptions();
            services.Configure<NHibernateExportSettings>(Configuration.GetSection("NHibernateExport"));
            services.AddControllers();
            services.AddSingleton<WeatherDbConfig>();
            services.AddSingleton(sp => sp.GetRequiredService<WeatherDbConfig>().BuildSessionFactory());
            services.AddScoped(sp => sp.GetRequiredService<ISessionFactory>().OpenSession());
            services.AddSwaggerGen(setup =>
            {
                setup.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "Weather Forcast API",
                    Version = "v1"
                });
            });


        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            app.UseSwagger();
            app.UseSwaggerUI(swagger =>
            {
                swagger.SwaggerEndpoint("/swagger/v1/swagger.json", "ChargeSavvy API v1");

            });

            using (var serviceScope = app.ApplicationServices
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope())
            {
                var cfg = serviceScope.ServiceProvider.GetRequiredService<WeatherDbConfig>();
                cfg.ExportMappings();
                cfg.ExportSchema();
                if (File.Exists("Weather.db"))
                {
                    File.Delete("Weather.db");
                }
                cfg.CreateDatabase();
                var seedDate = WeatherForecast.CreateForecasts(20, DateTime.Today.AddDays(-10));
                var session = serviceScope.ServiceProvider.GetRequiredService<ISession>();
                using (var tx = session.BeginTransaction())
                {
                    foreach (var f in seedDate)
                    {
                        session.Save(f);
                    }
                    tx.Commit();
                }

            }

   
            
        }
    }
}
