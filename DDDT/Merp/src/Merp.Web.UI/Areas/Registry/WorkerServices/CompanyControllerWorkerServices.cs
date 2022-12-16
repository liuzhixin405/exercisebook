using Merp.Infrastructure;
using Merp.Registry.CommandStack.Commands;
using Merp.Registry.QueryStack;
using Merp.Web.UI.Areas.Registry.Models.Company;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Merp.Web.UI.Areas.Registry.WorkerServices
{
    public class CompanyControllerWorkerServices
    {
        public IBus Bus { get; private set; }
        public IDatabase Database { get; set; }

        public CompanyControllerWorkerServices(IBus bus, IDatabase database)
        {
            if(bus==null)
            {
                throw new ArgumentNullException("bus");
            }
            if (database == null)
            {
                throw new ArgumentNullException("database");
            }
            this.Bus = bus;
            this.Database = database;
        }

        public void AddEntry(AddEntryViewModel model)
        {
            var command = new RegisterCompanyCommand(model.CompanyName, model.VatIndex);
            Bus.Send(command);
        }
    }
}