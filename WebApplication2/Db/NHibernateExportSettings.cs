using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication2.Db
{
    public class NHibernateExportSettings
    {
        public bool ExportSchema { get; set; } = false;
        public bool ExportMappings { get; set; } = false;
        public string SchemaFile { get; set; } = "weather.sql";
        public string MappingsFile { get; set; } = "weather.hbm.xml";
        public bool CreateDatabase { get; set; } = true;
    }
}
