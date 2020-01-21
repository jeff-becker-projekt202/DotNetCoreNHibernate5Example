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

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSingleton<WeatherDbConfig>();
            services.AddSingleton(sp => sp.GetRequiredService<WeatherDbConfig>().BuildSessionFactory());
            services.AddTransient(sp => new SchemaExport(sp.GetRequiredService<WeatherDbConfig>()));
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

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
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
            if (!File.Exists("Weather.db"))
            {
                using (var serviceScope = app.ApplicationServices
                                .GetRequiredService<IServiceScopeFactory>()
                                .CreateScope())
                {
                    var export = serviceScope.ServiceProvider.GetRequiredService<SchemaExport>();
                    export.Create(true, true);
                    var seedDate = WeatherForecast.CreateForecasts(20, DateTime.Today.AddDays(-10));
                    var session = serviceScope.ServiceProvider.GetRequiredService<ISession>();
                    using(var tx = session.BeginTransaction())
                    {
                        foreach(var f in seedDate)
                        {
                            session.Save(f);
                        }
                        tx.Commit();
                    }

                }
            }
            
        }
    }
}
