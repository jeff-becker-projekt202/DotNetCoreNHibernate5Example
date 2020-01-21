using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using NHibernate.Cfg;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Dialect;
using NHibernate.Mapping.ByCode;
using NHibernate.Tool.hbm2ddl;
using System.IO;
using System.Linq;
using WebApplication2.Models;

namespace WebApplication2.Db
{
    public class WeatherDbConfig : NHibernate.Cfg.Configuration
    {
        public NHibernateExportSettings ConfigurationExportSettings { get; }
        public WeatherDbConfig(IConfiguration appConfig, IOptions<NHibernateExportSettings> exportSettings)
        {
            this.DataBaseIntegration(db =>
            {
                db.Dialect<SQLiteDialect>();
                db.ConnectionString = appConfig.GetConnectionString("DefaultConnection");
            });
            ConfigurationExportSettings = exportSettings.Value;
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

            mapper.BeforeMapManyToOne += (insp, prop, map) => map.Column(prop.LocalMember.GetPropertyOrFieldType().Name + "Id");
            mapper.BeforeMapManyToOne += (insp, prop, map) => map.Cascade(Cascade.Persist);
            mapper.BeforeMapBag += (insp, prop, map) => map.Key(km => km.Column(prop.GetContainerEntity(insp).Name + "Id"));
            mapper.BeforeMapBag += (insp, prop, map) => map.Cascade(Cascade.All);


            ConfiguredMapping = mapper.CompileMappingFor(baseEntityType.Assembly.GetExportedTypes().Where(t => t.Namespace.EndsWith("Models")));
            AddMapping(ConfiguredMapping);
        }
        public HbmMapping ConfiguredMapping { get; set; }

        public void ExportSchema()
        {
            if (this.ConfigurationExportSettings.ExportSchema)
            {
                if (File.Exists(this.ConfigurationExportSettings.SchemaFile))
                {
                    File.Delete(this.ConfigurationExportSettings.SchemaFile);
                }
                using (var tw = new StreamWriter(this.ConfigurationExportSettings.SchemaFile))
                {
                    ExportSchema(tw);
                }
            }
        }
        public void ExportSchema(TextWriter writer)
        {
            var export = new SchemaExport(this);
            export.Create(writer, false);
        }
        public void CreateDatabase()
        {
            var export = new SchemaExport(this);
            export.Create(false, true);
        }
        public void ExportMappings(TextWriter writer)
        {
            writer.Write(this.ConfiguredMapping.AsString());
        }
        public void ExportMappings()
        {
            if (this.ConfigurationExportSettings.ExportMappings)
            {
                if (File.Exists(this.ConfigurationExportSettings.MappingsFile))
                {
                    File.Delete(this.ConfigurationExportSettings.MappingsFile);
                }
                using (var tw = new StreamWriter(this.ConfigurationExportSettings.MappingsFile))
                {
                    ExportMappings(tw);
                }
            }
        }
        

        
    }
}
