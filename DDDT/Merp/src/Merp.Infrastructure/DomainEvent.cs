using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Merp.Infrastructure
{
    public class DomainEvent : Message
    {
        public DateTime TimeStamp { get; private set; }

        public DomainEvent()
        {
            this.TimeStamp = DateTime.Now;
        }
    }
}
