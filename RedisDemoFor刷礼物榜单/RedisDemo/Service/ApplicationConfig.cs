using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace RedisDemo.Service
{
    public class ApplicationConfig
    {
        private static IConfiguration _configuration;


        public static IConfiguration Configuration
        {
            get
            {
                if (_configuration == null)
                {

                    ConfigurationBuilder();
                }
                return _configuration;
            }
        }
        private const string DefaultConfigFileName = "appsettings";
        private const string DefaultConfigFileSuffix = "json";
        private static void ConfigurationBuilder(string targetConfig = null)
        {
            string appConfigFile = string.IsNullOrEmpty(targetConfig)
                ? $"{DefaultConfigFileName}.{DefaultConfigFileSuffix}"
                : $"{DefaultConfigFileName}.{targetConfig}.{DefaultConfigFileSuffix}";
            IConfigurationBuilder builder = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile(appConfigFile);
            _configuration = builder.Build();
        }
    }
}
