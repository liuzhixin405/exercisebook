using Autofac;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Infrastructure.Logging
{
    public class LoggingModule:Module
    {
        private readonly ILogger _logger;

        internal LoggingModule(ILogger logger)
        {
            _logger = logger;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterInstance(_logger)
                .As<ILogger>()
                .SingleInstance();
        }
    }
}
