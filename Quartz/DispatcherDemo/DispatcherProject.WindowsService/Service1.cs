using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using Ruanmou.DispatcherProject.Common;
using Ruanmou.DispatcherProject.QuartzNet;

namespace Ruanmou.DispatcherProject.WindowsService
{
    public partial class Service1 : ServiceBase
    {
        private Logger logger = new Logger(typeof(Service1));
        public Service1()
        {
            InitializeComponent();
            this.logger.Info("This is Service1 ctor start..");

            DispatcherManager.Init().GetAwaiter().GetResult();

            this.logger.Info("This is Service1 ctor end..");
        }

        protected override void OnStart(string[] args)
        {
            this.logger.Info("This is OnStart..");
        }

        protected override void OnStop()
        {
            this.logger.Info("This is OnStop..");
        }
    }
}
