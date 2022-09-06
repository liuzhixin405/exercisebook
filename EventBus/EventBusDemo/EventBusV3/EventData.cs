using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBusV3
{
    /// <summary>
    /// 事件源 描述事件信息,用于传递参数
    /// </summary>
    public class EventData : IEventData
    {
        public DateTime EventTime { get ; set ; }
        public object EventStore { get ; set; }

        public EventData()
        {
            EventTime = DateTime.Now;
        }
    }
}
