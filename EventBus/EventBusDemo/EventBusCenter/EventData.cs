using EventBusCenter.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBusCenter
{
    public class EventData : IEventData
    {
        public DateTime EventTime { get ; set  ; }
        public object EventStore { get ; set ; }
        public EventData()
        {
            EventTime= DateTime.Now;
        }
    }
}
