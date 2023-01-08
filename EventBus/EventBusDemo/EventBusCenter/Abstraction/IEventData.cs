using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBusCenter.Abstraction
{
    public interface IEventData
    {
        DateTime EventTime { get; set; }
        object EventStore { get; set; } //观察者
    }
}
