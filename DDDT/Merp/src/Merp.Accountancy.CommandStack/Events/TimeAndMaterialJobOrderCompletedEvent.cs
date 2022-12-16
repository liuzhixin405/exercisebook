using Merp.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Merp.Accountancy.CommandStack.Events
{
    public class TimeAndMaterialJobOrderCompletedEvent : DomainEvent
    {
        public Guid JobOrderId { get; private set; }
        public DateTime DateOfCompletion { get; private set; }

        public TimeAndMaterialJobOrderCompletedEvent(Guid jobOrderId, DateTime dateOfCompletion)
        {
            this.JobOrderId = jobOrderId;
            this.DateOfCompletion = dateOfCompletion;
        }
    }
}
