using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBusV3
{
    /// <summary>
    ///定义事件源接口 所有的事件源都要要实现该接口
    /// </summary>
    public interface IEventData
    {
        /// <summary>
        /// 事件发生时间
        /// </summary>
        DateTime EventTime { get; set; }
        /// <summary>
        /// 触发事件的对象
        /// </summary>
        object EventStore { get; set; }
    }
}
