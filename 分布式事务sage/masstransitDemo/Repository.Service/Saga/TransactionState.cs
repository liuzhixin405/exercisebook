using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Service.Saga
{
    public class TransactionState : SagaStateMachineInstance
    {
        public Guid CorrelationId { get; set; }
        public State CurrentState { get; set; }

        // 这里可以添加你需要的其他属性
        public string CustomerName { get; set; }
        public decimal TotalAmount { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public bool CreateProductConsumerResult { get; set; }
        public bool CreateOrderConsumerResult { get; set; }
    }
}
