using Contract.Core.Entities;
using Contract.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Contract.Core
{
    public class OrderNotificationPolicy : ISpecification<Order>
    {
        public OrderNotificationPolicy(string orderId = "")
        {
            Criteria = e => e.CreateTime > DateTimeOffset.UtcNow.AddDays(-1)
            && e.Id != orderId;
        }

        public System.Linq.Expressions.Expression<Func<Order, bool>> Criteria {get;}
    }
}
