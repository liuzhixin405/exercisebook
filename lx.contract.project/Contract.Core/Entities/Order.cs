using Contract.Core.Enum;
using Contract.Core.SharedKernel;
using Contract.SharedKernel;
using Contract.SharedKernel.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Contract.Core.Entities
{
    public class Order:EntityBase, IAggregateRoot
    {
        public Order(string contractId, decimal price, decimal size, int marginMode)
        {
            //unitMode 判断下单数量是U还是币种
            ContractId = contractId;
            Price = price;
            CumQty = size;
            MarginMode = (MarginMode)marginMode;
        }
        public Order()
        {

        }
        /// <summary>
        /// 合约id
        /// </summary>

        public String ContractId { get; set; }

        /// <summary>
        /// 价格
        /// </summary>
      
        public Decimal Price { get; set; }
        /// <summary>
        /// 已成交数量
        /// </summary>
    
        public Decimal CumQty { get; set; }
        /// <summary>
        /// 保证金模式
        /// </summary>
        public MarginMode MarginMode { set; get; }
        public OrderStatus OrderStatus { get; set; }

    }
}
