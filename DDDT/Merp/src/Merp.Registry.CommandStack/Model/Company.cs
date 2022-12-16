using Merp.Registry.CommandStack.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Merp.Registry.CommandStack.Model
{
    public class Company : Party
    {
        public string CompanyName { get; private set; }
        protected Company()
        {

        }

        public static class Factory
        {
            public static Company CreateNewEntry(string companyName, string vatIndex)
            {
                var p = new Company()
                {
                     Id = Guid.NewGuid(),
                     CompanyName = companyName,
                     VatIndex = vatIndex
                };
                var e = new CompanyRegisteredEvent(p.Id, p.CompanyName, p.VatIndex);
                p.RaiseEvent(e);
                return p;
            }
        }
    }
}
