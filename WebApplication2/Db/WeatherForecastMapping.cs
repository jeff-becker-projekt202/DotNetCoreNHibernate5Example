//using NHibernate.Mapping.ByCode;
//using NHibernate.Mapping.ByCode.Conformist;
//using WebApplication2.Models;

//namespace WebApplication2.Db
//{
//    public class WeatherForecastMapping : ClassMapping<WeatherForecast>
//    {
//        public WeatherForecastMapping()
//        {
//            Schema("weatherExample");
//            Table("WeatherForecast");
//            Id(x => x.Id, idCfg=>
//            {
//                idCfg.Generator(Generators.Identity);
//            });
//            Property(x => x.Date, propCfg =>
//              {
//                  propCfg.NotNullable(true);
//              });
//            Property(x => x.TemperatureC, propCfg =>
//            {
//                propCfg.NotNullable(true);
//            });
//            Property(x => x.Summary, propCfg =>
//             {
//                 propCfg.NotNullable(true);
//                 propCfg.Length(256);
//             });

//        }
//    }
//}
