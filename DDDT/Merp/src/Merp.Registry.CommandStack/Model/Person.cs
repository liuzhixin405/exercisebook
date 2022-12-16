using Merp.Registry.CommandStack.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Merp.Registry.CommandStack.Model
{
    public class Person : Party
    {
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        protected Person()
        {

        }

        public static class Factory
        {
            public static Person CreateNewEntry(string firstName, string lastName, DateTime? dateOfBirth)
            {
                var p = new Person()
                {
                     Id = Guid.NewGuid(),
                     FirstName = firstName,
                     LastName = lastName
                };
                var e = new PersonRegisteredEvent(p.Id, p.FirstName, p.LastName);
                p.RaiseEvent(e);
                return p;
            }
        }
    }
}
