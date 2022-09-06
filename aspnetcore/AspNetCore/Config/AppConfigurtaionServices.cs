using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;

namespace AspNetCore.Config
{
    public class AppConfigurtaionServices
    {
        public static IConfiguration Configuration { get; set; }

        static AppConfigurtaionServices()
        {
            Configuration = new ConfigurationBuilder().Add(new JsonConfigurationSource() { Path= "appsettings.json",ReloadOnChange=true }).Build();

        }

        public static Entity GetSesionObject<Entity>(string keyPath=null) where Entity:new()
        {
            var entity = new Entity();
            if (string.IsNullOrEmpty(keyPath))
            {
                Configuration.Bind(entity);
            }
            else
            {
                var section = Configuration.GetSection(keyPath);
                section.Bind(entity);
            }
            return entity;
        }
    }
}
