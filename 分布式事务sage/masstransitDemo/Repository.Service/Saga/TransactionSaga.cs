using MassTransit;
using Repository.Service.Consumers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Repository.Service.Saga
{
    public class TransactionSaga: MassTransitStateMachine<TransactionState>
    {
        public TransactionSaga()
        {
            Event(() => CreateOrderEvent);
            Event(() => CreateProductEvent);

            Initially(
                When(CreateOrderEvent)
                    .Then(context =>
                    {
                        // 处理成功的逻辑
                    })
                    .TransitionTo(Completed),
                When(CreateProductEvent)
                    .Then(context =>
                    {
                        // 处理成功的逻辑
                    })
                    .TransitionTo(Completed));

            // 处理补偿逻辑

        }

        public State Completed { get; private set; }
        public Event<ITransactionCreated> CreateOrderEvent { get; private set; }
        public Event<ITransactionCreated> CreateProductEvent { get; private set; }
    }
}
