using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.Handlers
{
    /// <summary>
    /// 定义事件处理器公共接口,所有的事件处理都要实现该接口
    /// </summary>
    public interface IEventHandler
    {

    }

    /// <summary>
    /// 泛型版本
    /// </summary>
    /// <typeparam name="TEventData"></typeparam>
    public interface IEventHandler<TEventData> : IEventHandler where TEventData : IEventData
    {
        /// <summary>
        /// 事件处理方法
        /// </summary>
        /// <param name="eventData"></param>
        void HandleEvent(TEventData eventData);
    }
}