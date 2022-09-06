﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBusV3
{
    //定义事件处理器公共接口，所有的事件处理都要实现该接口
    public interface IEventHandler
    {

    }

    public interface IEventHandler<TEventData>:IEventHandler where TEventData : IEventData
    {
        /// <summary>
        /// 事件处理器实现该方法来处理事件
        /// </summary>
        /// <param name="eventData"></param>
        void HandleEvent(TEventData eventData);
    }
}
