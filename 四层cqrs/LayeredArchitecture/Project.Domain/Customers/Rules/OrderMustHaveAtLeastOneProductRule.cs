using Project.Domain.Customers.Orders;
using Project.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Domain.Customers.Rules
{
    public class OrderMustHaveAtLeastOneProductRule : IBusinessRule
    {
        public string Message => "Order must have at least one product";
        private readonly List<OrderProductData> _orderProductData;
        public OrderMustHaveAtLeastOneProductRule(List<OrderProductData> orderProductData)
        {
            _orderProductData = orderProductData;
        }

        public bool IsBroken() => !_orderProductData.Any();
    }
}
