using Automatonymous;
using GreenPipes;
using Shared.Contract.Events;
using Shared.Contract.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderApi.Saga.SubmitOrderSagas
{
    public class TakeProductActivity :
        Activity<OrderState, OrderAcceptedEvent>
    {
        public TakeProductActivity()
        {

        }
        public void Accept(StateMachineVisitor visitor)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Task Execute(BehaviorContext<OrderState, OrderAcceptedEvent> context, Behavior<OrderState, OrderAcceptedEvent> next)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 失败
        /// </summary>
        /// <typeparam name="TException"></typeparam>
        /// <param name="context"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Task Faulted<TException>(BehaviorExceptionContext<OrderState, OrderAcceptedEvent, TException> context, Behavior<OrderState, OrderAcceptedEvent> next) where TException : Exception
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 待定
        /// </summary>
        /// <param name="context"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void Probe(ProbeContext context)
        {
            throw new NotImplementedException();
        }
    }
}
