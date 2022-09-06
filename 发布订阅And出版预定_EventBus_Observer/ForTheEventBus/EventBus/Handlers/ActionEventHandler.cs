﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.Handlers
{
    /// <summary>
    /// 支持Action的事件处理器
    /// </summary>
    /// <typeparam name="TEventData"></typeparam>
    internal class ActionEventHandler<TEventData> : IEventHandler<TEventData> where TEventData : IEventData
    {
        public Action<TEventData> Action { get; private set; }
        /// <summary>
        /// 定义Action的引用，并通过构造函数传参初始化
        /// </summary>
        /// <param name="handler"></param>
        public ActionEventHandler(Action<TEventData> handler)
        {
            Action = handler;
        }
        /// <summary>
        /// 调用具体的action来处理事件逻辑
        /// </summary>
        /// <param name="eventData"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void HandleEvent(TEventData eventData)
        {
            Action(eventData);
        }
    }
}
