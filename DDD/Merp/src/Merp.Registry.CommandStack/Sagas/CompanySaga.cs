using Merp.Infrastructure;
using Merp.Registry.CommandStack.Commands;
using Merp.Registry.CommandStack.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Merp.Registry.CommandStack.Sagas
{
    public class CompanySaga : Saga,
        IAmStartedBy<RegisterCompanyCommand>
    {
        public CompanySaga(IBus bus, IEventStore eventStore, IRepository repository)
            : base(bus, eventStore, repository)
        {
            
        }

        public void Handle(RegisterCompanyCommand message)
        {
            var person = Company.Factory.CreateNewEntry(message.CompanyName, message.VatIndex);
            Repository.Save<Company>(person);
        }
    }
}
