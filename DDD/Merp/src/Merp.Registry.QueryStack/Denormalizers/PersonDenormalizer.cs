using Merp.Infrastructure;
using Merp.Registry.CommandStack.Events;
using Merp.Registry.QueryStack.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Merp.Registry.QueryStack.Denormalizers
{
    public class PersonDenormalizer : IHandleMessage<PersonRegisteredEvent>
    {
        public void Handle(PersonRegisteredEvent message)
        {
            var p = new Person()
            {
                FirstName = message.FirstName,
                LastName = message.LastName,
                OriginalId = message.PersonId,
                DisplayName = string.Format("{0} {1}", message.FirstName, message.LastName)
            };
            using(var context = new RegistryDbContext())
            {
                context.Parties.Add(p);
                context.SaveChanges();
            }

        }
    }
}
