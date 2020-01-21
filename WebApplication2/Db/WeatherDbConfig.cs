using Microsoft.Extensions.Configuration;
using NHibernate.Cfg;
using NHibernate.Dialect;
using NHibernate.Mapping.ByCode;
using System.Linq;
using WebApplication2.Models;

namespace WebApplication2.Db
{
    public class WeatherDbConfig : NHibernate.Cfg.Configuration
    {
        public WeatherDbConfig(IConfiguration appConfig)
        {
            this.DataBaseIntegration(db =>
            {
                db.Dialect<SQLiteDialect>();
                db.ConnectionString = appConfig.GetConnectionString("DefaultConnection");
            });
            /*
            // This chunk is for explicit coded mappings, uncomment WeatherForcastMapping to use this
            var mapper = new ModelMapper();
            //Here we're adding explicitly coded mappings but
            //convention based mappings are supported
            mapper.AddMappings(typeof(WeatherDbConfig).Assembly.GetExportedTypes());
            var mapping = mapper.CompileMappingForAllExplicitlyAddedEntities();

            AddMapping(mapping);
            */
            //http://fabiomaulo.blogspot.com/2011/04/nhibernate-32-mapping-by-code_13.html
            var mapper = new ConventionModelMapper();
            var baseEntityType = typeof(EntityBase);
            mapper.IsEntity((t, declared) => baseEntityType.IsAssignableFrom(t) && baseEntityType != t && !t.IsInterface);
            mapper.IsRootEntity((t, declared) => baseEntityType.Equals(t.BaseType));

            mapper.Class<EntityBase>(map =>
            {
                map.Schema("weatherExample");
                map.Id(x => x.Id, idCfg => idCfg.Generator(Generators.Identity));
            });

            var mapping = mapper.CompileMappingFor(baseEntityType.Assembly.GetExportedTypes().Where(t => t.Namespace.EndsWith("Models")));
            AddMapping(mapping);
        }

        
    }
}
