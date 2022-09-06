using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBus
{
    public class EventData : IEventData
    {
        public EventData()
        {
            EventTime = DateTime.Now;
        }

        public DateTime EventTime { get ; set ; }
        public object EventStore { get ; set ; }
    }
}
